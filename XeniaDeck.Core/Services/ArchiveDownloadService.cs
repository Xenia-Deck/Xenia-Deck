using SharpCompress.Archives;
using SharpCompress.Readers;

namespace XeniaDeck.Core.Services
{
    public class ArchiveDownloadService : IArchiveDownloadService
    {
        private readonly HttpClient _httpClient;

        public ArchiveDownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadAndExtractAsync(string downloadUrl, string destinationFolderPath)
        {
            using var response = await _httpClient.GetAsync(downloadUrl);

            using var stream = await response.Content.ReadAsStreamAsync();

            await using var archive = await ArchiveFactory.OpenAsyncArchive(stream);

            await using var reader = await archive.ExtractAllEntriesAsync();

            await reader.WriteAllToDirectoryAsync(destinationFolderPath);
        }
    }
}
