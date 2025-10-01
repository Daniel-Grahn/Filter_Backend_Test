namespace FilterAPI.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string SourceId { get; set; } // oklart namn, vilken del????? som vill ha ett specifikt filter
        public required string FieldName { get; set; }
        public string[]? Data { get; set; }
    }
}
