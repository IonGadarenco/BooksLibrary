using BooksLibrary.Infrastructure.DataSeed;
using BooksLibrary.Application;
using BooksLibrary.Infrastructure;
using MediatR;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.API.Middleware;
using Serilog;
using BooksLibrary.API.Extensions;
using BooksLibrary.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using BooksLibrary.Infrastructure.Repositories;
using BooksLibrary.Application.Mappings;
using BooksLibrary.API.Profiles;

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
            builder.Services.AddAutoMapper(
                typeof(ApplicationMappingProfile).Assembly,
                typeof(ApiMappingProfiles).Assembly);

            //builder.AddApiServices();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddApiValidation();

            builder.RegisterAuthentication();
            builder.RegisterCors();
            builder.RegisterSwagger();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BooksLibraryDbContext>();
                //db.Database.EnsureDeleted();
                //db.Database.Migrate();
                db.Database.Migrate();

                using (scope)
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    //await BooksSeed.SeedAsync(mediator);
                }

                //app.UseSwagger();
                //app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<DbTransactionMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}