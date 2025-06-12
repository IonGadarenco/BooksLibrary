using BooksLibrary.Infrastructure.DataSeed;
using BooksLibrary.Application;
using BooksLibrary.Infrastructure;


using MediatR;
using BooksLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);  // asigură-te că include DbContext
            builder.Services.AddApplication();     // asigură-te că înregistrează MediatR și handler-ele
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BooksLibraryDbContext>();
            db.Database.EnsureDeleted();  // Șterge DB
            db.Database.Migrate();       // Reaplică toate migrațiile
            


            // 🔽 Rulează seederul
            using (scope)
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await BooksSeed.SeedAsync(mediator);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync(); // pentru că Main e async
        }
    }
}
