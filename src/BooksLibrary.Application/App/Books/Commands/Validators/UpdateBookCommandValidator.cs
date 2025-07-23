

using BooksLibrary.Application.App.Books.Commands.DTOs;
using FluentValidation;

namespace BooksLibrary.Application.App.Books.Commands.Validators
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommandDto>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.ISBN).NotEmpty();
            RuleFor(x => x.TotalCopies).GreaterThan(0);
            RuleFor(x => x.Publisher).NotNull();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Categories).NotEmpty();
        }
    }
}
