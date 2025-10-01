namespace FilterAPI.DTOs
{
    public record FilterCompositionResponseDTO
    (
        int Id,         // If functionality to save compositions is added
        string Title,   // If functionality to save compositions is added
        string SourceId,
        string[] FieldNames
     );
}