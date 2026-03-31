using FluentValidation;
using MovieWatchlistAPI.Dtos;

namespace MovieWatchlistAPI.Validators
{
    public class AddMovieRequestValidator : AbstractValidator<AddMovieRequestDto>
    {
        public AddMovieRequestValidator()
        {
            RuleFor(x => x.ImdbId)
                .NotEmpty().WithMessage("IMDb ID cannot be empty.")
                .NotNull()
                .Matches(@"^tt\d{7,10}$").WithMessage("Invalid IMDb ID format. It should start with 'tt' followed by 7-10 digits (e.g., tt1375666)."); // IMDb IDs typically start with 'tt' followed by 7-10 digits
        }
    }
}