
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.Abstractions
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T item);
        Task RemoveAsync(T item);
        Task<List<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T> UpdateAsync(T item);
        Task GetAllAsync();
    }
}
