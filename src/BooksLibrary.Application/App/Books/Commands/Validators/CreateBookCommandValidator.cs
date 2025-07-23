

using FluentValidation;

namespace BooksLibrary.Application.App.Books.Commands.Validators
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.ISBN).NotEmpty();
            RuleFor(x => x.TotalCopies)
              .GreaterThan(0).WithMessage("At least one copy required");
            RuleFor(x => x.CoverImageFile)
              .NotNull().WithMessage("Cover image is required");
            RuleFor(x => x.Publisher).NotNull();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Categories).NotEmpty();
        }
    }
}
