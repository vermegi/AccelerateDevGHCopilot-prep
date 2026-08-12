using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;

    public BookService(IBookRepository bookRepository, ILoanRepository loanRepository)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
    }

    public Task<List<Book>> SearchBooks(string searchInput)
    {
        return _bookRepository.SearchBooks(searchInput);
    }

    public async Task<bool?> IsBookAvailable(int bookId)
    {
        if (await _bookRepository.GetBook(bookId) == null)
        {
            return null;
        }

        List<BookItem> bookItems = await _bookRepository.GetBookItems(bookId);
        if (bookItems.Count == 0)
        {
            return false;
        }

        List<int> bookItemIds = bookItems.Select(bookItem => bookItem.Id).ToList();
        List<Loan> loans = await _loanRepository.GetLoansForBookItems(bookItemIds);
        HashSet<int> unavailableBookItemIds = loans
            .Where(loan => loan.ReturnDate == null)
            .Select(loan => loan.BookItemId)
            .ToHashSet();

        return bookItemIds.Any(bookItemId => !unavailableBookItemIds.Contains(bookItemId));
    }
}