using LangChain.Providers.OpenRouter.CodeGenerator.Main;

var gc = new OpenRouterCodeGenerator();
await gc.GenerateCodesAsync(includeUnderScoresInEnum: true).ConfigureAwait(false);