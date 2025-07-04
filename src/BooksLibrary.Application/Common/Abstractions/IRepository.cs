﻿using BooksLibrary.Application.Common.Models;
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.Commun.Abstractions
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByNameAsync(string name);
        Task<Book> GetByTitleAndIsbnAsync(string title, string isbn);
        Task<T> AddAsync(T item);
        Task RemoveAsync(T item);
        Task<IQueryable<T>> GetAllAsync();
        Task<T> UpdateAsync(T item);
        Task<bool> CheckIsbn(string isbn);
        Task SaveChangesAsync();
    }
}
