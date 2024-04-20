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
langchain auth openai OPENAI_API_KEY # Default model - gpt-3.5-turbo, you can specify another model using --model parameter
langchain summarize --input-file README.md --output-file SUMMARY.md
langchain generate --input "Give me random word" # It will output random word to console

# Smart task
langchain model gpt-4-turbo
langchain generate --input "Give me solution for these problem: $PROBLEM"
```