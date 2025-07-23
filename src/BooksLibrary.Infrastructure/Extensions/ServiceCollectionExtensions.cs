using Azure.Storage.Blobs;
using Azure.Storage;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Infrastructure.Data;
using BooksLibrary.Infrastructure.Repositories;
using BooksLibrary.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;      // for AddOptions<T>

namespace BooksLibrary.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BooksLibraryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.Configure<AzureBlobSettings>(opts => configuration.GetSection("AzureBlobSettings").Bind(opts));

            services.AddSingleton(sp =>
            {
                var settings = sp
                  .GetRequiredService<IOptions<AzureBlobSettings>>()
                  .Value;

                var credential = new StorageSharedKeyCredential(
                  settings.StorageAccount,
                  settings.StorageKey);
                var blobUri = new Uri(
                  $"https://{settings.StorageAccount}.blob.core.windows.net");
                return new BlobServiceClient(blobUri, credential);
            });

            services.AddScoped<IAzureBlobService, AzureBlobService>();

            return services;
        }
    }
}
