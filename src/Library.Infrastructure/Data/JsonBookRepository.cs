using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class JsonBookRepository : IBookRepository
{
    private readonly JsonData _jsonData;

    public JsonBookRepository(JsonData jsonData)
    {
        _jsonData = jsonData;
    }

    public async Task<List<Book>> SearchBooks(string searchInput)
    {
        await _jsonData.EnsureDataLoaded();

        List<Book> searchResults = new List<Book>();
        foreach (Book book in _jsonData.Books!)
        {
            if (book.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            {
                searchResults.Add(book);
            }
        }
        searchResults.Sort((b1, b2) => String.Compare(b1.Title, b2.Title, StringComparison.OrdinalIgnoreCase));

        List<Book> populated = new List<Book>();
        foreach (Book book in searchResults)
        {
            populated.Add(_jsonData.GetPopulatedBook(book));
        }

        return populated;
    }

    public async Task<List<BookItem>> GetBookItemsForBook(int bookId)
    {
        await _jsonData.EnsureDataLoaded();

        List<BookItem> bookItems = new List<BookItem>();
        foreach (BookItem bookItem in _jsonData.BookItems!)
        {
            if (bookItem.BookId == bookId)
            {
                bookItems.Add(_jsonData.GetPopulatedBookItem(bookItem));
            }
        }

        return bookItems;
    }
}
