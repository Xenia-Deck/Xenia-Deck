using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IGameLibraryService
    {
        /// <summary>
        /// Retrieve game details from a ROM file
        /// </summary>
        /// <param name="filePath"> The path of the ROM file </param>
        /// <returns> The game details </returns>
        Task<Game?> GetGameDetailsAsync(string filePath);

        /// <summary>
        /// Retrieve games from games file
        /// </summary>
        /// <returns> Game data for saved games </returns>
        Task<IEnumerable<Game>> GetGamesAsync();

        /// <summary>
        /// Writes the game data to the games file
        /// </summary>
        /// <param name="games"> Games being saved </param>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task SaveGamesAsync(IEnumerable<Game> games);
    }
}