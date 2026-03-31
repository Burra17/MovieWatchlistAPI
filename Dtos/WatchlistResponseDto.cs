namespace MovieWatchlistAPI.Dtos
{
    // DTO for returning movie details in the watchlist response
    public record WatchlistResponseDto(
        string ImdbId,
        string Title,
        int Year,
        string? AiPitch,
        bool IsWatched
    );
}