namespace XeniaDeck.Core.Services
{
    public interface ILocalCacheService
    {
        /// <summary>
        /// Gets cache content from file
        /// </summary>
        /// <typeparam name="T"> Type of data to be retrieved </typeparam>
        /// <param name="fileName"> File from which to read the data </param>
        /// <returns> Data retrieved </returns>
        Task<T?> GetFromCacheAsync<T>(string fileName);

        /// <summary>
        /// Saves data to the cache in the file specified
        /// </summary>
        /// <typeparam name="T"> Type of data to be saved </typeparam>
        /// <param name="fileName">  </param>
        /// <param name="data"> Data to be saved </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task SaveToCacheAsync<T>(string fileName, T data);
    }
}