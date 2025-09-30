namespace FilterAPI.DTOs
{
    public record StoredFilterResponseDTO
    (
       int Id,
       string Title,
       bool IsPersonal,
       string SourceId,
       string Filters
    );
}

