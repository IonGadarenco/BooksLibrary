

namespace BooksLibrary.Application.Common.Abstractions
{
    public interface IAzureBlobService
    {
        Task<Uri> UploadFileAsync(Stream fileStream, string fileName, string containerName);
        Task<bool> DeleteBlobAsync(string containerName, string fileName);
        (  string? FileName, string? ContainerName) GetBlobInfoFromUrl(string blobUrl);
    }
}
