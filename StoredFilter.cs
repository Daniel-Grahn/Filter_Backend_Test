using filter_api_test;

public class StoredFilter
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int CompanyId { get; set; }
    public int? UserId { get; set; }

    public bool IsPersonal { get; set; } // no? yes? good? bad?
    public required string SourceId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Filters { get; set; }
}

public class StoredFilterInput
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int CompanyId { get; set; }
    public int? UserId { get; set; }

    public bool IsPersonal { get; set; } // no? yes? good? bad?
    public required string SourceId { get; set; }
    public string? Filters { get; set; }
}