using MovieWatchlistAPI.Database;
using MovieWatchlistAPI.Dtos;
using MovieWatchlistAPI.Interfaces;

namespace MovieWatchlistAPI.Services
{
    public class MovieService : IMovieService
    {
        // Private fields for dependencies
        private readonly AppDbContext _appDbContext;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        // Constructor and dependencies
        public MovieService(AppDbContext appDbContext, HttpClient httpClient, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public Task<WatchlistResponseDto> AddMovieToWatchlistAsync(AddMovieRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMovieAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<WatchlistResponseDto>> GetWatchlistAsync()
        {
            throw new NotImplementedException();
        }

        public Task<WatchlistResponseDto?> MarkAsWatchedAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
