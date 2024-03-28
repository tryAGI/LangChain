using System.Text.Json.Serialization;

namespace LangChain.Providers.Suno.Sdk;

[JsonSerializable(typeof(GenerateV2Request))]
[JsonSerializable(typeof(GenerateV2Response))]
[JsonSerializable(typeof(List<Clip>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;