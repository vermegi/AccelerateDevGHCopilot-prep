using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IBookService
{
    Task<List<Book>> SearchBooks(string searchInput);
    Task<bool?> IsBookAvailable(int bookId);
}