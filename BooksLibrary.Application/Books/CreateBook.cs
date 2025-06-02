
using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.Books
{
    public record CreateBook(
        string Title,
        string Description,
        string ISBN,
        int TotalCopies,
        Publisher publisher,
        List<Author> Authors,
        List<Category> Categories
        ) : IRequest<Book>;
    public class CreateBookHandler : IRequestHandler<CreateBook, Book>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateBookHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Book> Handle(CreateBook request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                 request.Authors.ForEach( a => {
                    var author = _unitOfWork.GetRepository<Author>().GetByIdAsync(a.Id);

                    if (author is null)
                    {
                        var newAuthor = new Author
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        };

                         author = _unitOfWork.GetRepository<Author>().AddAsync(newAuthor);
                    }
                });

                request.Categories.ForEach(c => {
                    var category = _unitOfWork.GetRepository<Category>().GetByIdAsync(c.Id);

                    if (category is null)
                    {
                        var newCategory = new Category
                        {
                            Name = c.Name
                        };

                        category = _unitOfWork.GetRepository<Category>().AddAsync(newCategory);
                    }
                });

                var publisher = _unitOfWork.GetRepository<Publisher>().GetByIdAsync(request.publisher.Id);

                if (publisher is null)
                {
                    var newPublisher = new Publisher
                    {
                        Name = request.publisher.Name,
                        Address = request.publisher.Address
                    };

                    publisher = _unitOfWork.GetRepository<Publisher>().AddAsync(newPublisher);
                }

                var book = new Book
                {
                    Title = request.Title,
                    Description = request.Description,
                    ISBN = request.ISBN,
                    TotalCopies = request.TotalCopies,
                    PublisherId = publisher.Id,
                    Authors = request.Authors,
                    Categories = request.Categories
                };

                book = await _unitOfWork.GetRepository<Book>().AddAsync(book);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return book;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
