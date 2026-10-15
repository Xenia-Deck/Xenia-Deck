using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class Xbox360DatabaseServiceTests
    {
        private readonly Xbox360DatabaseService _sut;
        private readonly MockHttpMessageHandler _httpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<Xbox360DatabaseService> _logger;

        public Xbox360DatabaseServiceTests()
        {
            _httpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_httpMessageHandler)
            {
                BaseAddress = new Uri("https://xenia-manager.github.io/x360db/")
            };

            _jsonSerializerOptions = new JsonSerializerOptions();
            _logger = Substitute.For<ILogger<Xbox360DatabaseService>>();

            _sut = new(_httpClient,
                _jsonSerializerOptions,
                _logger);
        }

        [Fact]
        public async Task GetGameInfoAsync_WhenTitleExists_ReturnsGameInfo()
        {
            // Arrange
            var titleId = 123u;

            var expectedGame = new GameInfo
                (
                    Id: "4D5307E6",
                    Title: new(Full: "Halo 3"),
                    Developer: "Bungie"
                );

            _httpMessageHandler
                .When($"{_httpClient.BaseAddress}titles/{titleId}/info.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(expectedGame, options: _jsonSerializerOptions));

            // Act
            var result = await _sut.GetGameInfoAsync(titleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("4D5307E6", result.Id);
            Assert.Equal("Halo 3", result.Title.Full);
            Assert.Empty(result.Genre);
        }

        [Fact]
        public async Task GetGameInfoAsync_WhenApiReturns404_ThrowsHttpRequestExceptionAndLogsError()
        {
            // Arrange
            uint missingTitleId = 99999;

            _httpMessageHandler
                .When($"{_httpClient.BaseAddress}titles/{missingTitleId}/info.json")
                .Respond(HttpStatusCode.NotFound); // Simulates a 404 error

            // Act 
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
                await _sut.GetGameInfoAsync(missingTitleId)
            );

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _logger.ReceivedWithAnyArgs().LogError("Error while retrieving data for game {TitleId}", 99999);
        }

        [Fact]
        public async Task GetGamesAsync__()
        {
            // Arrange
            var expectedSimpleGameArray = new SimpleGameInfo[]
            {
                new(
                    Id: "4D5307E6",
                    Title: "Halo 3",
                    Developer: "Bungie"
                ),
                new(
                    Id: "504787D8",
                    Title: "Plants vs. Zombies",
                    Developer: "PopCap"
                ),
                new(
                    Id: "454109A9",
                    Title: "Zuma's Revenge",
                    Developer: "PopCap"
                ),
            };


            _httpMessageHandler
               .When($"{_httpClient.BaseAddress}games.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(expectedSimpleGameArray, options: _jsonSerializerOptions));

            // Act
            var result = await _sut.GetGamesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("4D5307E6", result[0].Id);
            Assert.Equal("Plants vs. Zombies", result[1].Title);
            Assert.Empty(result[2].Genre);
        }

        [Fact]
        public async Task GetGamesAsync_WhenApiReturns500_ThrowsHttpRequestExceptionAndLogsError()
        {
            // Arrange
            _httpMessageHandler
                .When($"{_httpClient.BaseAddress}games.json")
                .Respond(HttpStatusCode.InternalServerError);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(_sut.GetGamesAsync);
            _logger.ReceivedWithAnyArgs().LogError("Error while retrieving the games list");
        }
    }
}
