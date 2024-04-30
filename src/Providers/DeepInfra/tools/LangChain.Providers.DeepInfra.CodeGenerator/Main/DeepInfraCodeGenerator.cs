using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using LangChain.Providers.DeepInfra.CodeGenerator.Classes;
using LangChain.Providers.DeepInfra.CodeGenerator.Helpers;
using Newtonsoft.Json.Linq;
using static System.Globalization.CultureInfo;
using static System.Text.RegularExpressions.Regex;

// ReSharper disable LocalizableElement

namespace LangChain.Providers.DeepInfra.CodeGenerator.Main;

public static class DeepInfraCodeGenerator
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
        
        
        //Load Deep Infra Docs Page...
        Console.WriteLine("Loading Models...");
        var models = await GetModelsAsync(options).ConfigureAwait(false);

        
        Console.WriteLine($"{models.Count} Models Found...");


        //Sort Models by index
        var sorted = models.OrderBy(s => s.Index).ToList();

        //Create AllModels.cs
        Console.WriteLine("Creating AllModels.cs...");
        await CreateAllModelsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create DeepInfraModelIds.cs
        Console.WriteLine("Creating DeepInfraModelIds.cs...");
        await CreateDeepInfraModelIdsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create DeepInfraModelIds.cs
        Console.WriteLine("Creating DeepInfraModelProvider.cs...");
        await CreateDeepInfraModelProviderFile(sorted, options.OutputFolder).ConfigureAwait(false);

        Console.WriteLine("Task Completed!");
    }

    private static async Task<List<ModelInfo>> GetModelsAsync(GenerationOptions options)
    {
        var str = await GetStringAsync(new Uri("https://deepinfra.com/models/text-generation")).ConfigureAwait(false);
        var lbb = new DocumentHelper();
        var list = new List<ModelInfo>();
        int index = 0;
        //Parse Html
        var hashSet = new HashSet<string?>();
        do
        {
            lbb.DocumentText = str ?? string.Empty;
            var links = lbb.FindNode("script", "type", "json", true);

            if (links == null)
                throw new InvalidOperationException("Model Info script node not found in the HTML document.");

            var json = JObject.Parse(links.InnerText);

            var models = (JArray)json.SelectToken("props.pageProps.models")!;

            foreach (var model in models)
            {
                var modelInfo = ParseModelInfo(index, model, options);
                if (modelInfo != null && hashSet.Add(modelInfo.EnumMemberCode))
                {
                    list.Add(modelInfo);
                }
            }
            
            var nextPage = lbb.FindNode("a", "aria-label", "next page", true);
            if (nextPage != null)
            {
                str = await GetStringAsync(new Uri($"https://deepinfra.com{nextPage.GetAttributeValue("href", "")}"))
                    .ConfigureAwait(false);
                if (string.IsNullOrEmpty(str))
                    break;
            }
            else break;

        } while (true);
        return list;
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Creates Codes\DeepInfraModelProvider.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateDeepInfraModelProviderFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        var first = true;
        HashSet<string?> keys = new HashSet<string?>();
        foreach (var item in sorted)
        {
            if(!keys.Add(item.EnumMemberName))
                continue;

            if (first)
            {
                sb3.AppendLine(item.DicAddCode);
                first = false;
                continue;
            }

            sb3.AppendLine($"        {item.DicAddCode}");
        }

        var dicsAdd =
            H.Resources.DeepInfraModelProvider_cs.AsString().Replace("{{DicAdd}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "DeepInfraModelProvider.cs");
        await File.WriteAllTextAsync(fileName, dicsAdd).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Creates Codes\DeepInfraModelIds.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateDeepInfraModelIdsFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            var tem = item.EnumMemberCode?
                .Replace("\r\n", "\n", StringComparison.InvariantCulture)
                .Replace("\n", "\n        ", StringComparison.InvariantCulture)
                ;
            sb3.Append(tem);
        }

        var modelIdsContent =
            H.Resources.DeepInfraModelIds_cs.AsString().Replace("{{ModelIds}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "DeepInfraModelIds.cs");
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
            H.Resources.AllModels_cs.AsString().Replace("{{DeepInfraClasses}}", sb3.ToString(), StringComparison.InvariantCulture);
        var path1 = Path.Join(outputFolder, "Predefined");
        Directory.CreateDirectory(path1);
        var fileName = Path.Combine(path1, "AllModels.cs");
        await File.WriteAllTextAsync(fileName, classesFileContent).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Parses Model info from Deep Infra docs
    /// </summary>
    /// <param name="i"></param>
    /// <param name="modelToken"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static ModelInfo? ParseModelInfo(int i, JToken? modelToken, GenerationOptions options)
    {
        if (modelToken == null)
            return null;

       
        //Modal Name
        var modelName = (string)modelToken["name"]!;

        if (string.IsNullOrEmpty(modelName))
            return null;

        //Model Id
        var modelId = (string)modelToken["full_name"]!;

        var organization = (string)modelToken["owner"]!;
        var enumMemberName = GetModelIdsEnumMemberFromName(modelId, modelName, options);


        
        var contextLength = (int)(modelToken["max_tokens"] ?? 0);
        var tokenLength = contextLength.ToString();
        var promptCost = Math.Round((double)(modelToken.SelectToken("pricing.cents_per_input_token") ?? 0) * 10000,2);
        var completionCost = Math.Round((double)(modelToken.SelectToken("pricing.cents_per_output_token") ?? 0) * 10000,2);

        var description =
            FormattableString.Invariant($"Name: {modelName} <br/>\r\n/// Organization: {organization} <br/>\r\n/// Context Length: {contextLength} <br/>\r\n/// Prompt Cost: ${promptCost}/MTok <br/>\r\n/// Completion Cost: ${promptCost}/MTok <br/>\r\n/// Description: {(string?)modelToken["description"]} <br/>\r\n/// HuggingFace Url: <a href=\"https://huggingface.co/{modelId}\">https://huggingface.co/{modelId}</a>");

        //Enum Member code with doc
        var enumMemberCode = GetEnumMemberCode(enumMemberName, description);

        //Code for adding ChatModel into Dictionary<Together AiModelIds,ChatModels>() 
        var dicAddCode = GetDicAddCode(enumMemberName, modelId, tokenLength, promptCost / (1000 * 1000),
            completionCost / (1000 * 1000));

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

    private static string GetEnumMemberCode(string enumMemberName, string description)
    {
        var sb2 = new StringBuilder();

        sb2.AppendLine($"\r\n/// <summary>\r\n/// {description} \r\n/// </summary>");

        sb2.AppendLine($"{enumMemberName},");
        return sb2.ToString();
    }

    private static string GetPreDefinedClassCode(string enumMemberName)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            $"/// <inheritdoc cref=\"DeepInfraModelIds.{enumMemberName}\"/>\r\n/// <param name=\"provider\">Deep Infra Provider Instance</param>");
        sb.AppendLine(
            $"public class {enumMemberName.Replace("_", "", StringComparison.OrdinalIgnoreCase)}Model(DeepInfraProvider provider) : DeepInfraModel(provider, DeepInfraModelIds.{enumMemberName});");
        return sb.ToString();
    }

    private static string GetDicAddCode(string enumMemberName, string modelId, string tokenLength, double promptCost,
        double completionCost)
    {
        return "{ " +
               FormattableString.Invariant($"DeepInfraModelIds.{enumMemberName}, new ChatModels(\"{modelId}\",{tokenLength},{promptCost},{completionCost})") +
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
        var url = $"https://DeepInfra.ai{href}";

        Console.WriteLine($"{i - 1} Fetching doc from {url}...");
        
        var path = Path.Combine("cache", href.Replace('/', '_') + ".html");
        if (File.Exists(path))
        {
            return await File.ReadAllTextAsync(path).ConfigureAwait(false);
        }
        
        var str = await GetStringAsync(new Uri(url)).ConfigureAwait(false);

        lbb.DocumentText = str ?? string.Empty;

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
    private static async Task<string?> GetStringAsync(Uri uri, CancellationToken cancellationToken = default)
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
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            }
        }
        
        throw new TaskCanceledException();
    }

    #endregion
}