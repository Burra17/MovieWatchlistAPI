using Microsoft.EntityFrameworkCore;
using MovieWatchlistAPI.Database;
using MovieWatchlistAPI.Dtos;
using MovieWatchlistAPI.Services.Interfaces;
using MovieWatchlistAPI.Models;

namespace MovieWatchlistAPI.Services
{
    public class MovieService : IMovieService
    {
        // Private fields for dependencies
        private readonly AppDbContext _appDbContext;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAiService _aiService;


        // Constructor and dependencies
        public MovieService(
            AppDbContext appDbContext, 
            HttpClient httpClient, 
            IConfiguration configuration,
            IAiService aiService)
        {
            _appDbContext = appDbContext;
            _httpClient = httpClient;
            _configuration = configuration;
            _aiService = aiService;
        }

        public async Task<WatchlistResponseDto> AddMovieToWatchlistAsync(AddMovieRequestDto request)
        {
            // 1. Check if the movie already exists in our database
            var existingMovie = await _appDbContext.WatchlistMovies
                .FirstOrDefaultAsync(m => m.ImdbId == request.ImdbId);

            if (existingMovie != null)
            {
                // Throwing an exception to inform the user that the movie is already present
                // This will be caught by the try-catch block in the Controller
                throw new InvalidOperationException($"The movie '{existingMovie.Title}' is already in your watchlist.");
            }

            // 2. Fetch full movie details from OMDb using the ImdbId
            var apiKey = _configuration["OMDb:ApiKey"];
            var url = $"https://www.omdbapi.com/?i={request.ImdbId}&apikey={apiKey}";

            var omdbResponse = await _httpClient.GetFromJsonAsync<OmdbMovieDto>(url);

            if (omdbResponse == null || string.IsNullOrEmpty(omdbResponse.Title))
            {
                throw new Exception("Movie not found in OMDb.");
            }

            // Generate AI pitch using the IAiService
            // We use the null-coalescing operator (??) to handle potential null plots
            var aiPitch = await _aiService.GenerateMoviePitchAsync(
                                omdbResponse.Title,
                                omdbResponse.Plot ?? "No plot available"
                            );

            // 3. Create the Database Entity
            // We safely parse the year, taking only the first 4 characters to handle date ranges (e.g., "2019–2023")
            var movieEntity = new WatchlistMovie
            {
                ImdbId = omdbResponse.imdbID,
                Title = omdbResponse.Title,
                Year = int.TryParse(omdbResponse.Year.Substring(0, 4), out int year) ? year : 0,
                AiPitch = aiPitch,
                IsWatched = false
            };

            // 4. Save to SQL Server
            _appDbContext.WatchlistMovies.Add(movieEntity);
            await _appDbContext.SaveChangesAsync();

            // 5. Return the response DTO
            return new WatchlistResponseDto(
                movieEntity.ImdbId,
                movieEntity.Title,
                movieEntity.Year,
                movieEntity.AiPitch,
                movieEntity.IsWatched
            );
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            // 1. Find the movie by its primary key (Id)
            var movie = await _appDbContext.WatchlistMovies.FindAsync(id);

            // 2. If the movie doesn't exist, return false to indicate failure
            if (movie == null) return false;

            // 3. If it exists, remove it from the DbSet and save changes to the database
            _appDbContext.WatchlistMovies.Remove(movie);
            await _appDbContext.SaveChangesAsync();

            // 4. Return true to indicate successful deletion
            return true;
        }

        public async Task<OmdbMovieDto?> GetMovieByTitleAsync(string title)
        {
            // 1. Retrieve the OMDb API key from user secrets via IConfiguration
            var apiKey = _configuration["OMDb:ApiKey"];

            // 2. Construct the request URL. 
            // We use Uri.EscapeDataString to handle spaces and special characters in movie titles (e.g., "The Matrix").
            var url = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={apiKey}";

            // 3. Perform the asynchronous GET request to the OMDb API
            var response = await _httpClient.GetAsync(url);

            // 4. Check if the HTTP request was successful (Status Code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Automatically deserialize the JSON response body into our OmdbMovieDto record
                var movieData = await response.Content.ReadFromJsonAsync<OmdbMovieDto>();

                // OMDb sometimes returns "200 OK" even if the movie wasn't found, 
                // returning a JSON with an Error property. We check for a valid Title to confirm success.
                if (movieData == null || string.IsNullOrEmpty(movieData.Title))
                {
                    return null;
                }

                return movieData;
            }

            // Return null if the API call failed or the movie doesn't exist
            return null;
        }

        public async Task<IEnumerable<WatchlistResponseDto>> GetWatchlistAsync()
        {
            var movies = await _appDbContext.WatchlistMovies.ToListAsync();

            // Map each database entity to a Response DTO
            return movies.Select(m => new WatchlistResponseDto(
                m.ImdbId,
                m.Title,
                m.Year,
                m.AiPitch,
                m.IsWatched
            ));
        }

        public async Task<WatchlistResponseDto?> MarkAsWatchedAsync(int id)
        {
            // 1. Find the movie by its primary key (Id)
            var movie = await _appDbContext.WatchlistMovies.FindAsync(id);

            // 2. If the movie doesn't exist, return null to indicate failure
            if (movie == null) return null;

            // 3. If it exists, update the IsWatched property and save changes to the database
            movie.IsWatched = true;
            await _appDbContext.SaveChangesAsync();

            // 4. Return the updated movie as a WatchlistResponseDto
            return new WatchlistResponseDto(
                movie.ImdbId,
                movie.Title,
                movie.Year,
                movie.AiPitch,
                movie.IsWatched
            );
        }
    }
}
