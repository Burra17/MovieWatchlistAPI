namespace MovieWatchlistAPI.Services.Interfaces
{
    public interface IAiService // This interface defines the contract for AI-related operations, such as generating movie pitches based on movie details.
    {
        // Generate a movie pitch using AI based on the provided title and plot
        Task<string> GenerateMoviePitchAsync(string title, string plot);
    }
}
