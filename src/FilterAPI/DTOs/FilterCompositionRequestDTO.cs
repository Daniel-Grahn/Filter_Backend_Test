namespace FilterAPI.DTOs
{
    public record FilterCompositionRequestDTO
    (
        int Id,
        string SourceId,
        string CompositionName,
        string Description
     );
}