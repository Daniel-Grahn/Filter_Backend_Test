namespace FilterAPI.DTOs
{
    public record StoredFilterResponseDTO
    (
       string Title,
       int CompanyId,
       int UserId,
       bool IsPersonal,
       string SourceId,
       string Filters
    );
}

