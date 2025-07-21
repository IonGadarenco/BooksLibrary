

namespace BooksLibrary.Infrastructure.Settings
{
    public class AzureBlobSettings
    {
        public string StorageAccount { get; set; } = default!;
        public string StorageKey { get; set; } = default!;
        public string ContainerName { get; set; } = "bookscoverimg";
    }
}
