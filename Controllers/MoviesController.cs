using Microsoft.AspNetCore.Mvc;
using MovieWatchlistAPI.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet("search/{title}")]
    public async Task<IActionResult> SearchMovie(string title)
    {
        var result = await movieService.GetMovieByTitleAsync(title);

        if (result == null)
            return NotFound($"Movie with title '{title}' not found.");

        return Ok(result);
    }
}