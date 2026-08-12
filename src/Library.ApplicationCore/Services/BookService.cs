using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;

    public BookService(IBookRepository bookRepository, ILoanRepository loanRepository)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
    }

    public async Task<List<BookAvailability>> SearchBookAvailability(string searchInput)
    {
        List<Book> matchingBooks = await _bookRepository.SearchBooks(searchInput);

        List<BookAvailability> results = new List<BookAvailability>();
        foreach (Book book in matchingBooks)
        {
            List<BookItem> bookItems = await _bookRepository.GetBookItemsForBook(book.Id);
            int totalCopies = bookItems.Count;

            int activeCopyCount = 0;
            if (totalCopies > 0)
            {
                List<int> bookItemIds = bookItems.Select(bi => bi.Id).ToList();
                List<Loan> loans = await _loanRepository.GetLoansForBookItems(bookItemIds);

                activeCopyCount = loans
                    .Where(l => l.ReturnDate == null)
                    .Select(l => l.BookItemId)
                    .Distinct()
                    .Count();
            }

            results.Add(new BookAvailability
            {
                Book = book,
                TotalCopies = totalCopies,
                AvailableCopies = totalCopies - activeCopyCount
            });
        }

        return results;
    }
}
