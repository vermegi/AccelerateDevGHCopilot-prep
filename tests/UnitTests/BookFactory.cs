using Library.ApplicationCore.Entities;

public static class BookFactory
{
    public static int bookId = 900;
    public static int bookItemId = 9000;

    public static Book CreateBook()
    {
        return new Book
        {
            Id = bookId++,
            Title = "The Great Gatsby",
            AuthorId = 1,
            Genre = "Classic",
            ImageName = "cover.jpg",
            ISBN = "978-0-00-000000-0",
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
