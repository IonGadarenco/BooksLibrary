using System.Linq.Expressions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using BooksLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly BooksLibraryDbContext _context;

        public Repository(BooksLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByAuth0IdAsync(string auth0Id)
        {

            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);

            return user;
        }
        public async Task<T> AddAsync(T item)
        {
            _context.Set<T>().Add(item);
            return item;
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Book))
            {
                var book = await _context.Set<Book>()
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .Include(b => b.Publisher)
                    .FirstOrDefaultAsync(b => b.Id == id);

                return book as T;
            }

            return await _context.Set<T>().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> GetByNameAsync(string name)
        {
            if (typeof(T) == typeof(Author))
            {
                var author = await _context.Set<Author>().FirstOrDefaultAsync(a => a.FullName.ToLower().Trim() == name.ToLower().Trim());
                return author as T;
            }

            if (typeof(T) == typeof(Category))
            {
                var category = await _context.Set<Category>().FirstOrDefaultAsync(c => c.FullName.ToLower().Trim() == name.ToLower().Trim());
                return category as T;
            }

            if (typeof(T) == typeof(Publisher))
            {
                var publisher = await _context.Set<Publisher>().FirstOrDefaultAsync(p => p.FullName.ToLower().Trim() == name.ToLower().Trim());
                return publisher as T;
            }

            return null;
        }

        public async Task<Book?> GetByTitleAndIsbnAsync(string title, string isbn)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b =>
                    b.Title.ToLower().Trim() == title.ToLower().Trim() &&
                    b.ISBN.ToLower().Trim() == isbn.ToLower().Trim());
        }

        public async Task RemoveAsync(T item)
        {
            var toDelete = _context.Set<T>()
                .FirstOrDefault(i => i.Id == item.Id);

            if (toDelete != null)
            {
                _context.Set<T>().Remove(toDelete);
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            _context.Set<T>().Update(item);
            return item;
        }

        public async Task<bool> DoesIsbnExist(string isbn)
        {
            var result = await _context.Set<Book>().FirstOrDefaultAsync(b => b.ISBN == isbn);

            return result == null ? true : false;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
