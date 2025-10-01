namespace FilterAPI.DTOs
{
    public record FilterCompositionRequestDTO
    (
        int Id,
        string Title,
        string SourceId,
        string[] FieldNames
     );
}