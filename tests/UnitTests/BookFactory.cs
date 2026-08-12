using Library.ApplicationCore.Entities;

public static class BookFactory
{
    public static int bookId = 1001;
    public static int bookItemId = 2001;

    public static Book CreateBook()
    {
        return new Book
        {
            Id = bookId++,
            Title = "The Great Gatsby",
            AuthorId = 1,
            Genre = "Fiction",
            ImageName = "gatsby.jpg",
            ISBN = "9780743273565",
            Author = new Author { Id = 1, Name = "F. Scott Fitzgerald" }
        };
    }

    public static BookItem CreateBookItem(Book book)
    {
        return new BookItem
        {
            Id = bookItemId++,
            BookId = book.Id,
            AcquisitionDate = DateTime.Now.AddYears(-1),
            Condition = "Good",
            Book = book
        };
    }
}
