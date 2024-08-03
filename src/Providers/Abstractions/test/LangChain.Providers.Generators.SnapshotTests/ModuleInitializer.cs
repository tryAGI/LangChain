using System.Runtime.CompilerServices;
using VerifyTests;

namespace LangChain.Providers.Generators.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySourceGenerators.Initialize();
    }
}