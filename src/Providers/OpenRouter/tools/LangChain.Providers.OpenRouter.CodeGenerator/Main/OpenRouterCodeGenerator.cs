using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using LangChain.Providers.OpenRouter.CodeGenerator.Helpers;
using LangChain.Providers.OpenRouter.CodeGenerator.Properties;
using static System.Globalization.CultureInfo;
using static System.Net.Mime.MediaTypeNames;
using static System.Text.RegularExpressions.Regex;

namespace LangChain.Providers.OpenRouter.CodeGenerator.Main;

public class OpenRouterCodeGenerator
{
    #region Fields
    private readonly List<Tuple<string, string>> forReplace = new([
        new Tuple<string, string>(" ", ""),
        new Tuple<string, string>("BV0", "Bv0"),
        new Tuple<string, string>("7b", "7B"),
        new Tuple<string, string>("bBa", "BBa"),
        new Tuple<string, string>("bSf", "BSf"),
        new Tuple<string, string>("DPO", "Dpo"),
        new Tuple<string, string>("SFT", "Sft"),
        new Tuple<string, string>("Openai", "OpenAi"),
        new Tuple<string, string>("Openchat", "OpenChat"),
        new Tuple<string, string>("Openher", "OpenHer"),
        new Tuple<string, string>("Openorca", "OpenOrca")
    ]);
    #endregion

    #region Public Methods
    /// <summary>
    ///     Generate Models and Enum files
    /// </summary>
    /// <param name="includeUnderScoresInEnum">Should add underscore in Enum?</param>
    /// <returns></returns>
    public async Task GenerateCodesAsync(bool includeUnderScoresInEnum)
    {
        //Initialize fields.
        var list = new List<Tuple<int, string, string, string>>();

        if (includeUnderScoresInEnum) forReplace[0] = new Tuple<string, string>(" ", "_");
       
        using var client = CreateClient();
        var lbb = new DocumentHelper();

        //Load Open AI Page...
        Console.WriteLine("Loading Model Page...");
        var str = await client.GetStringAsync(new Uri("https://openrouter.ai/docs")).ConfigureAwait(false);

        //Parse Html
        lbb.DocumentText = str;

        //Get Models Table on https://openrouter.ai/docs#models
        var trs = lbb.Document.DocumentNode.Descendants("tr");
        Console.WriteLine($"{trs.Count() - 2} Models Found...");

        //Run Parallel loop for each model
        await Parallel.ForAsync(2, trs.Count(), new ParallelOptions { MaxDegreeOfParallelism = 2 },
            async (i, cancellation) =>
            {
                var item = await ParseModelInfo(i, trs.ElementAt(i), includeUnderScoresInEnum)
                    .ConfigureAwait(false);
                if (item != null)
                    list.Add(item);
            }).ConfigureAwait(false);

        //Sort Models by index
        var sorted = list.OrderBy(s => s.Item1).ToList();

        //Create AllModels.cs
        Console.WriteLine("Creating AllModels.cs...");
        await CreateAllModelsFile(sorted).ConfigureAwait(false);

        //Create OpenRouterModelIds.cs
        Console.WriteLine("Creating OpenRouterModelIds.cs...");
        await CreateOpenRouterModelIdsFile(sorted).ConfigureAwait(false);

        //Create OpenRouterModelIds.cs
        Console.WriteLine("Creating OpenRouterModelProvider.cs...");
        await CreateOpenRouterModelProviderFile(sorted).ConfigureAwait(false);

        Console.WriteLine("Task Completed!");
    }

    #endregion
    
