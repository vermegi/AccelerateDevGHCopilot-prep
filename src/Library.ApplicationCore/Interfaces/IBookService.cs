using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IBookService
{
    Task<List<BookAvailability>> SearchBookAvailability(string searchInput);
}
