# LangChain
C# implementation of LangChain. We try to be as close to the original as possible in terms of abstractions, but are open to new entities.

While the [SemanticKernel](https://github.com/microsoft/semantic-kernel/) is good and we will use it wherever possible, we believe that it has many limitations and is aimed primarily at solving Microsoft problems.
We proceed from the position of the maximum choice of available options and are open to using third-party libraries within individual implementations.

### Usage
```csharp
var model = new OpenAiModel("API_KEY");
var response = await model.GenerateAsync("Hello, World of AI!");
```

## Support

Priority place for bugs: https://github.com/tryAGI/LangChain/issues  
Priority place for ideas and general questions: https://github.com/tryAGI/LangChain/discussions  
Discord: https://discord.gg/Ca2xhfBf3v  