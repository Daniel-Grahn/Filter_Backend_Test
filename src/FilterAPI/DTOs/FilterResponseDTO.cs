namespace FilterAPI.DTOs
{
    public record FilterResponseDTO
    (
        string SourceId,
        int UserId,
        string FieldName,
        string Data
    );
}
