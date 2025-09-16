namespace filter_api_test.DTOs
{
    public record FilterResponseDTO
    (
        string SourceId,
        int UserId,
        string FieldName,
        string Data
    );
}
