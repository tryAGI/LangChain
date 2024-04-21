using System.Net;
using System.Text;
using H;
using LangChain.Providers.TogetherAi.CodeGenerator.Classes;
using Newtonsoft.Json.Linq;
using static System.Globalization.CultureInfo;
using static System.Text.RegularExpressions.Regex;

// ReSharper disable LocalizableElement

namespace LangChain.Providers.TogetherAi.CodeGenerator.Main;

public static class TogetherAiCodeGenerator
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

        if (string.IsNullOrEmpty(options.TogetherAiApiKey))
            throw new ArgumentException(nameof(options.TogetherAiApiKey));

        //Initialize fields.
        var list = new List<ModelInfo>();

        //Load Together Ai Model Info...
        Console.WriteLine("Loading Model Page...");
        var models = await GetModelInfosAsync(options).ConfigureAwait(false);
        Console.WriteLine($"{models.Count} Models Found...");


        //Run loop for each model
        var duplicateSet = new HashSet<string?>();

        int count = 0;
        for (var i = 0; i < models.Count; i++)
        {
            var item = await ParseModelInfo(count, (JObject)models[i]!, options)
                .ConfigureAwait(false);

            if (item != null && duplicateSet.Add(item.EnumMemberCode))
            {
                list.Add(item);
                count++;
            }
        }

        //Sort Models by index
        var sorted = list.OrderBy(s => s.Index).ToList();

        //Create AllModels.cs
        Console.WriteLine("Creating AllModels.cs...");
        await CreateAllModelsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create TogetherAiModelIds.cs
        Console.WriteLine("Creating TogetherAiModelIds.cs...");
        await CreateTogetherAiModelIdsFile(sorted, options.OutputFolder).ConfigureAwait(false);

        //Create TogetherAiModelIds.cs
        Console.WriteLine("Creating TogetherAiModelProvider.cs...");
        await CreateTogetherAiModelProviderFile(sorted, options.OutputFolder).ConfigureAwait(false);

        Console.WriteLine($"{count} Models added into repo.");
        Console.WriteLine("Task Completed!");
    }

    private static async Task<JArray> GetModelInfosAsync(GenerationOptions options)
    {
        var modelInfoText = await GetStringAsync(new Uri("https://api.together.xyz/api/models?&info"), options)
            .ConfigureAwait(false);

        return JArray.Parse(modelInfoText);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Creates Codes\TogetherAiModelProvider.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateTogetherAiModelProviderFile(List<ModelInfo> sorted, string outputFolder)
    {
        var sb3 = new StringBuilder();
        var first = true;
        var keys = new HashSet<string?>();
        foreach (var item in sorted)
        {
            if (!keys.Add(item.EnumMemberName))
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
            Resources.TogetherAiModelProvider_cs.AsString()
                .Replace("{{DicAdd}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "TogetherAiModelProvider.cs");
        await File.WriteAllTextAsync(fileName, dicsAdd).ConfigureAwait(false);
        Console.WriteLine($"Saved to {fileName}");
    }

    /// <summary>
    ///     Creates Codes\TogetherAiModelIds.cs
    /// </summary>
    /// <param name="sorted"></param>
    /// <param name="outputFolder"></param>
    /// <returns></returns>
    private static async Task CreateTogetherAiModelIdsFile(List<ModelInfo> sorted, string outputFolder)
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
            Resources.TogetherAiModelIds_cs.AsString()
                .Replace("{{ModelIds}}", sb3.ToString(), StringComparison.InvariantCulture);
        Directory.CreateDirectory(outputFolder);
        var fileName = Path.Combine(outputFolder, "TogetherAiModelIds.cs");
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
            Resources.AllModels_cs.AsString()
                .Replace("{{TogetherAiClasses}}", sb3.ToString(), StringComparison.InvariantCulture);
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
    /// <param name="modelToken"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static async Task<ModelInfo?> ParseModelInfo(int i, JObject? modelToken, GenerationOptions options)
    {
        if (modelToken == null)
            return null;

        if ((string?)modelToken["display_type"] != "chat" && (string?)modelToken["display_type"] != "code")
            return null;

        if (!modelToken.ContainsKey("instances")) return null;

        //Modal Name
        var modelName = (string)modelToken["display_name"]!;

        if (string.IsNullOrEmpty(modelName))
            return null;

        //Model Id
        var modelId = (string)modelToken["name"]!;

        var organization = (string)modelToken["creator_organization"]!;
        var enumMemberName = GetModelIdsEnumMemberFromName(modelId, modelName, options);


        var length = modelToken["context_length"];
        if (length == null)
            return null;
        var contextLength = (int)(modelToken["context_length"] ?? 0);
        var tokenLength = contextLength.ToString();
        var promptCost = (double)(modelToken.SelectToken("pricing.input") ?? 0) * 0.004;
        var completionCost = (double)(modelToken.SelectToken("pricing.output") ?? 0) * 0.004;

        var description =
            $"Name: {(string)modelName} <br/>\r\n/// Organization: {organization} <br/>\r\n/// Context Length: {contextLength} <br/>\r\n/// Prompt Cost: ${promptCost}/MTok <br/>\r\n/// Completion Cost: ${promptCost}/MTok <br/>\r\n/// Description: {(string)modelToken["description"]} <br/>\r\n/// HuggingFace Url: <a href=\"https://huggingface.co/{modelId}\">https://huggingface.co/{modelId}</a>";

        //Enum Member code with doc
        var enumMemberCode = GetEnumMemberCode(i, enumMemberName, description);

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

    private static string GetEnumMemberCode(int i, string enumMemberName, string description)
    {
        var sb2 = new StringBuilder();

        sb2.AppendLine($"\r\n/// <summary>\r\n/// {description} \r\n/// </summary>");

        sb2.AppendLine($"{enumMemberName} = {i},");
        return sb2.ToString();
    }

    private static string GetPreDefinedClassCode(string enumMemberName)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            $"/// <inheritdoc cref=\"TogetherAiModelIds.{enumMemberName}\"/>\r\n/// <param name=\"provider\">Open Router Provider Instance</param>");
        sb.AppendLine(
            $"public class {enumMemberName}Model(TogetherAiProvider provider) : TogetherAiModel(provider, TogetherAiModelIds.{enumMemberName});");
        return sb.ToString();
    }

    private static string GetDicAddCode(string enumMemberName, string modelId, string tokenLength, double promptCost,
        double completionCost)
    {
        return "{ " +
               $"TogetherAiModelIds.{enumMemberName}, new ChatModels(\"{modelId}\",{tokenLength},{promptCost},{completionCost})" +
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
    ///     Get String From Uri
    /// </summary>
    /// <returns></returns>
    private static async Task<string> GetStringAsync(Uri uri, GenerationOptions options,
        CancellationToken cancellationToken = default)
    {
        using var handler = new HttpClientHandler();
        //handler.CheckCertificateRevocationList = true;
        handler.AutomaticDecompression =
            DecompressionMethods.Deflate | DecompressionMethods.Brotli | DecompressionMethods.GZip;
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("UserAgent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.TogetherAiApiKey}");

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(5));

        while (!cancellationToken.IsCancellationRequested)
            try
            {
                return await client.GetStringAsync(uri, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            }

        throw new TaskCanceledException();
    }

    #endregion
}