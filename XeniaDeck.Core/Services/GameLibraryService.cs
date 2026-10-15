using Microsoft.Extensions.Logging;
using System.Buffers.Binary;
using System.Globalization;
using System.IO.Abstractions;
using System.Text.Json;
using XboxToolkit;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    // TODO: i think this class is doing too much, might need rework

    public class GameLibraryService : IGameLibraryService
    {
        private readonly IFileSystem _fileSystem;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly IXbox360DatabaseService _xbox360DatabaseService;
        private readonly IGameFileExtensionIdentifierService _gameFileExtensionIdentifierService;
        private readonly ILogger<GameLibraryService> _logger;

        public GameLibraryService(IFileSystem fileSystem, JsonSerializerOptions jsonSerializerOptions, IInternalFoldersService internalFoldersService, IXbox360DatabaseService xbox360DatabaseService, IGameFileExtensionIdentifierService gameFileExtensionIdentifierService, ILogger<GameLibraryService> logger)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
            _internalFoldersService = internalFoldersService;
            _xbox360DatabaseService = xbox360DatabaseService;
            _gameFileExtensionIdentifierService = gameFileExtensionIdentifierService;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public async Task<IEnumerable<Game>> GetGamesAsync()
        {
            try
            {
                var gamesStorageFilePath = _fileSystem.Path.Combine(_internalFoldersService.GetConfigFolderPath(), "games.json");

                using var gamesStorageFileStream = _fileSystem.FileStream.New(gamesStorageFilePath, FileMode.OpenOrCreate);

                var games = await JsonSerializer.DeserializeAsync<IEnumerable<Game>>(gamesStorageFileStream, _jsonSerializerOptions);

                return games ?? [];
            }
            catch (Exception e)
            {
                _logger.LogError("Games could not be retrieved {exception}", e);
                return [];
            }
        }

        public async Task SaveGamesAsync(IEnumerable<Game> games)
        {
            try
            {
                var gamesStorageFilePath = _fileSystem.Path.Combine(_internalFoldersService.GetConfigFolderPath(), "games.json");

                using var gamesStorageFileStream = _fileSystem.File.Create(gamesStorageFilePath);

                await JsonSerializer.SerializeAsync(gamesStorageFileStream, games, _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError("Games could not be retrieved {exception}", e);
            }
        }

        public async Task<Game?> GetGameDetailsAsync(string filePath)
        {
            if (!_fileSystem.File.Exists(filePath)) throw new FileNotFoundException("File not found");

            var fileSignature = _gameFileExtensionIdentifierService.IdentifyFileType(filePath);

            var game = fileSignature switch
            {
                FileSignature.ConsoleSignedStfs or FileSignature.LiveSignedStfs or FileSignature.PublisherSignedStfs => await GetGameFromStfsAsync(filePath),
                FileSignature.PrototypeXboxExecutable or FileSignature.RetailXboxExecutable => await GetGameFromXexAsync(filePath),
                FileSignature.XboxIso or FileSignature.StandardIso => await GetGameFromIsoAsync(filePath),
                FileSignature.ZarArchive => throw new NotImplementedException("Zar Archives are not supported yet"),
                FileSignature.Unknown or _ => throw new NotSupportedException("File type unknown"),
            };

            return game! with { FilePath = filePath };
        }

        private async Task<Game?> GetGameFromStfsAsync(string filePath)
        {
            using var fileStream = _fileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);

            var length = Math.Min(0xA000, (int)fileStream.Length);

            var fileData = new byte[length];

            await fileStream.ReadExactlyAsync(fileData, 0, length);

            var offset = 0x0360;
            var titleId = (uint)BinaryPrimitives.ReadInt32BigEndian(fileData.AsSpan(offset));

            var simpleGameInfos = await _xbox360DatabaseService.GetGamesAsync();

            var simpleGameInfo = simpleGameInfos.FirstOrDefault(x =>
            {
                if (x.Id == titleId.ToString("X8"))
                {
                    return true;
                }
                else
                {
                    if (x.AlternativeId != null && x.AlternativeId.Length > 0)
                    {
                        if (x.AlternativeId.Any(y => y == titleId.ToString("X8")))
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            if (simpleGameInfo == null) return null;

            return new Game(uint.Parse(simpleGameInfo.Id, NumberStyles.HexNumber), false, filePath, simpleGameInfo);
        }

        private async Task<Game?> GetGameFromXexAsync(string filePath)
        {
            byte[] xexData = _fileSystem.File.ReadAllBytes(filePath);

            if (XexUtility.TryExtractXexMetaData(xexData, out var metaData))
            {
                var simpleGameInfos = await _xbox360DatabaseService.GetGamesAsync();

                var simpleGameInfo = simpleGameInfos.FirstOrDefault(x => x.Id == metaData.TitleId.ToString("X8"));

                if (simpleGameInfo == null) return null;

                return new Game(uint.Parse(simpleGameInfo.Id, NumberStyles.HexNumber), false, filePath, simpleGameInfo);
            }

            return null;
        }

        private async Task<Game?> GetGameFromIsoAsync(string filePath)
        {
            Game? gameToAdd = null;

            if (ContainerUtility.TryAutoDetectContainerType(filePath, out var containerReader))
            {
                if (containerReader == null) return null;

                if (containerReader.TryMount())
                {
                    var parentDirectotyInfo = _fileSystem.Directory.GetParent(filePath);
                    var parentDirectotyPath = parentDirectotyInfo?.FullName;

                    if (parentDirectotyPath == null) return null;

                    var temporaryFolderPath = _fileSystem.Path.Combine(parentDirectotyPath, "~temp");

                    _fileSystem.Directory.CreateDirectory(temporaryFolderPath);

                    await Task.Run(async () =>
                    {
                        // TODO: this is an absolute travesty and I wish i shouldn't have to do this, it tanks the usefulness

                        if (containerReader.TryExtractFiles(temporaryFolderPath))
                        {
                            gameToAdd = await GetGameFromXexAsync(_fileSystem.Path.Combine(temporaryFolderPath, "default.xex"));
                        }
                    });

                    _fileSystem.Directory.Delete(temporaryFolderPath, true);

                    containerReader.Dismount();
                    containerReader.Dispose();
                }
            }

            return gameToAdd;
        }
    }
}
