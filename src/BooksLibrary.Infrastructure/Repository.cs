
using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using BooksLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly BooksLibraryDbContext _context;

        public Repository(BooksLibraryDbContext context)
        {
            _context = context;
        }
        public async Task<T> AddAsync(T item)
        {
            _context.Set<T>().Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<List<T>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task RemoveAsync(T item)
        {
            var toDelete = _context.Set<T>()
                .FirstOrDefault(i => i.Id == item.Id);

            if (toDelete != null)
            {
                _context.Set<T>().Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            _context.Set<T>().Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
