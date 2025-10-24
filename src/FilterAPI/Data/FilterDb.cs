using Microsoft.EntityFrameworkCore;
using FilterAPI.Models;

namespace FilterAPI.Data
{
    public class FilterDb : DbContext
    {
        public FilterDb(DbContextOptions<FilterDb> options) : base(options) { }

        //public DbSet<Filter> Filters => Set<Filter>();
        public DbSet<Filter> Filter { get; set; } = null!;
        public DbSet<StoredFilter> StoredFilter { get; set; } = null!;
        public DbSet<FilterPosition> FilterPosition { get; set; } = null!;
        public DbSet<DateRange> DateRange { get; set; } = null!;

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder
                .Entity<Filter>()
                .HasKey(f => new { f.SourceId, f.UserId, f.FieldName });

            modelBuilder
                .Entity<Filter>()
                .HasAlternateKey(f => f.Id);

            modelBuilder
                .Entity<Filter>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<DateRange>()
                .HasKey(f => new { f.UserId, f.SourceId });

            modelBuilder
                .Entity<DateRange>()
                .HasAlternateKey(f => f.Id);

            modelBuilder
                .Entity<DateRange>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
