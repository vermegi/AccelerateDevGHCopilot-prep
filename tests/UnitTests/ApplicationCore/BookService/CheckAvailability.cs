using NSubstitute;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.UnitTests.ApplicationCore.BookServiceTests;

public class CheckAvailabilityTest
{
    private readonly IBookRepository _mockBookRepository;
    private readonly ILoanRepository _mockLoanRepository;
    private readonly BookService _bookService;

    public CheckAvailabilityTest()
    {
        _mockBookRepository = Substitute.For<IBookRepository>();
        _mockLoanRepository = Substitute.For<ILoanRepository>();
        _bookService = new BookService(_mockBookRepository, _mockLoanRepository);
    }

    [Fact(DisplayName = "BookService.CheckAvailability: Returns null if book is not found")]
    public async Task CheckAvailability_ReturnsNullIfBookNotFound()
    {
        // Arrange
        var bookId = 1;
        _mockBookRepository.GetBook(bookId).Returns((Book?)null);

        // Act
        var result = await _bookService.CheckAvailability(bookId);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "BookService.CheckAvailability: Returns available when all copies have no active loan")]
    public async Task CheckAvailability_ReturnsAvailableWhenAllCopiesFree()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItemsByBookId(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem1.Id).Returns((Loan?)null);
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem2.Id).Returns((Loan?)null);

        // Act
        var result = await _bookService.CheckAvailability(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result!.IsAvailable);
        Assert.Equal(2, result.AvailableCount);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.BookItemAvailabilities, a => Assert.True(a.IsAvailable));
    }

    [Fact(DisplayName = "BookService.CheckAvailability: Returns not available when all copies are on loan")]
    public async Task CheckAvailability_ReturnsNotAvailableWhenAllCopiesOnLoan()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var loan1 = new Loan { Id = 1, BookItemId = bookItem1.Id, PatronId = 1, DueDate = DateTime.Now.AddDays(5) };
        var loan2 = new Loan { Id = 2, BookItemId = bookItem2.Id, PatronId = 2, DueDate = DateTime.Now.AddDays(3) };
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItemsByBookId(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem1.Id).Returns(loan1);
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem2.Id).Returns(loan2);

        // Act
        var result = await _bookService.CheckAvailability(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.False(result!.IsAvailable);
        Assert.Equal(0, result.AvailableCount);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact(DisplayName = "BookService.CheckAvailability: Returns available when at least one copy is free (mixed)")]
    public async Task CheckAvailability_ReturnsAvailableWhenMixedCopies()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        var bookItem1 = BookFactory.CreateBookItem(book);
        var bookItem2 = BookFactory.CreateBookItem(book);
        var loan1 = new Loan { Id = 1, BookItemId = bookItem1.Id, PatronId = 1, DueDate = DateTime.Now.AddDays(5) };
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItemsByBookId(book.Id).Returns(new List<BookItem> { bookItem1, bookItem2 });
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem1.Id).Returns(loan1);
        _mockLoanRepository.GetActiveLoanByBookItemId(bookItem2.Id).Returns((Loan?)null);

        // Act
        var result = await _bookService.CheckAvailability(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result!.IsAvailable);
        Assert.Equal(1, result.AvailableCount);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact(DisplayName = "BookService.CheckAvailability: Returns not available when book has zero copies")]
    public async Task CheckAvailability_ReturnsNotAvailableWhenNoCopies()
    {
        // Arrange
        var book = BookFactory.CreateBook();
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItemsByBookId(book.Id).Returns(new List<BookItem>());

        // Act
        var result = await _bookService.CheckAvailability(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.False(result!.IsAvailable);
        Assert.Equal(0, result.TotalCount);
    }
}
