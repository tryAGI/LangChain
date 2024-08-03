namespace LangChain.Providers.Generators.SnapshotTests;

[TestClass]
public class IpcGeneratorSnapshotTests : VerifyBase
{
    [TestMethod]
    public Task GeneratesCorrectly()
    {
        return this.CheckSourceAsync(H.Resources.WeatherFunctions_cs.AsString());
    }
    
    [TestMethod]
    public Task VariousTypes()
    {
        return this.CheckSourceAsync(H.Resources.VariousTypesFunctions_cs.AsString());
    }
}