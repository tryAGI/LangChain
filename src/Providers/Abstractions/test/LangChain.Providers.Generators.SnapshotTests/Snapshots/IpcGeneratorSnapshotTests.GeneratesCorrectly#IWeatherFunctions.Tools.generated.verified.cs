//HintName: IWeatherFunctions.Tools.generated.cs

#nullable enable

namespace LangChain.Providers.IntegrationTests
{
    public static partial class WeatherFunctionsExtensions
    {
        public static global::System.Collections.Generic.IList<global::LangChain.Providers.OpenApiSchema> AsTools(this IWeatherFunctions functions)
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
                            ["location"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Description = "The city and state, e.g. San Francisco, CA",
                            },
                            ["unit"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Description = "",
                                Enum = new string[] { "celsius", "fahrenheit" },
                            }
                        },
                        Required = new string[] { "location" },
                    },
                },

                new global::LangChain.Providers.OpenApiSchema
                {
                    Type = "GetCurrentWeatherAsync",
                    Description = "Get the current weather in a given location",
                    Items = new global::LangChain.Providers.OpenApiSchema
                    {
                        Type = "object",
                        Description = "Get the current weather in a given location",
                        Properties = new global::System.Collections.Generic.Dictionary<string, global::LangChain.Providers.OpenApiSchema>
                        {
                            ["location"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Description = "The city and state, e.g. San Francisco, CA",
                            },
                            ["unit"] = new global::LangChain.Providers.OpenApiSchema
                            {
                                Type = "string",
                                Description = "",
                                Enum = new string[] { "celsius", "fahrenheit" },
                            }
                        },
                        Required = new string[] { "location" },
                    },
                },
            };
        }
    }
}