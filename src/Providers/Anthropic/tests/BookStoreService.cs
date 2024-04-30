using LangChain.Providers.Anthropic.Tools;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace LangChain.Providers.Anthropic.Tests
{
    public class GetAuthorBook
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    [AnthropicTools]
    public interface IBookStoreService
    {
        [Description("Get books written by some author")]
        public Task<List<GetAuthorBook>> GetAuthorBooksAsync([Description("Author name")] string authorName, CancellationToken cancellationToken = default);

        [Description("Get book page content")]
        public Task<string> GetBookPageContentAsync([Description("Book Name")] string bookName, [Description("Book Page Number")] int bookPageNumber, CancellationToken cancellationToken = default);

    }
    public class BookStoreService : IBookStoreService
    {
        public Task<List<GetAuthorBook>> GetAuthorBooksAsync(string authorName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<GetAuthorBook>([
                new GetAuthorBook
                { Title = "Five point someone", Description = "This book is about 3 college friends" },
                new GetAuthorBook
                { Title = "Two States", Description = "This book is about intercast marriage in India" }
            ]));
        }

        public Task<string> GetBookPageContentAsync(string bookName, int bookPageNumber, CancellationToken cancellationToken = default)
        {
            return Task.FromResult("this is a cool weather out there, and I am stuck at home.");
        }
    }
}
