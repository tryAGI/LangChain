//HintName: IVariousTypesFunctions.Tools.generated.cs

#nullable enable

namespace LangChain.Providers.IntegrationTests
{
    public static partial class VariousTypesFunctionsExtensions
    {
        public static global::System.Collections.Generic.IList<global::LangChain.Providers.OpenApiSchema> AsTools(this IVariousTypesFunctions functions)
        {
            return new global::System.Collections.Generic.List<global::LangChain.Providers.OpenApiSchema>
            {
                new global::LangChain.Providers.OpenApiSchema
                {
                    Type = "GetCurrentWeather",
                    Description = "Get the current weather in a given location",
                    Items = new global::LangChain.Providers.OpenApiSchema
                    {
                        Type = "object",
                        Description = "Get the current weather in a given location",
                        Properties = new global::System.Collections.Generic.Dictionary<string, global::LangChain.Providers.OpenApiSchema>
                        {
                            ["parameter1"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64",
                                Description = "",
                            },
                            ["parameter2"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int32",
                                Description = "",
                            },
                            ["parameter3"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "number",
                                Format = "float",
                                Description = "",
                            },
                            ["parameter4"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "number",
                                Format = "double",
                                Description = "",
                            },
                            ["parameter5"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "boolean",
                                Description = "",
                            },
                            ["dateTime"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Format = "date-time",
                                Description = "",
                            },
                            ["date"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Format = "date",
                                Description = "",
                            }
                        },
                        Required = new string[] { "parameter1", "parameter2", "parameter3", "parameter4", "parameter5", "dateTime", "date" },
                    },
                },
            };
        }
    }
}