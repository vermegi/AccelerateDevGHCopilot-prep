namespace Library.ApplicationCore.Entities;

public class BookItemAvailability
{
    public required BookItem BookItem { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? DueDate { get; set; }
}
