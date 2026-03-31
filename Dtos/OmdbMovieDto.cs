namespace MovieWatchlistAPI.Dtos
{
    // DTO for OMDb API response
    public record OmdbMovieDto(
        string Title,
        string Year,
        string imdbID,
        string? Plot
    );
}