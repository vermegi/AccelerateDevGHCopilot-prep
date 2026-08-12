using NSubstitute;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.UnitTests.ApplicationCore.BookServiceTests;

public class SearchBookAvailabilityTest
{
    private readonly IBookRepository _mockBookRepository;
    private readonly ILoanRepository _mockLoanRepository;
    private readonly BookService _bookService;

    public SearchBookAvailabilityTest()
    {
        _mockBookRepository = Substitute.For<IBookRepository>();
        _mockLoanRepository = Substitute.For<ILoanRepository>();
        _bookService = new BookService(_mockBookRepository, _mockLoanRepository);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Returns an empty list when no titles match")]
    public async Task SearchBookAvailability_ReturnsEmptyList_WhenNoTitleMatches()
    {
        // Arrange
        var searchInput = "Nonexistent";
        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book>());

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        Assert.Empty(results);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Returns all copies as available when there are no loans")]
    public async Task SearchBookAvailability_ReturnsAllCopiesAvailable_WhenNoLoans()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>()).Returns(new List<Loan>());

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(2, result.TotalCopies);
        Assert.Equal(2, result.AvailableCopies);
        Assert.True(result.IsAvailable);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Reduces available copies when some copies are on active loan")]
    public async Task SearchBookAvailability_ReducesAvailableCopies_WhenSomeCopiesLoaned()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>())
            .Returns(new List<Loan> { LoanFactory.CreateActiveLoanForBookItem(bookItem1.Id) });

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(2, result.TotalCopies);
        Assert.Equal(1, result.AvailableCopies);
        Assert.True(result.IsAvailable);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Returns zero available copies when all copies are on active loan")]
    public async Task SearchBookAvailability_ReturnsZeroAvailable_WhenAllCopiesLoaned()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>())
            .Returns(new List<Loan>
            {
                LoanFactory.CreateActiveLoanForBookItem(bookItem1.Id),
                LoanFactory.CreateActiveLoanForBookItem(bookItem2.Id)
            });

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(2, result.TotalCopies);
        Assert.Equal(0, result.AvailableCopies);
        Assert.False(result.IsAvailable);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Returned loans do not reduce availability")]
    public async Task SearchBookAvailability_ReturnedLoansDoNotReduceAvailability()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem> { bookItem1 });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>())
            .Returns(new List<Loan> { LoanFactory.CreateReturnedLoanForBookItem(bookItem1.Id) });

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(1, result.TotalCopies);
        Assert.Equal(1, result.AvailableCopies);
        Assert.True(result.IsAvailable);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: Multiple historical loans for one copy count it only once")]
    public async Task SearchBookAvailability_MultipleHistoricalLoansForOneCopy_CountedOnce()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>())
            .Returns(new List<Loan>
            {
                LoanFactory.CreateReturnedLoanForBookItem(bookItem1.Id),
                LoanFactory.CreateReturnedLoanForBookItem(bookItem1.Id),
                LoanFactory.CreateActiveLoanForBookItem(bookItem1.Id)
            });

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(2, result.TotalCopies);
        Assert.Equal(1, result.AvailableCopies);
    }

    [Fact(DisplayName = "BookService.SearchBookAvailability: A book with zero copies is returned as unavailable")]
    public async Task SearchBookAvailability_BookWithZeroCopies_ReturnedAsUnavailable()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var searchInput = "Gatsby";

        _mockBookRepository.SearchBooks(searchInput).Returns(new List<Book> { book });
        _mockBookRepository.GetBookItemsForBook(book.Id).Returns(new List<BookItem>());

        // Act
        var results = await _bookService.SearchBookAvailability(searchInput);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal(0, result.TotalCopies);
        Assert.Equal(0, result.AvailableCopies);
        Assert.False(result.IsAvailable);
        await _mockLoanRepository.DidNotReceive().GetLoansForBookItems(Arg.Any<IEnumerable<int>>());
    }
}
