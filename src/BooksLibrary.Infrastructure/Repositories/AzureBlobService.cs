using System.Threading;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Infrastructure.Settings;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BooksLibrary.Infrastructure.Repositories
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AzureBlobSettings _settings;
        private readonly ILogger<AzureBlobService> _logger;

        public AzureBlobService(
            ILogger<AzureBlobService> logger,
            IOptions<AzureBlobSettings> options,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _settings = options.Value;
            _blobServiceClient = blobServiceClient;
            _logger.LogInformation("AzureBlobService: initialized for account {Account}", _settings.StorageAccount);
        }

        public async Task<bool> DeleteBlobAsync(
            string containerName,
            string fileName)
        {
            try
            {
                var blobClient = _blobServiceClient
                    .GetBlobContainerClient(containerName)
                    .GetBlobClient(fileName);

                var result = await blobClient
                    .DeleteIfExistsAsync();

                _logger.LogInformation(result
                    ? "Deleted blob {Container}/{File}"
                    : "Blob not found {Container}/{File}",
                    containerName, fileName);

                return result;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(
                    ex,
                    "AzureBlobService: Failed deleting blob {Container}/{File}: {Msg}",
                    containerName, fileName, ex.Message);
                throw;
            }
        }
        public ( string? FileName, string? ContainerName) GetBlobInfoFromUrl(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl))
                return (null, null);

            try
            {
                if (!Uri.TryCreate(blobUrl, UriKind.Absolute, out Uri? uri) || !uri.Host.Contains(".blob.core.windows.net"))
                {
                    _logger.LogWarning("AzureBlobService: Invalid Azure Blob URL format: {BlobUrl}", blobUrl);
                    return (null, null);
                }

                string fullPath = uri.LocalPath;

                string[] parts = fullPath.TrimStart('/').Split('/', 2);

                if (parts.Length < 2)
                {
                    _logger.LogWarning("AzureBlobService: Could not parse container and filename from URL path: {Path}", fullPath);
                    return (null, null);
                }

                string containerName = parts[0];
                string fileName = parts[1];

                _logger.LogInformation("AzureBlobService: Successfully extracted info from URL. Container: '{Container}', File: '{File}'", containerName, fileName);
                return (FileName: fileName, ContainerName: containerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AzureBlobService: An error occurred while extracting blob info from URL: {BlobUrl}", blobUrl);
                return (null, null);
            }
        }

        public async Task<Uri> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string? containerName = null)
        {
            var container = _blobServiceClient
                .GetBlobContainerClient(containerName ?? _settings.ContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = container.GetBlobClient(fileName);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            _logger.LogInformation("Uploaded blob {Container}/{File}", container.Name, fileName);

            return blobClient.Uri;
        }
    }
}