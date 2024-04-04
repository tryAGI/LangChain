using LangChain.Providers.OpenRouter.CodeGenerator.Main;

var gc = new OpenRouterCodeGenerator();
await gc.GenerateCodesAsync(true).ConfigureAwait(false);