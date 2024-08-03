using System;
using System.ComponentModel;

namespace LangChain.Providers.IntegrationTests;

[LangChainTools]
public interface IVariousTypesFunctions
{
    [Description("Get the current weather in a given location")]
    public bool GetCurrentWeather(
        long parameter1,
        int parameter2,
        double parameter3,
        float parameter4,
        bool parameter5,
        DateTime dateTime,
        DateOnly date);
}

public class VariousTypesService : IVariousTypesFunctions
{
    public bool GetCurrentWeather(
        long parameter1,
        int parameter2,
        double parameter3,
        float parameter4,
        bool parameter5,
        DateTime dateTime,
        DateOnly date)
    {
        return true;
    }
}