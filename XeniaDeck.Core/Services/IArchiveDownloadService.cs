namespace XeniaDeck.Core.Services
{
    public interface IArchiveDownloadService
    {
        /// <summary>
        /// Download archive from url and extract to destination folder
        /// </summary>
        /// <param name="downloadUrl"> Url of the archive </param>
        /// <param name="destinationFolderPath"> Destination folder for archive content </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task DownloadAndExtractAsync(string downloadUrl, string destinationFolderPath);
    }
}