namespace FilterAPI.DTOs
{
    public record FilterRequestDTO {

        public required string SourceId { get; init; }
        public int UserId { get; init; }
        public required string FieldName { get; init; }
        public string[]? Data { get; init; }
    }
}
