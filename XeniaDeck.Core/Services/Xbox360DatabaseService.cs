using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    // TODO: i think this can be moved to it's own project since it's not dependent on me
    // TODO: should prob delete the tests too
    // TODO: the rest of the project will be less dependent on this, I swear

    public class Xbox360DatabaseService : IXbox360DatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<Xbox360DatabaseService> _logger;

        public Xbox360DatabaseService(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, ILogger<Xbox360DatabaseService> logger)
        {
            _httpClient = httpClient;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        public async Task<SimpleGameInfo[]> GetGamesAsync()
        {
            try
            {
                var games = await _httpClient.GetFromJsonAsync<SimpleGameInfo[]>("games.json", _jsonSerializerOptions);

                return games ?? [];
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving the games list");
                throw;
            }
        }

        public async Task<GameInfo?> GetGameInfoAsync(uint titleId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<GameInfo>($"titles/{titleId}/info.json", _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving data for game {TitleId}", titleId);
                throw;
            }
        }
    }
}