    #region Private Methods
    /// <summary>
    /// Creates Codes\OpenRouterModelProvider.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    private async Task CreateOpenRouterModelProviderFile(List<Tuple<int, string, string, string>> sorted)
    {
        var sb3 = new StringBuilder();
        var first = true;
        foreach (var item in sorted)
        {
            if (first)
            {
                sb3.AppendLine(item.Item3);
                first = false;
            }

            sb3.AppendLine($"\t\t\t\t{item.Item3}");
        }

        var dicsAdd =
            Resources.OpenRouterModelProvider.Replace("{{DicAdd}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory("Codes");
        await File.WriteAllTextAsync("Codes\\OpenRouterModelProvider.cs", dicsAdd).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates Codes\OpenRouterModelIds.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    private async Task CreateOpenRouterModelIdsFile(List<Tuple<int, string, string, string>> sorted)
    {
        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            var tem = item.Item2.Replace("\r\n", "\r\n\t\t", StringComparison.InvariantCulture);
            sb3.Append(tem);
        }


        var modelIdsContent =
            Resources.OpenRouterModelIds.Replace("{{ModelIds}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory("Codes");
        await File.WriteAllTextAsync("Codes\\OpenRouterModelIds.cs", modelIdsContent).ConfigureAwait(false);
    }
    /// <summary>
    /// Creates Codes\Predefined\AllModels.cs file
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    private async Task CreateAllModelsFile(List<Tuple<int, string, string, string>> sorted)
    {
        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            sb3.AppendLine(item.Item4);
            sb3.AppendLine();
        }

        var classesFileContent =
            Resources.AllModels.Replace("{{OpenRouterClasses}}", sb3.ToString(), StringComparison.InvariantCulture);

        Directory.CreateDirectory("Codes\\Predefined");
        await File.WriteAllTextAsync("Codes\\Predefined\\AllModels.cs", classesFileContent).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses Model info from open router docs
    /// </summary>
    /// <param name="i"></param>
    /// <param name="htmlNode"></param>
    /// <param name="includeUnderScoresInEnum"></param>
    /// <returns></returns>
    private async Task<Tuple<int, string, string, string>?> ParseModelInfo(int i, HtmlNode htmlNode, bool includeUnderScoresInEnum)
    {
        var tr = htmlNode.Descendants("td");
        
        //Modal Name
        var modelName = tr.ElementAt(0).Descendants("a").ElementAt(0).InnerText;

        if (string.IsNullOrEmpty(modelName))
            return null;

        //Model Id
        var modelId = tr.ElementAt(0).Descendants("code").ElementAt(0).InnerText;

        var enumType = NormalizeEnumType(modelName);

        if (modelId == "openai/gpt-3.5-turbo-0125")
            enumType = includeUnderScoresInEnum ? "OpenAi_Gpt_3_5_Turbo_16K_0125" : "OpenAiGpt35Turbo16K0125";

        var description = await GetDescription(i, tr);

        ///Builds Enum Member with Doc
        var sb2 = new StringBuilder();

        sb2.AppendLine($"\r\n/// <summary>\r\n/// {description} \r\n/// </summary>");

        sb2.AppendLine($"{enumType} = {i - 2},");

        //Tuple Item2 is Enum Member with doc
        var item2 = sb2.ToString();

        //Parse Cost of Prompt/Input per 1000 token
        var text2 = tr.ElementAt(1);
        var child = text2.FirstChild.FirstChild;
        var promptCost = child.InnerText.Replace("$", "", StringComparison.InvariantCulture);

        //Parse Cost of Completion/Output Tokens
        text2 = tr.ElementAt(2);
        child = text2.FirstChild.FirstChild;
        var completionCost = child.InnerText.Replace("$", "", StringComparison.InvariantCulture);

        //Parse Context Length
        var text3 = tr.ElementAt(3);
        var tokenLength = text3.FirstChild.FirstChild.InnerText.Replace(",", "", StringComparison.InvariantCulture);

        //Calculate Cost per Token
        var promptCost2 = double.Parse(promptCost) / 1000;
        var completionCost2 = double.Parse(completionCost) / 1000;

        //item3 is a Dictionary<OpenRouterModelIds,ChatModels>() item
        var item3 = "{ " +
                    $"OpenRouterModelIds.{enumType}, new ChatModels(\"{modelId}\",{tokenLength},{promptCost2},{completionCost2})" +
                    "},";

        //Item4 is the Predefined Model Class
        var sb = new StringBuilder();
        sb.AppendLine(
            $"/// <inheritdoc cref=\"OpenRouterModelIds.{enumType}\"/>\r\n/// <param name=\"provider\">Open Router Provider Instance</param>");
        sb.AppendLine(
            $"public class {enumType.Replace("_", "")}Model(OpenRouterProvider provider):OpenRouterModel(provider, OpenRouterModelIds.{enumType});");
        var item4 = sb.ToString();

        return new Tuple<int, string, string, string>(i, item2, item3, item4);
    }

    /// <summary>
    /// Creates Enum Member name from Model Name with C# convention
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string NormalizeEnumType(string modelName)
    {
        var enumType = Replace(modelName, "[_.: -]", "_");
        enumType = Replace(enumType, "[()]", "");

        enumType = enumType.Replace("__", "_", StringComparison.InvariantCulture)
            .Replace("_🐬", "", StringComparison.InvariantCulture);

        enumType = CurrentCulture.TextInfo.ToTitleCase(enumType
            .Replace("_", " ", StringComparison.InvariantCulture).ToLower(CurrentCulture));

        foreach (var tuple in forReplace)
            enumType = enumType.Replace(tuple.Item1, tuple.Item2, StringComparison.InvariantCulture);

        return enumType;
    }

    /// <summary>
    /// Fetches Model Page and parses the model description
    /// </summary>
    /// <param name="i"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    private async Task<string> GetDescription(int i, IEnumerable<HtmlNode> tr)
    {
        using var client = CreateClient();
        var lbb = new DocumentHelper();
        var anchor = tr.ElementAt(0).Descendants("a").FirstOrDefault();

        var href = anchor.GetAttributeValue("href", "");
        var url = $"https://openrouter.ai{href}";

        Console.WriteLine($"{i} Fetching doc from {url}...");

        var str = await client.GetStringAsync(new Uri(url)).ConfigureAwait(false);

        lbb.DocumentText = str;

        var msg = lbb.FindNode("div", "class", "prose-slate", true);

        var sb = new StringBuilder();
        var first = true;
        foreach (var child in msg.ChildNodes)
        {
            if (string.IsNullOrEmpty(child.InnerText.Trim())) continue;
            var text = child.InnerText.Trim().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var text2 in text)
            {
                var text3 = HttpUtility.HtmlDecode(text2);
                if (first)
                {
                    sb.AppendLine(text3 + "  <br/>");
                    first = false;
                }

                if (sb.ToString().Contains(text3))
                    continue;
                sb.AppendLine($"/// {text3}  <br/>");
            }
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Creates HttpClient
    /// </summary>
    /// <returns></returns>
    private HttpClient CreateClient()
    {
        var handler = new HttpClientHandler();
        handler.AutomaticDecompression =
            DecompressionMethods.Deflate | DecompressionMethods.Brotli | DecompressionMethods.GZip;
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("UserAgent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        return client;
    }
    
    #endregion
}