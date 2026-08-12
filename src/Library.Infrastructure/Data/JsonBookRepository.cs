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
        Book? book = _jsonData.Books!.FirstOrDefault(book => book.Id == bookId);
        return book == null ? null : _jsonData.GetPopulatedBook(book);
    }

    public async Task<List<Book>> SearchBooks(string searchInput)
    {
        await _jsonData.EnsureDataLoaded();

        string normalizedInput = searchInput.Trim();
        bool hasId = int.TryParse(normalizedInput, out int bookId);

        return _jsonData.Books!
            .Where(book =>
                (hasId && book.Id == bookId) ||
                book.Title.Contains(normalizedInput, StringComparison.OrdinalIgnoreCase) ||
                book.ISBN.Contains(normalizedInput, StringComparison.OrdinalIgnoreCase))
            .OrderBy(book => book.Title)
            .Select(_jsonData.GetPopulatedBook)
            .ToList();
    }

    public async Task<List<BookItem>> GetBookItems(int bookId)
    {
        await _jsonData.EnsureDataLoaded();

        return _jsonData.BookItems!
            .Where(bookItem => bookItem.BookId == bookId)
            .Select(_jsonData.GetPopulatedBookItem)
            .ToList();
    }
}