using BooksLibrary.Infrastructure.DataSeed;
using BooksLibrary.Application;
using BooksLibrary.Infrastructure;
using MediatR;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.API.Middleware;
using Serilog;

namespace BooksLibrary.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, config) =>
            {
                config.ReadFrom.Configuration(ctx.Configuration);
            });


            builder.Services.AddInfrastructure(builder.Configuration); 
            builder.Services.AddApplication();     
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BooksLibraryDbContext>();
            //db.Database.EnsureDeleted();  
            //db.Database.Migrate();       
            
            using (scope)
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await BooksSeed.SeedAsync(mediator);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}
