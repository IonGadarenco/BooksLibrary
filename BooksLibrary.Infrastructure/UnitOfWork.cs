
using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using BooksLibrary.Infrastructure.Data;

namespace BooksLibrary.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BooksLibraryDbContext _context;
        public UnitOfWork( BooksLibraryDbContext context )
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : Entity
        {
            return new Repository<T>(_context);
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
