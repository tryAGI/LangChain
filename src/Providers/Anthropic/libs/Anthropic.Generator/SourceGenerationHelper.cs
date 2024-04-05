using H.Generators;
using H.Generators.Extensions;

namespace LangChain.Providers.Anthropic.Generators;

internal static class SourceGenerationHelper
{
    /// <summary>
    /// https://swagger.io/docs/specification/data-models/data-types/
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static string GenerateOpenApiSchema(ParameterData parameter, int depth = 0)
    {
        var indent = new string(' ', depth * 4);
        if (parameter.ArrayItem.Count != 0)
        {
            return $@"new
{indent}                    {{
{indent}                        type = ""{parameter.SchemaType}"",
{indent}                        description = ""{parameter.Description}"",
{indent}                        items = {GenerateOpenApiSchema(parameter.ArrayItem.First(), depth: depth + 1)},
{indent}                    }}";
        }
        if (parameter.Properties.Count != 0)
        {
            return $@"new
{indent}                    {{
{indent}                        type = ""{parameter.SchemaType}"",
{indent}                        description = ""{parameter.Description}"",
{indent}                        properties = new Dictionary<string, object>
{indent}                        {{
{indent}                            {string.Join(",\n                            " + indent, parameter.Properties.Select(x => $@"[""{x.Name}""] = " + GenerateOpenApiSchema(x, depth: depth + 2)))}
{indent}                        }},
{indent}                        required = new string[] {{ {string.Join(", ", parameter.Properties
                                    .Where(static x => x.IsRequired)
                                    .Select(static x => $"\"{x.Name}\""))} }},
{indent}                    }}";
        }
        
        if (parameter.EnumValues.Count != 0)
        {
            return $@"new
{indent}                    {{
{indent}                        type = ""{parameter.SchemaType}"",
{indent}                        description = ""{parameter.Description}"",
{indent}                        @enum = new string[] {{ {string.Join(", ", parameter.EnumValues.Select(static x => $"\"{x}\""))} }},
{indent}                    }}";
        }
        
        return $@"new
{indent}                    {{
{indent}                        type = ""{parameter.SchemaType}"",{(parameter.Format != null ? $@"
{indent}                        format = ""{parameter.Format}""," : "")}
{indent}                        description = ""{parameter.Description}"",
{indent}                    }}";
    }

    public static string GenerateClientImplementation(InterfaceData @interface)
    {
        var extensionsClassName = @interface.Name.Substring(startIndex: 1) + "AnthropicExtensions";
        
        return @$"
using System.Collections.Generic;

#nullable enable

namespace {@interface.Namespace}
{{
    public static class {extensionsClassName}
    {{
{@interface.Methods.Select(static method => $@"
        public class {method.Name}Args
        {{
            {string.Join("\n            ", method.Parameters.Select(static x => $@"public {x.Type}{(x.IsNullable ? "?" : "")} {x.Name.ToPropertyName()} {{ get; set; }}{(!string.IsNullOrEmpty(x.DefaultValue) ? $" = {x.DefaultValue};" : "")}"))}
        }}
").Inject()}

{@interface.Methods.Select(method => $@"
        public static (string Name, string Description, object Obj) {method.Name}AsParametersObject(this {@interface.Name} functions)
        {{
            return (""{method.Name}"", ""{method.Description}"", new
            {{
                type = ""object"",
                properties = new Dictionary<string, object>
                {{
                    {string.Join(",\n                    ", method.Parameters.Select(static parameter => $@"[""{parameter.Name}""] = " + GenerateOpenApiSchema(parameter)))}
                }},
                required = new string[] {{ {string.Join(", ", method.Parameters
                    .Where(static x => x.IsRequired)
                    .Select(static x => $"\"{x.Name}\""))} }},
            }});
        }}
").Inject()}

{@interface.Methods.Select(method => $@"
        public static (string Name, string Description, Dictionary<string, object> Dictionary) {method.Name}AsDictionary(this {@interface.Name} functions)
        {{
            return (""{method.Name}"", ""{method.Description}"", new Dictionary<string, object>
            {{
                [""type""] = ""object"",
                [""properties""] = new Dictionary<string, object>
                {{
                    {string.Join(",\n                    ", method.Parameters.Select(static parameter => $@"[""{parameter.Name}""] = " + GenerateOpenApiSchema(parameter)))}
                }},
                [""required""] = new string[] {{ {string.Join(", ", method.Parameters
                    .Where(static x => x.IsRequired)
                    .Select(static x => $"\"{x.Name}\""))} }},
            }});
        }}
").Inject()}

        public static global::System.Collections.Generic.ICollection<global::LangChain.Providers.Anthropic.Tools.AnthropicTool> AsAnthropicTools(this {@interface.Name} functions)
        {{
{@interface.Methods.Select((method, i) => $@"
            var function{i} = functions.{method.Name}AsDictionary();").Inject()}

            {@interface.Methods.Select((_,i)=>$@"                
            var methodParams{i} = new global::System.Collections.Generic.List<global::LangChain.Providers.Anthropic.Tools.AnthropicToolParameter>();").Inject()}

            {@interface.Methods.Select((_, i) => string.Join("\r\n", _.Parameters.Select(s => $"\r\n\t\t\tmethodParams{i}.Add(new global::LangChain.Providers.Anthropic.Tools.AnthropicToolParameter(){{Type = {"\"" + s.SchemaType + "\""}, Name = {"\""+s.Name+"\""}, Format = {"\"" + s.Format + "\""}, Description = " +"\""+s.Description+"\""+ $@"}});"))).Inject()}

            return new global::System.Collections.Generic.List<global::LangChain.Providers.Anthropic.Tools.AnthropicTool>
            {{
{@interface.Methods.Select((_, i) => $@"                
                new global::LangChain.Providers.Anthropic.Tools.AnthropicTool()
                {{
                    Name = function{i}.Name,
                    Description = function{i}.Description,
                    Parameters = methodParams{i}
                }},
").Inject()}
            }};
        }}

        public static global::System.Collections.Generic.IReadOnlyDictionary<string, global::System.Func<string, global::System.Threading.CancellationToken, global::System.Threading.Tasks.Task<string>>> AsAnthropicCalls(this {@interface.Name} service)
        {{
            return new global::System.Collections.Generic.Dictionary<string, global::System.Func<string, global::System.Threading.CancellationToken, global::System.Threading.Tasks.Task<string>>>
            {{
{@interface.Methods.Where(static x => x is { IsAsync: false, IsVoid: false }).Select(method => $@"
                [""{method.Name}""] = (json, _) =>
                {{
                    return global::System.Threading.Tasks.Task.FromResult(service.Call{method.Name}(json));
                }},
").Inject()}
{@interface.Methods.Where(static x => x is { IsAsync: false, IsVoid: true }).Select(method => $@"
                [""{method.Name}""] = (json, _) =>
                {{
                    service.Call{method.Name}(json);

                    return global::System.Threading.Tasks.Task.FromResult(string.Empty);
                }},
").Inject()}
{@interface.Methods.Where(static x => x is { IsAsync: true, IsVoid: false }).Select(method => $@"
                [""{method.Name}""] = async (json, cancellationToken) =>
                {{
                    return await service.Call{method.Name}(json, cancellationToken);
                }},
").Inject()}
{@interface.Methods.Where(static x => x is { IsAsync: true, IsVoid: true }).Select(method => $@"
                [""{method.Name}""] = async (json, cancellationToken) =>
                {{
                    await service.Call{method.Name}(json, cancellationToken);

                    return string.Empty;
                }},
").Inject()}
            }};
        }}

{@interface.Methods.Select(method => $@"
        public static object {method.Name}AsFunctionObject(this {@interface.Name} functions)
        {{
            var (name, description, parameters) = functions.{method.Name}AsParametersObject();

            return new
            {{
                name = name,
                description = description,
                parameters = parameters,
            }};
        }}
").Inject()}

{@interface.Methods.Select(method => $@"
        public static (string Name, string Description, global::System.Text.Json.Nodes.JsonNode Node) {method.Name}AsParametersJsonNode(this {@interface.Name} functions)
        {{
            var (name, description, parameters) = functions.{method.Name}AsParametersObject();
            var node =
                global::System.Text.Json.JsonSerializer.SerializeToNode(parameters) ??
                throw new global::System.InvalidOperationException(""Could not serialize parameters."");

            return (name, description, node);
        }}
").Inject()}

{@interface.Methods.Select(method => $@"
        public static {extensionsClassName}.{method.Name}Args As{method.Name}Args(
            this {@interface.Name} functions,
            string json)
        {{
            return
                global::System.Text.Json.JsonSerializer.Deserialize<{extensionsClassName}.{method.Name}Args>(json, new global::System.Text.Json.JsonSerializerOptions
                {{
                    PropertyNamingPolicy = global::System.Text.Json.JsonNamingPolicy.CamelCase,
Converters = {{{{ new global::System.Text.Json.Serialization.JsonStringEnumConverter(global::System.Text.Json.JsonNamingPolicy.CamelCase) }}}}
                }}) ??
                throw new global::System.InvalidOperationException(""Could not deserialize JSON."");
        }}
").Inject()}

{@interface.Methods.Where(static x => x is { IsAsync: false, IsVoid: false }).Select(method => $@"
        public static string Call{method.Name}(this {@interface.Name} functions, string json)
        {{
            var args = functions.As{method.Name}Args(json);
            var jsonResult = functions.{method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $@"args.{parameter.Name.ToPropertyName()}"))});

            return global::System.Text.Json.JsonSerializer.Serialize(jsonResult, new global::System.Text.Json.JsonSerializerOptions
            {{
                PropertyNamingPolicy = global::System.Text.Json.JsonNamingPolicy.CamelCase,
                Converters = {{ new global::System.Text.Json.Serialization.JsonStringEnumConverter(global::System.Text.Json.JsonNamingPolicy.CamelCase) }},
            }});
        }}
").Inject()}

{@interface.Methods.Where(static x => x is { IsAsync: false, IsVoid: true }).Select(method => $@"
        public static void Call{method.Name}(this {@interface.Name} functions, string json)
        {{
            var args = functions.As{method.Name}Args(json);
            functions.{method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $@"args.{parameter.Name.ToPropertyName()}"))});
        }}
").Inject()}

{@interface.Methods.Where(static x => x is { IsAsync: true, IsVoid: false }).Select(method => $@"
        public static async global::System.Threading.Tasks.Task<string> Call{method.Name}(
            this {@interface.Name} functions,
            string json,
            global::System.Threading.CancellationToken cancellationToken = default)
        {{
            var args = functions.As{method.Name}Args(json);
            var jsonResult = await functions.{method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $@"args.{parameter.Name.ToPropertyName()}"))}, cancellationToken);

            return global::System.Text.Json.JsonSerializer.Serialize(jsonResult, new global::System.Text.Json.JsonSerializerOptions
            {{
                PropertyNamingPolicy = global::System.Text.Json.JsonNamingPolicy.CamelCase,
                Converters = {{ new global::System.Text.Json.Serialization.JsonStringEnumConverter(global::System.Text.Json.JsonNamingPolicy.CamelCase) }},
            }});
        }}
").Inject()}

{@interface.Methods.Where(static x => x is { IsAsync: true, IsVoid: true }).Select(method => $@"
        public static async global::System.Threading.Tasks.Task<string> Call{method.Name}(
            this {@interface.Name} functions,
            string json,
            global::System.Threading.CancellationToken cancellationToken = default)
        {{
            var args = functions.As{method.Name}Args(json);
            await functions.{method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $@"args.{parameter.Name.ToPropertyName()}"))}, cancellationToken);

            return string.Empty;
        }}
").Inject()}

        public static async global::System.Threading.Tasks.Task<string> CallAsync(
            this {@interface.Name} service,
            string functionName,
            string argumentsAsJson,
            global::System.Threading.CancellationToken cancellationToken = default)
        {{
            var calls = service.AsAnthropicCalls();
            var func = calls[functionName];

            return await func(argumentsAsJson, cancellationToken);
        }}
    }}
}}";
    }
}
