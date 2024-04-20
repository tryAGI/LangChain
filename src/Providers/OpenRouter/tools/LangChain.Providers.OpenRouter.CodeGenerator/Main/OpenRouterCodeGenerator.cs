using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using LangChain.Providers.OpenRouter.CodeGenerator.Classes;
using LangChain.Providers.OpenRouter.CodeGenerator.Helpers;
using static System.Globalization.CultureInfo;
using static System.Text.RegularExpressions.Regex;

// ReSharper disable LocalizableElement

namespace LangChain.Providers.OpenRouter.CodeGenerator.Main;

public static class OpenRouterCodeGenerator
{
    #region Public Methods

    /// <summary>
    ///     Generate Models and Enum files
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static async Task GenerateCodesAsync(GenerationOptions options)
    {
        options = options ?? throw new ArgumentNullException(nameof(options));
        
        //Initialize fields.
        var list = new List<ModelInfo>();

        var lbb = new DocumentHelper();

        //Load Open Router Docs Page...
        Console.WriteLine("Loading Model Page...");
        var str = await GetStringAsync(new Uri("https://openrouter.ai/docs")).ConfigureAwait(false);

        //Parse Html
        lbb.DocumentText = str;

        //Get Models Table on https://openrouter.ai/docs#models
        var trs = lbb.Document.DocumentNode.Descendants("tr").ToArray();
        Console.WriteLine($"{trs.Length - 2} Models Found...");

        //Run Parallel loop for each model
        await Parallel.ForAsync(2, trs.Length, new ParallelOptions { MaxDegreeOfParallelism = 8 },
            async (i, _) =>
            {
                var item = await ParseModelInfo(i, trs.ElementAt(i), options)
                    .ConfigureAwait(false);
                if (item != null)
                    list.Add(item);
            }).ConfigureAwait(false);

        //Sort Models by index
        var sorted = list.OrderBy(s => s.Index).ToList();

        //Create AllModels.cs
        Console.WriteLine("Creating AllModels.cs...");
        await CreateAllModelsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create OpenRouterModelIds.cs
        Console.WriteLine("Creating OpenRouterModelIds.cs...");
        await CreateOpenRouterModelIdsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create OpenRouterModelIds.cs
        Console.WriteLine("Creating OpenRouterModelProvider.cs...");
        await CreateOpenRouterModelProviderFile(sorted, options.OutputFolder).ConfigureAwait(false);

        Console.WriteLine("Task Completed!");
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Creates Codes\OpenRouterModelProvider.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateOpenRouterModelProviderFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        var first = true;
        foreach (var item in sorted)
        {
            if (first)
            {
                sb3.AppendLine(item.DicAddCode);
                first = false;
            }

            sb3.AppendLine($"\t\t{item.DicAddCode}");
        }

        var dicsAdd =
            H.Resources.OpenRouterModelProvider_cs.AsString().Replace("{{DicAdd}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "OpenRouterModelProvider.cs");
        await File.WriteAllTextAsync(fileName, dicsAdd).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Creates Codes\OpenRouterModelIds.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateOpenRouterModelIdsFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            var tem = item.EnumMemberCode?
                .Replace("\r\n", "\n", StringComparison.InvariantCulture)
                .Replace("\n", "\n\t\t", StringComparison.InvariantCulture)
                ;
            sb3.Append(tem);
        }

        var modelIdsContent =
            H.Resources.OpenRouterModelIds_cs.AsString().Replace("{{ModelIds}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "OpenRouterModelIds.cs");
        await File.WriteAllTextAsync(fileName, modelIdsContent).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Creates Codes\Predefined\AllModels.cs file
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateAllModelsFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            sb3.AppendLine(item.PredefinedClassCode);
            sb3.AppendLine();
        }

        var classesFileContent =
            H.Resources.AllModels_cs.AsString().Replace("{{OpenRouterClasses}}", sb3.ToString(), StringComparison.InvariantCulture);
        var path1 = Path.Join(outputFolder, "Predefined");
        Directory.CreateDirectory(path1);
        var fileName = Path.Combine(path1, "AllModels.cs");
        await File.WriteAllTextAsync(fileName, classesFileContent).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Parses Model info from open router docs
    /// </summary>
    /// <param name="i"></param>
    /// <param name="htmlNode"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static async Task<ModelInfo?> ParseModelInfo(int i, HtmlNode htmlNode, GenerationOptions options)
    {
        var td = htmlNode.Descendants("td").ToArray();

        //Modal Name
        var modelName = td.ElementAt(0).Descendants("a").ElementAt(0).InnerText;

        if (string.IsNullOrEmpty(modelName))
            return null;

        //Model Id
        var modelId = td.ElementAt(0).Descendants("code").ElementAt(0).InnerText;

        var enumMemberName = GetModelIdsEnumMemberFromName(modelId, modelName, options);

        var description = await GetDescription(i, td).ConfigureAwait(false);

        //Enum Member code with doc
        var enumMemberCode = GetEnumMemberCode(i, enumMemberName, description);

        //Parse Cost of Prompt/Input per 1000 token
        var inputTokenCostNode = td.ElementAt(1);
        var child = inputTokenCostNode.FirstChild.FirstChild;
        var promptCostText = child.InnerText.Replace("$", "", StringComparison.InvariantCulture);

        //Parse Cost of Completion/Output Tokens
        var outputTokenColumnNode = td.ElementAt(2);
        child = outputTokenColumnNode.FirstChild.FirstChild;
        var completionCostText = child.InnerText.Replace("$", "", StringComparison.InvariantCulture);

        //Parse Context Length
        var contextLengthNode = td.ElementAt(3);
        var tokenLength =
            contextLengthNode.FirstChild.FirstChild.InnerText.Replace(",", "", StringComparison.InvariantCulture);

        //Calculate Cost per Token
        if (double.TryParse(promptCostText, out var promptCost)) promptCost = promptCost / 1000;

        if (double.TryParse(completionCostText, out var completionCost)) completionCost = completionCost / 1000;


        //Code for adding ChatModel into Dictionary<OpenRouterModelIds,ChatModels>() 
        var dicAddCode = GetDicAddCode(enumMemberName, modelId, tokenLength, promptCost, completionCost);

        //Code for Predefined Model Class
        var predefinedClassCode = GetPreDefinedClassCode(enumMemberName);

        return new ModelInfo
        {
            DicAddCode = dicAddCode,
            EnumMemberName = enumMemberName,
            Index = i,
            ModelId = modelId,
            ModelName = modelName,
            PredefinedClassCode = predefinedClassCode,
            EnumMemberCode = enumMemberCode,
            Description = description
        };
    }

    private static string GetEnumMemberCode(int i, string enumMemberName, string description)
    {
        var sb2 = new StringBuilder();

        sb2.AppendLine($"\r\n/// <summary>\r\n/// {description} \r\n/// </summary>");

        sb2.AppendLine($"{enumMemberName} = {i - 2},");
        return sb2.ToString();
    }

    private static string GetPreDefinedClassCode(string enumMemberName)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            $"/// <inheritdoc cref=\"OpenRouterModelIds.{enumMemberName}\"/>\r\n/// <param name=\"provider\">Open Router Provider Instance</param>");
        sb.AppendLine(
            $"public class {enumMemberName.Replace("_", "", StringComparison.OrdinalIgnoreCase)}Model(OpenRouterProvider provider) : OpenRouterModel(provider, OpenRouterModelIds.{enumMemberName});");
        return sb.ToString();
    }

    private static string GetDicAddCode(string enumMemberName, string modelId, string tokenLength, double promptCost,
        double completionCost)
    {
        return "{ " +
               $"OpenRouterModelIds.{enumMemberName}, new ChatModels(\"{modelId}\",{tokenLength},{promptCost},{completionCost})" +
               "},";
    }

    /// <summary>
    ///     Creates Enum Member name from Model Name with C# convention
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="modelName"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static string GetModelIdsEnumMemberFromName(string modelId, string modelName, GenerationOptions options)
    {
        var enumType = Replace(modelName, "[_.: -]", "_");
        enumType = Replace(enumType, "[()]", "");

        enumType = enumType
            .Replace("__", "_", StringComparison.InvariantCulture)
            .Replace("_🐬", "", StringComparison.InvariantCulture);

        enumType = CurrentCulture.TextInfo.ToTitleCase(enumType
            .Replace("_", " ", StringComparison.InvariantCulture).ToLower(CurrentCulture));

        enumType = options.ReplaceEnumNameFunc?.Invoke(enumType, modelId, modelName) ?? enumType;

        return enumType;
    }

    /// <summary>
    ///     Fetches Model Page and parses the model description
    /// </summary>
    /// <param name="i"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    private static async Task<string> GetDescription(int i, IEnumerable<HtmlNode> tr)
    {
        var lbb = new DocumentHelper();
        var anchor = tr.ElementAt(0).Descendants("a").First();

        var href = anchor.GetAttributeValue("href", "");
        var url = $"https://openrouter.ai{href}";

        Console.WriteLine($"{i - 1} Fetching doc from {url}...");
        
        var path = Path.Combine("cache", href.Replace('/', '_') + ".html");
        if (File.Exists(path))
        {
            return await File.ReadAllTextAsync(path).ConfigureAwait(false);
        }
        
        var str = await GetStringAsync(new Uri(url)).ConfigureAwait(false);

        lbb.DocumentText = str;

        var msg =
            lbb.FindNode("div", "class", "prose-slate", true) ??
            throw new InvalidOperationException("Description not found");

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

                if (sb.ToString().Contains(text3, StringComparison.OrdinalIgnoreCase))
                    continue;
                sb.AppendLine($"/// {text3}  <br/>");
            }
        }

        var html = sb.ToString().Trim();
        
        Directory.CreateDirectory("cache");
        
        await File.WriteAllTextAsync(path, html).ConfigureAwait(false);
        
        return html;
    }

    /// <summary>
    ///     Creates HttpClient
    /// </summary>
    /// <returns></returns>
    private static async Task<string> GetStringAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var handler = new HttpClientHandler();
        handler.CheckCertificateRevocationList = true;
        handler.AutomaticDecompression =
            DecompressionMethods.Deflate | DecompressionMethods.Brotli | DecompressionMethods.GZip;
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("UserAgent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(5));
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                return await client.GetStringAsync(uri, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            }
        }
        
        throw new TaskCanceledException();
    }

    #endregion
}