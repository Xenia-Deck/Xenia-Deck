using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IGitHubUpdateService
    {
        /// <summary>
        /// Retrieve the emulator data from cache
        /// </summary>
        /// <param name="cacheFileName"> The name of the cache file to retrieve the data from </param>
        /// <returns> Cached emulator data </returns>
        Task<EmulatorData?> GetCachedEmulatorAsync(string cacheFileName);

        /// <summary>
        /// Retrieve the data of the latest version of the emulator
        /// </summary>
        /// <param name="repositoryId"> The repository Id of the emulator </param>
        /// <param name="fileName"> The name of the file for downloading the asset </param>
        /// <param name="cacheFileName"> The name of the cache file for saving the data </param>
        /// <returns> The emulator data </returns>
        Task<EmulatorData> GetLatestEmulatorAsync(long repositoryId, string fileName, string cacheFileName);
    }
}