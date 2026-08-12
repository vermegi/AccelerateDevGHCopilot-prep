using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;

    public BookService(IBookRepository bookRepository, ILoanRepository loanRepository)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
    }

    public async Task<BookAvailabilityResult?> CheckAvailability(int bookId)
    {
        Book? book = await _bookRepository.GetBook(bookId);
        if (book == null)
        {
            return null;
        }

        List<BookItem> bookItems = await _bookRepository.GetBookItemsByBookId(bookId);

        List<BookItemAvailability> bookItemAvailabilities = new List<BookItemAvailability>();
        foreach (BookItem bookItem in bookItems)
        {
            Loan? activeLoan = await _loanRepository.GetActiveLoanByBookItemId(bookItem.Id);
            bookItemAvailabilities.Add(new BookItemAvailability
            {
                BookItem = bookItem,
                IsAvailable = activeLoan == null,
                DueDate = activeLoan?.DueDate
            });
        }

        return new BookAvailabilityResult
        {
            Book = book,
            BookItemAvailabilities = bookItemAvailabilities
        };
    }
}
