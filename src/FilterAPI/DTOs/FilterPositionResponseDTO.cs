namespace FilterAPI.DTOs
{
    public record FilterPositionResponseDTO
    (
        int Id,         // If functionality to save compositions is added
        string Title,   // If functionality to save compositions is added
        string SourceId,
        string[] FieldNames
     );
}