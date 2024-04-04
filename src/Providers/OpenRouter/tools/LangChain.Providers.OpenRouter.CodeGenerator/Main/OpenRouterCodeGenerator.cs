using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using LangChain.Providers.OpenRouter.CodeGenerator.Helpers;
using LangChain.Providers.OpenRouter.CodeGenerator.Properties;
using static System.Globalization.CultureInfo;

namespace LangChain.Providers.OpenRouter.CodeGenerator.Main;

public class OpenRouterCodeGenerator
{
    /// <summary>
    ///     Generate Models and Enum files
    /// </summary>
    /// <returns></returns>
    public async Task GenerateCodesAsync(bool includeUnderScoresInEnum)
    {
        var client = CreateClient();
        var lbb = new DocumentHelper();
        Console.WriteLine("Loading Model Page...");
        var str = await client.GetStringAsync("https://openrouter.ai/docs#quick-start").ConfigureAwait(false);

        var forReplace = new List<Tuple<string, string>>([
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

        if (includeUnderScoresInEnum)
        {
            forReplace[0] = new Tuple<string, string>(" ","_");
        }
        lbb.DocumentText = str;
        var trs = lbb.Document.DocumentNode.Descendants("tr");
        Console.WriteLine($"{trs.Count() - 2} Models Found...");
        var list = new List<Tuple<int, string, string, string>>();

        await Parallel.ForAsync(2, trs.Count(), new ParallelOptions { MaxDegreeOfParallelism = 2 },
            async (i, cancellation) =>
            {
                var tr = trs.ElementAt(i).Descendants("td");
                var text = tr.ElementAt(0).Descendants("a").ElementAt(0).InnerText;

                if (string.IsNullOrEmpty(text))
                    return;

                var modelName = tr.ElementAt(0).Descendants("code").ElementAt(0).InnerText;

                var enumType = Regex.Replace(text, "[_.: -]", "_");
                enumType = Regex.Replace(enumType, "[()]", "");

                enumType = enumType.Replace("__", "_", StringComparison.InvariantCulture)
                    .Replace("_🐬", "", StringComparison.InvariantCulture);

                enumType = CurrentCulture.TextInfo.ToTitleCase(enumType
                    .Replace("_", " ", StringComparison.InvariantCulture).ToLower());

                foreach (var tuple in forReplace)
                    enumType = enumType.Replace(tuple.Item1, tuple.Item2, StringComparison.InvariantCulture);

                if (modelName == "openai/gpt-3.5-turbo-0125") enumType = includeUnderScoresInEnum? "OpenAi_Gpt_3_5_Turbo_16K_0125" : "OpenAiGpt35Turbo16K0125";
                var description = await GetDescription(i, tr);

                var sb2 = new StringBuilder();

                sb2.AppendLine($"\r\n/// <summary>\r\n/// {description} \r\n/// </summary>");

                sb2.AppendLine($"{enumType} = {i - 2},");

                var item2 = sb2.ToString();

                var text2 = tr.ElementAt(1);
                var child = text2.FirstChild.FirstChild;

                var promptCost = child.InnerText.Replace("$", "",StringComparison.InvariantCulture);

                text2 = tr.ElementAt(2);
                child = text2.FirstChild.FirstChild;

                var completionCost = child.InnerText.Replace("$", "",StringComparison.InvariantCulture);

                var text3 = tr.ElementAt(3);

                var tokenLength = text3.FirstChild.FirstChild.InnerText.Replace(",", "",StringComparison.InvariantCulture);
               
                var promptCost2 = double.Parse(promptCost) / 1000;
                var completionCost2 = double.Parse(completionCost) / 1000;
                var item3 = "{ " + $"OpenRouterModelIds.{enumType}, new ChatModels(\"{modelName}\",{tokenLength},{promptCost2},{completionCost2})" + "},";

                var sb = new StringBuilder();
                sb.AppendLine(
                    $"/// <inheritdoc cref=\"OpenRouterModelIds.{enumType}\"/>\r\n/// <param name=\"provider\">Open Router Provider Instance</param>");
                sb.AppendLine(
                    $"public class {enumType.Replace("_", "")}Model(OpenRouterProvider provider):OpenRouterModel(provider, OpenRouterModelIds.{enumType});");
                var item4 = sb.ToString();

                list.Add(new Tuple<int, string, string, string>(i, item2, item3, item4));
            }).ConfigureAwait(false);

        var sorted = list.OrderBy(s => s.Item1);
        foreach (var enumType in sorted) Console.WriteLine(enumType);

        var sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            sb3.AppendLine(item.Item4);
            sb3.AppendLine();
        }

        Directory.CreateDirectory("Codes");
        Directory.CreateDirectory("Codes\\Predefined");
        var classesFileContent = Resources.AllModels.Replace("{{OpenRouterClasses}}", sb3.ToString(),StringComparison.InvariantCulture);
        await File.WriteAllTextAsync("Codes\\Predefined\\AllModels.cs", classesFileContent);

        sb3 = new StringBuilder();
        foreach (var item in sorted)
        {
            var tem = item.Item2.Replace("\r\n","\r\n\t\t",StringComparison.InvariantCulture);
            sb3.Append(tem);
        }

        var st4 = sb3.ToString();

        var modelIdsContent = Resources.OpenRouterModelIds.Replace("{{ModelIds}}", sb3.ToString(),StringComparison.InvariantCulture);
        await File.WriteAllTextAsync("Codes\\OpenRouterModelIds.cs", modelIdsContent);


        sb3 = new StringBuilder();
        bool first = true;
        foreach (var item in sorted)
        {
            if (first)
            {
                sb3.AppendLine(item.Item3);
                first = false;
            }

            sb3.AppendLine($"\t\t\t\t{item.Item3}");

        }

        var dicsAdd = Resources.OpenRouterModelProvider.Replace("{{DicAdd}}", sb3.ToString(),StringComparison.InvariantCulture);
        await File.WriteAllTextAsync("Codes\\OpenRouterModelProvider.cs", dicsAdd);
    }

    private async Task<string> GetDescription(int i, IEnumerable<HtmlNode> tr)
    {
        var client = CreateClient();
        var lbb = new DocumentHelper();
        var anchor = tr.ElementAt(0).Descendants("a").FirstOrDefault();

        var href = anchor.GetAttributeValue("href", "");
        var url = $"https://openrouter.ai{href}";

        Console.WriteLine($"{i} Fetching doc from {url}...");

        var str = await client.GetStringAsync(url);

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

    private HttpClient CreateClient()
    {
        var client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.Deflate | DecompressionMethods.Brotli | DecompressionMethods.GZip
        });
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("UserAgent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        return client;
    }
}