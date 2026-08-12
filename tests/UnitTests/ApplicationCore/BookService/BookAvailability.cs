using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Services;
using NSubstitute;

namespace Library.UnitTests.ApplicationCore.BookServiceTests;

public class BookAvailabilityTest
{
    private readonly IBookRepository _mockBookRepository;
    private readonly ILoanRepository _mockLoanRepository;
    private readonly BookService _bookService;

    public BookAvailabilityTest()
    {
        _mockBookRepository = Substitute.For<IBookRepository>();
        _mockLoanRepository = Substitute.For<ILoanRepository>();
        _bookService = new BookService(_mockBookRepository, _mockLoanRepository);
    }

    [Fact(DisplayName = "BookService.IsBookAvailable: Returns true when a copy is not loaned")]
    public async Task IsBookAvailable_ReturnsTrueWhenCopyIsNotLoaned()
    {
        var book = CreateBook(1);
        var bookItem = CreateBookItem(10, book.Id);
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItems(book.Id).Returns(new List<BookItem> { bookItem });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>()).Returns(new List<Loan>());

        bool? available = await _bookService.IsBookAvailable(book.Id);

        Assert.True(available);
    }

    [Fact(DisplayName = "BookService.IsBookAvailable: Returns false when all copies have active loans")]
    public async Task IsBookAvailable_ReturnsFalseWhenAllCopiesHaveActiveLoans()
    {
        var book = CreateBook(1);
        var bookItems = new List<BookItem> { CreateBookItem(10, book.Id), CreateBookItem(11, book.Id) };
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItems(book.Id).Returns(bookItems);
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>()).Returns(bookItems.Select(bookItem => new Loan
        {
            BookItemId = bookItem.Id,
            ReturnDate = null,
            DueDate = DateTime.Now.AddDays(-1)
        }).ToList());

        bool? available = await _bookService.IsBookAvailable(book.Id);

        Assert.False(available);
    }

    [Fact(DisplayName = "BookService.IsBookAvailable: Returns true when a copy has been returned")]
    public async Task IsBookAvailable_ReturnsTrueWhenCopyHasBeenReturned()
    {
        var book = CreateBook(1);
        var bookItem = CreateBookItem(10, book.Id);
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItems(book.Id).Returns(new List<BookItem> { bookItem });
        _mockLoanRepository.GetLoansForBookItems(Arg.Any<IEnumerable<int>>()).Returns(new List<Loan>
        {
            new Loan { BookItemId = bookItem.Id, ReturnDate = DateTime.Now }
        });

        bool? available = await _bookService.IsBookAvailable(book.Id);

        Assert.True(available);
    }

    [Fact(DisplayName = "BookService.IsBookAvailable: Returns false when book has no copies")]
    public async Task IsBookAvailable_ReturnsFalseWhenBookHasNoCopies()
    {
        var book = CreateBook(1);
        _mockBookRepository.GetBook(book.Id).Returns(book);
        _mockBookRepository.GetBookItems(book.Id).Returns(new List<BookItem>());

        bool? available = await _bookService.IsBookAvailable(book.Id);

        Assert.False(available);
    }

    [Fact(DisplayName = "BookService.IsBookAvailable: Returns null when book is not found")]
    public async Task IsBookAvailable_ReturnsNullWhenBookIsNotFound()
    {
        _mockBookRepository.GetBook(1).Returns((Book?)null);

        bool? available = await _bookService.IsBookAvailable(1);

        Assert.Null(available);
    }

    private static Book CreateBook(int id)
    {
        return new Book
        {
            Id = id,
            Title = "Test book",
            Genre = "Test",
            ISBN = "978-0000000000",
            ImageName = "test.jpg"
        };
    }

    private static BookItem CreateBookItem(int id, int bookId)
    {
        return new BookItem
        {
            Id = id,
            BookId = bookId,
            AcquisitionDate = DateTime.Now
        };
    }
}
