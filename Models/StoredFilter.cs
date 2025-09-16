using filter_api_test;
using filter_api_test.DTOs;

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

    public void Update(StoredFilterRequestDTO request)
    {
        if (request == null) return;
        bool isUpdated = false;

        if (request.Title != null)
        {
            Title = request.Title;
            isUpdated = true;
        }
        if (request.Filters != null)
        {
            Filters = request.Filters;
            isUpdated = true;
        }

        if (request.IsPersonal != null)
        {
            IsPersonal = request.IsPersonal.Value;
            isUpdated = true;
        }
        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

