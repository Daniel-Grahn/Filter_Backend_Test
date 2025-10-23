namespace FilterAPI.DTOs
{
    public record DateRangeRequestDTO
    {

        public required int UserId { get; init; }
        public DateOnly? DateStart { get; init; }
        public DateOnly? DateEnd { get; init; }
    }
}