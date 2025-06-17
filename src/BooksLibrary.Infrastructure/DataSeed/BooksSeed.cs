
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using MediatR;

namespace BooksLibrary.Infrastructure.DataSeed
{
    public static class BooksSeed
    {
        public static async Task SeedAsync(IMediator mediator)
        {
            var books = new List<CreateBookCommand>
            {
                new CreateBookCommand
                {
                    Title = "Clean Code",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    ISBN = "9780132350884",
                    TotalCopies = 10,
                    Publisher = new PublisherDto { FullName = "Prentice Hall", Address = "USA" },
                    Authors = new List<AuthorDto>
                    {
                        new AuthorDto { FullName = "Robert Martin" }
                    },
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { FullName = "Programming" },
                        new CategoryDto { FullName = "Best Practices" }
                    }
                },
                new CreateBookCommand
                {
                    Title = "The Pragmatic Programmer",
                    Description = "Your Journey to Mastery",
                    ISBN = "9780135957059",
                    TotalCopies = 8,
                    Publisher = new PublisherDto { FullName = "Addison-Wesley", Address = "USA" },
                    Authors = new List<AuthorDto>
                    {
                        new AuthorDto { FullName = "Andrew Hunt" },
                        new AuthorDto { FullName = "David Thomas" }
                    },
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { FullName = "Software Development" }
                    }
                },
                new CreateBookCommand
                {
                    Title = "Domain-Driven Design",
                    Description = "Tackling Complexity in the Heart of Software",
                    ISBN = "9780321125217",
                    TotalCopies = 6,
                    Publisher = new PublisherDto { FullName = "Addison-Wesley", Address = "USA" },
                    Authors = new List<AuthorDto>
                    {
                        new AuthorDto { FullName = "Eric Evans" }
                    },
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { FullName = "Architecture" },
                        new CategoryDto { FullName = "DDD" }
                    }
                },
                new CreateBookCommand
                {
                    Title = "Refactoring",
                    Description = "Improving the Design of Existing Code",
                    ISBN = "9780201485677",
                    TotalCopies = 7,
                    Publisher = new PublisherDto { FullName = "Pearson", Address = "UK" },
                    Authors = new List<AuthorDto>
                    {
                        new AuthorDto { FullName = "Martin Fowler" }
                    },
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { FullName = "Refactoring" }
                    }
                },
                new CreateBookCommand
                {
                    Title = "C# in Depth",
                    Description = "Deep dive into C# by Jon Skeet",
                    ISBN = "9781617294532",
                    TotalCopies = 12,
                    Publisher = new PublisherDto { FullName = "Manning", Address = "USA" },
                    Authors = new List<AuthorDto>
                    {
                        new AuthorDto { FullName = "Jon Skeet" }
                    },
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { FullName = "C#" }
                    }
                }
            };

            foreach (var book in books)
            {
                await mediator.Send(book);
            }
        }
    }
}
