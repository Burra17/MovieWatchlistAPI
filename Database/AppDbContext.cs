using Microsoft.EntityFrameworkCore;
using MovieWatchlistAPI.Models;

namespace MovieWatchlistAPI.Database
{
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes it to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet representing the WatchlistMovies table in the database
        public DbSet<WatchlistMovie> WatchlistMovies { get; set; } = null!;
    }
}
