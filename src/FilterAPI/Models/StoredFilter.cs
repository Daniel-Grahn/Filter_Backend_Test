using FilterAPI;
using FilterAPI.DTOs;

public class StoredFilter
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int CompanyId { get; set; }
    public int? UserId { get; set; }

    public bool IsPersonal { get; set; } // no? yes? good? bad?
    public required string SourceId { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? Filters { get; set; }

    public void Update(StoredFilter request)
    {
        if (request == null) return;
        bool isUpdated = false;

        if (Title != request.Title)
        {
            Title = request.Title;
            isUpdated = true;
        }
        if (Filters != request.Filters)
        {
            Filters = request.Filters;
            isUpdated = true;
        }

        if (IsPersonal != request.IsPersonal)
        {
            IsPersonal = request.IsPersonal;
            isUpdated = true;
        }

        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

