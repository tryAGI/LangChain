namespace H.Generators;

public readonly record struct InterfaceData(
    string Namespace,
    string Name,
    IReadOnlyCollection<MethodData> Methods);