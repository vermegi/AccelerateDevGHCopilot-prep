using Library.ApplicationCore.Entities;

public interface IBookService
{
    Task<BookAvailabilityResult?> CheckAvailability(int bookId);
}
