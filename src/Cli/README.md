# LangChain CLI

This is a console utility that will help you use our library for such tasks:
- Summarize text
- Generate release notes
- Generate changelog
- Generate documentation
- Generate code snippets
- Generate code samples
- Generate code documentation

## Usage:
```
dotnet tool install -g langchain.cli
langchain auth openai OPENAI_API_KEY
langchain summarize --input README.md --output SUMMARY.md
```