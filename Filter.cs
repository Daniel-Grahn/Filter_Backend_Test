using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace filter_api_test
{
    public class Filter
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string SourceId { get; set; } // oklart namn, vilken del????? som vill ha ett specifikt filter
        public required string FieldName { get; set; }
        public string[]? Data { get; set; }
    }
}
