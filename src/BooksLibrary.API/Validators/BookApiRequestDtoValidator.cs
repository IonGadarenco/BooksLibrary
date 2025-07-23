using BooksLibrary.API.Controllers.Dtos;
using FluentValidation;

namespace BooksLibrary.API.Validators
{
    public class BookApiRequestDtoValidator : AbstractValidator<BookApiRequestDto>
    {
        public BookApiRequestDtoValidator()
        {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.ISBN)
              .NotEmpty().WithMessage("ISBN is required");
            RuleFor(x => x.TotalCopies)
              .GreaterThan(0)
              .WithMessage("Total copies must be at least 1");
            RuleFor(x => x.Publisher)
              .NotNull().WithMessage("Publisher is required");
            RuleFor(x => x.Authors)
              .NotEmpty().WithMessage("At least one author is required");
            RuleFor(x => x.Categories)
              .NotEmpty().WithMessage("At least one category is required");
        }
    }
}
