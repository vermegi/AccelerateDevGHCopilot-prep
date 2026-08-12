namespace Library.ApplicationCore.Entities;

public class BookAvailability
{
    public required Book Book { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public bool IsAvailable => AvailableCopies > 0;
}
