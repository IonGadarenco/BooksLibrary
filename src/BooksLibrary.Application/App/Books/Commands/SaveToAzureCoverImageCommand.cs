

using BooksLibrary.Application.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class SaveToAzureCoverImageCommand : IRequest<string>
    {
        public IFormFile CoverImage { get; set; }
    }

    public class SaveToAzureCoverImageCommandHandler : IRequestHandler<SaveToAzureCoverImageCommand, string>
    {
        private readonly IAzureBlobService _azureBlobService;
        private readonly ILogger<SaveToAzureCoverImageCommandHandler> _logger;
        public SaveToAzureCoverImageCommandHandler
            (IAzureBlobService azureBlobService, 
            ILogger<SaveToAzureCoverImageCommandHandler> logger)
        {
            _azureBlobService = azureBlobService;
            _logger = logger;
        }
        public async Task<string> Handle(SaveToAzureCoverImageCommand request, CancellationToken cancellationToken)
        {
            string coverImageUrl = null;

            if (request.CoverImage != null && request.CoverImage.Length > 0)
            {
                try
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.CoverImage.FileName)}";
                    string containerName = "bookscoverimg";

                    Uri blobUri = await _azureBlobService.UploadFileAsync(
                        request.CoverImage.OpenReadStream(),
                        fileName,
                        containerName);

                    coverImageUrl = blobUri.ToString();
                    _logger.LogInformation("Successfully uploaded image for new book. URL: {Url}", coverImageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload cover image");
                }
            }
            else
            {
                _logger.LogWarning("No cover image file provided");
            }
            return coverImageUrl;
        }
    }
}
