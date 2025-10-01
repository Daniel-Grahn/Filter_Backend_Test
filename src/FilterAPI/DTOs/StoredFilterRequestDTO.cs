namespace FilterAPI.DTOs
{
    public record StoredFilterRequestDTO
    {
        public int Id { get; init; }
        public string? Title { get; init; }
        public bool? IsPersonal { get; init; }
        public required string SourceId { get; init; }
        public string? Filters { get; init; }
    }
}

