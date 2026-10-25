using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class LocalCacheServiceTests
    {
        private readonly LocalCacheService _sut;
        private readonly MockFileSystem _fileSystem;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<LocalCacheService> _logger;
        private readonly string _fakeCacheFolder = @"C:\CacheFolder";

        public LocalCacheServiceTests()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.AddDirectory(_fakeCacheFolder);

            _jsonSerializerOptions = new JsonSerializerOptions();
            _internalFoldersService = Substitute.For<IInternalFoldersService>();
            _internalFoldersService.LocalCacheFolderPath.Returns(_fakeCacheFolder);
            _logger = Substitute.For<ILogger<LocalCacheService>>();

            _sut = new LocalCacheService(_fileSystem,
                _jsonSerializerOptions,
                _internalFoldersService,
                _logger);
        }

        [Fact]
        public async Task SaveToCacheAsync_WhenCalled_ShouldWriteJsonFileToDisk()
        {
            // Arrange
            var fileName = "test.json";
            var expectedPath = Path.Combine(_fakeCacheFolder, fileName);
            var testData = new EmulatorReleaseInfo("Xenia", "www.example.com");

            // Act
            await _sut.SaveToCacheAsync(fileName, testData);

            // Assert
            Assert.True(_fileSystem.FileExists(expectedPath));

            var savedJson = await _fileSystem.File.ReadAllTextAsync(expectedPath, TestContext.Current.CancellationToken);
            var deserializedData = JsonSerializer.Deserialize<EmulatorReleaseInfo>(savedJson);

            Assert.NotNull(deserializedData);
            Assert.Equal("Xenia", deserializedData.Name);
            Assert.Equal("www.example.com", deserializedData.DownloadUrl);
        }

        [Fact]
        public async Task SaveToCacheAsync_WhenUnexpectedErrorOccurs_ShouldLogErrorAndThrow()
        {
            // Arrange
            var fileName = "invalid/path/name.json";

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
                _sut.SaveToCacheAsync(fileName, new EmulatorReleaseInfo("Xenia", "www.example.com"))
            );

            _logger.Received(1).Log(
               LogLevel.Error,
               Arg.Any<EventId>(),
               Arg.Is<object>(state => state.ToString()!.Contains($"Failed to save data to cache file: {fileName}")),
               Arg.Any<Exception>(),
               Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task GetFromCacheAsync_WhenFileExists_ShouldReturnDeserializedObject()
        {
            // Arrange
            var fileName = "cached_data.json";
            var filePath = Path.Combine(_fakeCacheFolder, fileName);
            var originalData = new EmulatorReleaseInfo("Xenia Stable", "www.example.com");

            var json = JsonSerializer.Serialize(originalData, _jsonSerializerOptions);
            _fileSystem.AddFile(filePath, new MockFileData(json));

            // Act
            var result = await _sut.GetFromCacheAsync<EmulatorReleaseInfo>(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Xenia Stable", result.Name);
            Assert.Equal("www.example.com", result.DownloadUrl);
        }

        [Fact]
        public async Task GetFromCacheAsync_WhenFileIsEmpty_ShouldReturnDefault()
        {
            // Arrange
            var fileName = "empty.json";
            var filePath = Path.Combine(_fakeCacheFolder, fileName);
            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));

            // Act
            var result = await _sut.GetFromCacheAsync<EmulatorReleaseInfo>(fileName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFromCacheAsync_WhenJsonIsCorrupted_ShouldLogWarningAndReturnDefault()
        {
            // Arrange
            var fileName = "corrupted.json";
            var filePath = Path.Combine(_fakeCacheFolder, fileName);
            _fileSystem.AddFile(filePath, new MockFileData("{ invalid json }"));

            // Act
            var result = await _sut.GetFromCacheAsync<EmulatorReleaseInfo>(fileName);

            // Assert
            Assert.Null(result);

            _logger.Received(1).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(state => state.ToString()!.Contains($"Cache file {fileName} contains invalid JSON. It may be corrupted")),
                Arg.Any<JsonException>(),
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}
