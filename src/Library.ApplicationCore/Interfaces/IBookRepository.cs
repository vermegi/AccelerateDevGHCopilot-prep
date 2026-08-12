using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IBookRepository
{
    Task<Book?> GetBook(int bookId);
    Task<List<Book>> SearchBooks(string searchInput);
    Task<List<BookItem>> GetBookItems(int bookId);
}