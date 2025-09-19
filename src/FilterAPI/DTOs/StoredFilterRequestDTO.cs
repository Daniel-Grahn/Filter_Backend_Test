namespace FilterAPI.DTOs
{
    public record StoredFilterRequestDTO
    {
        public string? Title { get; init; }
        public int CompanyId { get; init; }
        public int? UserId { get; init; }
        public bool? IsPersonal { get; init; }
        public required string SourceId { get; init; }
        public string? Filters { get; init; }
    }
}

