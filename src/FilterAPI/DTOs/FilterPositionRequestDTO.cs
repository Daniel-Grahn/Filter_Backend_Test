namespace FilterAPI.DTOs
{
    public record FilterPositionRequestDTO
    (
        int Id,
        string Title,
        string SourceId,
        string[] FieldNames
     );
}