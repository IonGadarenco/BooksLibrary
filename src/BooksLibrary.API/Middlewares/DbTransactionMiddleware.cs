using BooksLibrary.Infrastructure.Data;

namespace BooksLibrary.API.Middlewares
{
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public DbTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, BooksLibraryDbContext dbContext)
        {
            if (context.Request.Method == HttpMethod.Get.Method)
            {
                await _next(context);
                return;
            }

            // Rulează într-o tranzacție
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await _next(context);

                await dbContext.SaveChangesAsync(); // Salvăm schimbările
                await dbContext.Database.CommitTransactionAsync();
            }
            catch
            {
                await dbContext.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
