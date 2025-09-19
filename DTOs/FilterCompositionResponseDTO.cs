namespace FilterAPI.DTOs
{
    public record FilterCompositionResponseDTO
    (
        int Id,
        int CompanyId,
        string SourceId,
        string CompositionName,
        string Description
     );
}