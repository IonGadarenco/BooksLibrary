using AutoMapper;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Mappings;
using BooksLibrary.Domain.Models;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http; // <-- ADAUGĂ ACEST USING
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq; // <-- ADAUGĂ ACEST USING

namespace BooksLibrary.IntegrationTests.Helpers
{
    public static class TestHelpers
    {
        // Această metodă rămâne neschimbată, dar nu o vom mai folosi direct.
        public static IMapper CreateMappre()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(ApplicationMappingProfile));
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IMapper>();
        }

        // Metoda existentă, pe care o vom păstra pentru testele simple.
        // O vom face să o apeleze pe cea nouă, pentru a nu duplica cod.
        public static IMediator CreateMediator()
        {
            // Apelează noua metodă fără a-i pasa un mock.
            return CreateMediator(null);
        }

        // NOUA METODĂ SUPRAÎNCĂRCATĂ (OVERLOADED)
        // Aceasta este metoda principală pe care o vom folosi.
        public static IMediator CreateMediator(Mock<IHttpContextAccessor> httpContextAccessorMock)
        {
            var services = new ServiceCollection();

            // DbContext EF Core InMemory
            // Folosim un Guid nou pentru fiecare mediator creat pentru a asigura izolarea testelor.
            services.AddDbContext<BooksLibraryDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Repository-uri generice
            // Nu mai este nevoie să le înregistrezi pe fiecare în parte.
            // O singură linie pentru toate.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // AutoMapper
            services.AddAutoMapper(typeof(ApplicationMappingProfile));

            // MediatR (scanează toate handler-ele)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly);
            });

            // Logger generic
            services.AddLogging();

            // ADAUGĂM LOGICA PENTRU HTTP CONTEXT ACCESSOR
            // Dacă nu primim un mock din test, creăm unul gol.
            // Acest lucru asigură că handler-ul nu va crăpa chiar dacă testul nu are nevoie de un rol specific.
            if (httpContextAccessorMock == null)
            {
                httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            }
            // Înregistrăm instanța mock-ului în containerul de servicii.
            services.AddSingleton(httpContextAccessorMock.Object);


            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IMediator>();
        }
    }
}