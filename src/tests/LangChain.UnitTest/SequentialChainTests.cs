using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains.Sequentials;
using LangChain.Schema;
using Moq;

namespace LangChain.UnitTest;

[TestClass]
public class SequentialChainTests
{
    [TestMethod]
    public async Task Sequential_Usage_Single_Inputs()
    {
        // Arrange
        var chain1 = CreateFakeChainMock(new[] { "foo" }, new[] { "bar" }).Object;
        var chain2 = CreateFakeChainMock(new[] { "bar" }, new[] { "baz" }).Object;
        var chain = new SequentialChain(new SequentialChainInput(new[] { chain1, chain2 }, new[] { "foo" }));

        // Act
        var outputs = await chain.CallAsync(new ChainValues("foo", "123"));

        // Assert
        outputs.Value.Count.Should().Be(1);
        outputs.Value.First().Key.Should().Be("baz");
        outputs.Value.First().Value.Should().Be("123foofoo");
    }

    [TestMethod]
    public async Task Sequential_Usage_Single_Inputs_With_ReturnAll()
    {
        // Arrange
        var chain1 = CreateFakeChainMock(new[] { "foo" }, new[] { "bar" }).Object;
        var chain2 = CreateFakeChainMock(new[] { "bar" }, new[] { "baz" }).Object;
        var chain = new SequentialChain(new SequentialChainInput(new[] { chain1, chain2 }, new[] { "foo" }, returnAll: true));

        // Act
        var outputs = await chain.CallAsync(new ChainValues("foo", "123"));

        // Assert
        outputs.Value.Count.Should().Be(2);
        outputs.Value.ContainsKey("bar").Should().BeTrue();
        outputs.Value.ContainsKey("baz").Should().BeTrue();
        outputs.Value["bar"].Should().Be("123foo");
        outputs.Value["baz"].Should().Be("123foofoo");
    }

    [TestMethod]
    public void Should_Throw_Exception_On_Empty_Chains()
    {
        // Arrange
        var input = new SequentialChainInput(Array.Empty<IChain>(), Array.Empty<string>());

        // Act
        var act = new Action(() => new SequentialChain(input));

        act.Should().Throw<ArgumentException>()
            .WithMessage("Sequential chain must have at least one chain.");
    }

    [TestMethod]
    public void Should_Throw_Exception_On_Wrongly_Specify_ReturnAll()
    {
        // Arrange
        var input = new SequentialChainInput(Array.Empty<IChain>(), Array.Empty<string>(), new string[] { "output1" }, true);

        // Act
        var act = new Action(() => new SequentialChain(input));

        act.Should().Throw<ArgumentException>()
            .WithMessage("Either specify variables to return using `outputVariables` or use `returnAll` param. Cannot apply both conditions at the same time.");
    }

    [TestMethod]
    public void Should_Throw_Exception_On_Duplicate_Output_Key()
    {
        // Arrange
        var chain1 = CreateFakeChainMock(new[] { "input1" }, new[] { "duplicateOutput" }).Object;
        var chain2 = CreateFakeChainMock(new[] { "duplicateOutput" }, new[] { "duplicateOutput" }).Object;

        // Act 
        Action act = () => new SequentialChain(new SequentialChainInput(new[] { chain1, chain2 }, new[] { "Some Input" }));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Duplicate output key `duplicateOutput`");
    }

    private Mock<IChain> CreateFakeChainMock(string[] inputVariables, string[] outputVariables)
    {
        var fakeChainMock = new Mock<IChain>();

        fakeChainMock.Setup(_ => _.InputKeys).Returns(inputVariables);
        fakeChainMock.Setup(_ => _.OutputKeys).Returns(outputVariables);
        fakeChainMock.Setup(x => x.CallAsync(It.IsAny<IChainValues>()))
            .Returns<IChainValues>(chainValues =>
            {
                var output = new ChainValues();

                foreach (var outputVariable in outputVariables)
                {
                    var text = string.Join("", inputVariables.Select(_ => chainValues.Value[_]).Select(_ => _ as string));
                    var outputValue = $"{text}foo";
                    output.Value.Add(outputVariable, outputValue);
                }

                return Task.FromResult<IChainValues>(output);
            });

        return fakeChainMock;
    }
}