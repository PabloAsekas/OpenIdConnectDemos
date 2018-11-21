using Microsoft.EntityFrameworkCore;

namespace ECApi.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var filmEntity = modelBuilder.Entity<Film>();
            filmEntity.ToTable("movies");
            filmEntity.Property(f => f.Director).HasColumnName("director_name");
            filmEntity.Property("keywords").HasColumnName("plot_keywords");
            filmEntity.Property(f => f.ImdbLink).HasColumnName("movie_imdb_link");
            filmEntity.Property(f => f.Title).HasColumnName("movie_title");
            filmEntity.Property(f => f.Year).HasColumnName("title_year");
            filmEntity.Property(f => f.ImdbScore).HasColumnName("imdb_score");
        }
    }
}