using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class GameLibraryServiceTests
    {
        private readonly GameLibraryService _sut;
        private readonly MockFileSystem _fileSystem;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly IXbox360DatabaseService _databaseService;
        private readonly IGameFileExtensionIdentifierService _gameFileExtensionIdentifierService;
        private readonly ILogger<GameLibraryService> _logger;

        public GameLibraryServiceTests()
        {
            _fileSystem = new MockFileSystem();
            _jsonSerializerOptions = new JsonSerializerOptions();
            _internalFoldersService = Substitute.For<IInternalFoldersService>();
            _internalFoldersService.GetConfigFolderPath().Returns(@"C:\XeniaDeck\Config");

            _databaseService = Substitute.For<IXbox360DatabaseService>();
            _gameFileExtensionIdentifierService = Substitute.For<IGameFileExtensionIdentifierService>();
            _logger = Substitute.For<ILogger<GameLibraryService>>();

            _sut = new GameLibraryService(_fileSystem,
                _jsonSerializerOptions,
                _internalFoldersService,
                _databaseService,
                _gameFileExtensionIdentifierService,
                _logger);
        }

        [Fact]
        public async Task GetGamesAsync_WhenFileIsEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            _fileSystem.AddFile(@"C:\XeniaDeck\Config\games.json", new MockFileData(""));

            // Act
            var result = await _sut.GetGamesAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGamesAsync_WhenFileHasValidJson_ReturnsGames()
        {
            // Arrange
            var sampleGames = new List<Game>
            {
                new(536473, true, @"C:\XeniaDeck\Games\game1.xbla", new()),
                new(359593, true, @"C:\XeniaDeck\Games\game2.xbla", new()),
                new(425894, true, @"C:\XeniaDeck\Games\game3.xbla", new()),
                new(759302, true, @"C:\XeniaDeck\Games\game4.xbla", new()),
                new(468263, true, @"C:\XeniaDeck\Games\game5.xbla", new())
            };

            var jsonContent = JsonSerializer.Serialize(sampleGames, _jsonSerializerOptions);

            _fileSystem.AddFile(@"C:\XeniaDeck\Config\games.json", new MockFileData(jsonContent));

            // Act
            var result = await _sut.GetGamesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetGameDetailsAsync_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _sut.GetGameDetailsAsync(@"C:\MissingGame.iso"));
        }

        [Fact]
        public async Task GetGameDetailsAsync_WhenFileTypeIsUnknown_ThrowsNotSupportedException()
        {
            // Arrange
            var filePath = @"C:\XeniaDeck\Games\Unknown Game.xbla";
            _fileSystem.AddFile(filePath, new MockFileData(new byte[100]));
            _gameFileExtensionIdentifierService.IdentifyFileType(filePath).Returns(FileSignature.Unknown);

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() => _sut.GetGameDetailsAsync(filePath));
        }

        [Fact]
        public async Task SaveGamesAsync_WhenGivenGames_GamesAreSaved()
        {
            // Arrange
            var sampleGames = new List<Game>
            {
                new(536473, true, @"C:\XeniaDeck\Games\game1.xbla", new()),
                new(359593, true, @"C:\XeniaDeck\Games\game2.xbla", new()),
                new(425894, true, @"C:\XeniaDeck\Games\game3.xbla", new()),
                new(759302, true, @"C:\XeniaDeck\Games\game4.xbla", new()),
                new(468263, true, @"C:\XeniaDeck\Games\game5.xbla", new())
            };

            _fileSystem.AddFile(@"C:\XeniaDeck\Config\games.json", new MockFileData(string.Empty));

            // Act
            await _sut.SaveGamesAsync(sampleGames);

            // Assert
            using var fileSystemStream = _fileSystem.FileStream.New(@"C:\XeniaDeck\Config\games.json", FileMode.Open);
            var games = JsonSerializer.Deserialize<List<Game>>(fileSystemStream, _jsonSerializerOptions);

            Assert.NotNull(games);
            Assert.NotEmpty(games);
        }
    }
}
