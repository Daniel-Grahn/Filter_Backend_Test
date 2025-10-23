namespace FilterAPI.DTOs
{
    public record DateRangeResponseDTO
    {
        public DateOnly? DateStart { get; init; }
        public DateOnly? DateEnd { get; init; }
    }
}