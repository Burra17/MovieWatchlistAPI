using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MovieWatchlistAPI.Dtos;
using MovieWatchlistAPI.Services.Interfaces;

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
        // Adds a new movie to the watchlist based on the provided IMDb ID
        [HttpPost]
        public async Task<IActionResult> AddMovie(
            [FromBody] AddMovieRequestDto request, // The request body containing the IMDb ID
            [FromServices] IValidator<AddMovieRequestDto> validator) // Injecting the FluentValidation validator for AddMovieRequestDto
        {
            // 1. Syntax/Format validation (FluentValidation)
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            try
            {
                var result = await movieService.AddMovieToWatchlistAsync(request);
                return Ok(result);
            }
            // 2. Business Logic validation (Captured from Service)
            catch (InvalidOperationException ex)
            {
                // This specifically handles the "Movie already exists" scenario
                return BadRequest(new { message = ex.Message });
            }
            // 3. Technical/Unexpected errors
            catch (Exception ex)
            {
                // For unexpected errors, it's cleaner to return a 500 or a generic message
                return StatusCode(500, new { message = "An internal error occurred.", details = ex.Message });
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