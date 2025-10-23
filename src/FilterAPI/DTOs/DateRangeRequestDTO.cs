namespace FilterAPI.DTOs
{
    public record DateRangeRequestDTO
    {
        public DateOnly DateStart { get; init; }
        public DateOnly DateEnd { get; init; }
    }
}