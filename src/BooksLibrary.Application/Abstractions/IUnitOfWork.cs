
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.Abstractions
{
    public interface IUnitOfWork
    {
        public IRepository<T> GetRepository<T>() where T : Entity;

        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
