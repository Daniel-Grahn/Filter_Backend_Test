using FilterAPI;
using FilterAPI.DTOs;

public class DateRange
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string SourceId { get; set; }
    public DateOnly DateStart { get; set; }
    public DateOnly DateEnd { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public void Update(DateRange request)
    {
        if (request == null) return;
        bool isUpdated = false;

        if (DateStart != request.DateStart)
        {
            DateStart = request.DateStart;
            isUpdated = true;
        }
        if (DateEnd != request.DateEnd)
        {
            DateEnd = request.DateEnd;
            isUpdated = true;
        }
        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

