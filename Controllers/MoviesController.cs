using Microsoft.AspNetCore.Mvc;
using MovieWatchlistAPI.Dtos;
using MovieWatchlistAPI.Interfaces;

namespace MovieWatchlistAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController(IMovieService movieService) : ControllerBase
    {
        // GET: api/movies/search/{title}
        [HttpGet("search/{title}")]
        public async Task<IActionResult> SearchMovie(string title)
        {
            var result = await movieService.GetMovieByTitleAsync(title);

            if (result == null)
                return NotFound($"Movie with title '{title}' not found.");

            return Ok(result);
        }

        // GET: api/movies
        // Retrieves the entire watchlist from the database
        [HttpGet]
        public async Task<IActionResult> GetWatchlist()
        {
            var watchlist = await movieService.GetWatchlistAsync();
            return Ok(watchlist);
        }

        // POST: api/movies
        // Adds a new movie to the watchlist using an IMDb ID
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] AddMovieRequestDto request)
        {
            try
            {
                var result = await movieService.AddMovieToWatchlistAsync(request);
                return Ok(result);
                // Tip: In a production app, we'd use CreatedAtAction here
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/movies/{id}/watched
        // Updates the status of a movie to 'Watched'
        [HttpPut("{id}/watched")]
        public async Task<IActionResult> MarkAsWatched(int id)
        {
            var result = await movieService.MarkAsWatchedAsync(id);

            if (result == null)
                return NotFound($"Movie with ID {id} not found in your watchlist.");

            return Ok(result);
        }

        // DELETE: api/movies/{id}
        // Removes a movie from the database
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var success = await movieService.DeleteMovieAsync(id);

            if (!success)
                return NotFound($"Could not find movie with ID {id} to delete.");

            return NoContent(); // 204 No Content is standard for successful deletes
        }
    }
}