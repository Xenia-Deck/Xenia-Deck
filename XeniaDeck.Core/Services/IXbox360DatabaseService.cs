using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IXbox360DatabaseService
    {
        /// <summary>
        /// Get detailed information from a game
        /// </summary>
        /// <param name="titleId"> The title for which you want the details </param>
        /// <returns> The game's information </returns>
        Task<GameInfo?> GetGameInfoAsync(uint titleId);

        /// <summary>
        /// Get a less detailed list of all games available on the Xbox360
        /// </summary>
        /// <returns> Information from every game </returns>
        Task<SimpleGameInfo[]> GetGamesAsync();
    }
}