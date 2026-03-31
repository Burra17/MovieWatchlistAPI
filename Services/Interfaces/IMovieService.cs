using MovieWatchlistAPI.Dtos;

namespace MovieWatchlistAPI.Services.Interfaces
{
    // Interface for movie-related operations, including fetching movie details from external APIs 
    // and managing the user's personal watchlist in the database.
    public interface IMovieService
    {
        // Get
        // Fetches movie details from the OMDb API based on the provided title
        Task<OmdbMovieDto?> GetMovieByTitleAsync(string title);

        // Get
        // Retrieves the user's watchlist, returning a list of movies in the watchlist
        Task<IEnumerable<WatchlistResponseDto>> GetWatchlistAsync();

        // Post
        // Adds a movie to the user's watchlist based on the provided request data (IMDb ID)
        // This method will handle external API calls, AI generation, and database storage.
        Task<WatchlistResponseDto> AddMovieToWatchlistAsync(AddMovieRequestDto request);


        // Put
        // Toggles or sets the 'IsWatched' status of a movie in the watchlist
        Task<WatchlistResponseDto?> MarkAsWatchedAsync(int id);

        // Delete
        // Removes a movie from the watchlist based on its database ID
        // Returns true if the deletion was successful
        Task<bool> DeleteMovieAsync(int id);
    }
}