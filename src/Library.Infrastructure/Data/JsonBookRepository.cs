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

    public async Task<Book?> GetBook(int bookId)
    {
        await _jsonData.EnsureDataLoaded();

        foreach (Book book in _jsonData.Books!)
        {
            if (book.Id == bookId)
            {
                return _jsonData.GetPopulatedBook(book);
            }
        }
        return null;
    }

    public async Task<List<Book>> SearchBooks(string searchInput)
    {
        await _jsonData.EnsureDataLoaded();

        List<Book> matches = new List<Book>();
        foreach (Book book in _jsonData.Books!)
        {
            if (book.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(_jsonData.GetPopulatedBook(book));
            }
        }
        return matches;
    }

    public async Task<List<BookItem>> GetBookItemsByBookId(int bookId)
    {
        await _jsonData.EnsureDataLoaded();

        List<BookItem> matches = new List<BookItem>();
        foreach (BookItem bookItem in _jsonData.BookItems!)
        {
            if (bookItem.BookId == bookId)
            {
                matches.Add(_jsonData.GetPopulatedBookItem(bookItem));
            }
        }
        return matches;
    }
}
