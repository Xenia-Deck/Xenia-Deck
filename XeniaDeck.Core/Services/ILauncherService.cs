namespace XeniaDeck.Core.Services
{
    public interface ILauncherService
    {
        /// <summary>
        /// Opens a given folder using the file explorer
        /// </summary>
        /// <param name="folderPath"> The path of the folder to open </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task OpenFolderAsync(string folderPath);
    }
}
