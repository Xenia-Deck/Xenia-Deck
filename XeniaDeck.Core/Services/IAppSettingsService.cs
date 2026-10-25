using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IAppSettingsService
    {
        /// <summary>
        /// Retrieve app settings
        /// </summary>
        /// <returns> App settings </returns>
        Task<AppSettings> LoadAsync();

        /// <summary>
        /// Save app settings to disk
        /// </summary>
        /// <param name="appSettings"> The settings to save </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task SaveAsync(AppSettings appSettings);
    }
}