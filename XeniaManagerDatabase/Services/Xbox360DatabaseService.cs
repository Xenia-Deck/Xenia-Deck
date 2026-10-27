using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using XeniaManagerDatabase.Models;

namespace XeniaManagerDatabase.Services
{
    public class Xbox360DatabaseService : IXbox360DatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Xbox360DatabaseService> _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public Xbox360DatabaseService(HttpClient httpClient, ILogger<Xbox360DatabaseService> logger)
        {
            _httpClient = httpClient;
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
