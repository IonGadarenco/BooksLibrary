using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.Books.Commands
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

                var authorList = new List<Author>();
                foreach (var a in request.Authors)
                {
                    var author = await _unitOfWork.GetRepository<Author>().GetByIdAsync(a.Id);
                    if (author == null)
                    {
                        author = new Author
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        };
                        author = await _unitOfWork.GetRepository<Author>().AddAsync(author);
                    }

                    authorList.Add(author);
                }

                var categoryList = new List<Category>();
                foreach (var c in request.Categories)
                {
                    var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(c.Id);
                    if (category == null)
                    {
                        category = new Category
                        {
                            Name = c.Name
                        };
                        category = await _unitOfWork.GetRepository<Category>().AddAsync(category);
                    }

                    categoryList.Add(category);
                }

                var publisher = await _unitOfWork.GetRepository<Publisher>().GetByIdAsync(request.publisher.Id);
                if (publisher == null)
                {
                    publisher = new Publisher
                    {
                        Name = request.publisher.Name,
                        Address = request.publisher.Address
                    };
                    publisher = await _unitOfWork.GetRepository<Publisher>().AddAsync(publisher);
                }

                await _unitOfWork.BeginTransactionAsync();

                var book = new Book
                {
                    Title = request.Title,
                    Description = request.Description,
                    ISBN = request.ISBN,
                    TotalCopies = request.TotalCopies,
                    PublisherId = publisher.Id,
                    Authors = authorList,
                    Categories = categoryList
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
