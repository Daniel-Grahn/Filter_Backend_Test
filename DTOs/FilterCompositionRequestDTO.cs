namespace filter_api_test.DTOs
{
    public record FilterCompositionRequestDTO
    (
        int Id,
        int CompanyId,
        string SourceId,
        string CompositionName,
        string Description
     );
}