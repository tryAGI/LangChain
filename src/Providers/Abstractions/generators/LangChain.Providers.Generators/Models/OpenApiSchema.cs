namespace LangChain.Providers.Generators;

public readonly record struct OpenApiSchema(
    string Name,
    string Description,
    string Type,
    string SchemaType,
    string? Format,
    IReadOnlyCollection<string> EnumValues,
    IReadOnlyCollection<OpenApiSchema> Properties,
    IReadOnlyCollection<OpenApiSchema> ArrayItem,
    bool IsRequired,
    bool IsNullable,
    string DefaultValue);