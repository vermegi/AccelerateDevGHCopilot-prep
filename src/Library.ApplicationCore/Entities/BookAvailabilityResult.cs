namespace Library.ApplicationCore.Entities;

public class BookAvailabilityResult
{
    public required Book Book { get; set; }
    public required List<BookItemAvailability> BookItemAvailabilities { get; set; }

    public bool IsAvailable => BookItemAvailabilities.Any(b => b.IsAvailable);
    public int AvailableCount => BookItemAvailabilities.Count(b => b.IsAvailable);
    public int TotalCount => BookItemAvailabilities.Count;
}
