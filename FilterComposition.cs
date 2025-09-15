namespace filter_api_test
{
    public class FilterComposition
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CompanyId { get; set; }
        public int? UserId { get; set; }
        public required string SourceId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 
        public string[]? FieldNames { get; set; }
    }
}
