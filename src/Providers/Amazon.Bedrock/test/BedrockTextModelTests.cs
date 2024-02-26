﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace LangChain.Providers.Amazon.Bedrock.Tests;

[TestFixture, Explicit]
public class BedrockTextModelTests
{
    [Test]
    public Task TestAllTextLLMs()
    {
        var srcDir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\..\";
        var predefined = @"libs\Providers\LangChain.Providers.Amazon.Bedrock\Predefined\";
        var predefinedDir = Path.Combine(srcDir, predefined); 
        
        var assembly = Assembly.GetAssembly(typeof(BedrockProvider));
        var allTypes = assembly?.GetTypes().ToList();

        var derivedTypeNames = FindDerivedTypes(predefinedDir);

        var provider = new BedrockProvider();

        var failedTypes = new List<string>();
        var workingTypes = new Dictionary<string, double>();

        try
        {
            foreach (var t in derivedTypeNames)
            {
                var className = t;
                var type = allTypes.Where(x => x.Name == className)
                    .FirstOrDefault();

                if (type.FullName.ToLower().Contains("embed") ||
                    type.FullName.ToLower().Contains("image") ||
                    type.FullName.ToLower().Contains("stablediffusion")
                   )
                {
                    failedTypes.Add(className);
                    continue;
                }

                Console.WriteLine($"############## {type.FullName}");

                object[] args = { provider };
                var llm = (ChatModel)Activator.CreateInstance(type, args)!;
                var result = llm.GenerateAsync("who's your favor superhero?");

                workingTypes.Add(className, result.Result.Usage.Time.TotalSeconds);

                Console.WriteLine(result.Result + "\n\n\n");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****        ****        **** ERROR: " + e);
        }

        Console.WriteLine("FAILED");
        for (var i = 0; i < failedTypes.Count; i++)
        {
            Console.WriteLine($"{i}. {failedTypes[i]}");
        }

        Console.WriteLine("\nWORKING");
        using var workingTypesEnum = workingTypes.GetEnumerator();
        workingTypesEnum.MoveNext();
        for (var i = 0; i < workingTypes.Count; i++)
        {
            Console.WriteLine($"{i}. {workingTypesEnum.Current.Key}\t\t{workingTypesEnum.Current.Value}");
            workingTypesEnum.MoveNext();
        }

        return Task.CompletedTask;
    }

    public static List<string> FindDerivedTypes(string folderPath)
    {
        var listTypes = new List<string>();

        var csFiles = Directory.EnumerateFiles(folderPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in csFiles)
        {
            var code = File.ReadAllText(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var classNodes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            listTypes.AddRange(classNodes.Select(classNode => classNode.Identifier.Text));
        }

        return listTypes;
    }
}