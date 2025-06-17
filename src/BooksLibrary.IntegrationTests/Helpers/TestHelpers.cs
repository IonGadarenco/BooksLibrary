using AutoMapper;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Mappings;
using BooksLibrary.Domain.Models;
using BooksLibrary.Infrastructure;
using BooksLibrary.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BooksLibrary.IntegrationTests.Helpers
{
    public static class TestHelpers
    {
       public static IMapper CreateMappre() 
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(MappingProfile));
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IMapper>();
        }

        public static IMediator CreateMediator()
        {
            var services = new ServiceCollection();

            // DbContext EF Core InMemory
            services.AddDbContext<BooksLibraryDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Repository-uri generice
            services.AddScoped(typeof(IRepository<Book>), typeof(Repository<Book>));
            services.AddScoped(typeof(IRepository<Author>), typeof(Repository<Author>));
            services.AddScoped(typeof(IRepository<Category>), typeof(Repository<Category>));
            services.AddScoped(typeof(IRepository<Publisher>), typeof(Repository<Publisher>));

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // MediatR (scanează toate handler-ele)
            services.AddMediatR(typeof(CreateBookCommand).Assembly);

            // Logger generic
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IMediator>();
        }

    }
}
