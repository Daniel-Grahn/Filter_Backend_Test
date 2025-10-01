using FilterAPI.Data;

namespace FilterAPI.Models
{
    public class FilterComposition
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CompanyId { get; set; }
        public int? UserId { get; set; }
        public required string SourceId { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public string[]? FieldNames { get; set; }

        public void Update(FilterComposition request)
        {
            if (request == null) return;
            bool isUpdated = false;


            if (FieldNames != request.FieldNames)
            {
                FieldNames = request.FieldNames;
                isUpdated = true;
            }

            if (isUpdated)
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
