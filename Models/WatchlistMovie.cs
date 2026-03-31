namespace MovieWatchlistAPI.Models
{
    public class WatchlistMovie // This class represents a movie in the user's watchlist
    {
        public int Id { get; set; } // Unique identifier for the watchlist movie entry
        public string ImdbId { get; set; } = null!; // IMDb ID of the movie
        public string Title { get; set; } = null!; // Title of the movie
        public int Year { get; set; } // Release year of the movie
        public string? AiPitch { get; set; } // AI-generated pitch for the movie
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; // Timestamp when the movie was added to the watchlist
        public bool IsWatched { get; set; } = false; // Indicates whether the user has watched the movie
    }
}
