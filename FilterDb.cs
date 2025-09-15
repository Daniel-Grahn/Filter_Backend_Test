using Microsoft.EntityFrameworkCore;

namespace filter_api_test
{
    public class FilterDb : DbContext
    {
        public FilterDb(DbContextOptions<FilterDb> options) : base(options) { }

        //public DbSet<Filter> Filters => Set<Filter>();
        public DbSet<Filter> Filter { get; set; }
        public DbSet<StoredFilter> StoredFilter { get; set; }
        public DbSet<FilterComposition> FilterComposition { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Filter>()
                .HasKey(f => new { f.SourceId, f.UserId, f.FieldName });
            modelBuilder.Entity<Filter>().HasAlternateKey(f => f.Id);
            modelBuilder.Entity<Filter>().Property(f => f.Id).ValueGeneratedOnAdd();
        }
    }
}
