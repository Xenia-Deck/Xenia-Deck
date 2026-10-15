using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Text.Json;

namespace XeniaDeck.Core.Services
{
    // TODO: this will be made more generic and should be used more
    public class LocalCacheService : ILocalCacheService
    {
        private readonly IFileSystem _fileSystem;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<LocalCacheService> _logger;

        public LocalCacheService(IFileSystem fileSystem, JsonSerializerOptions jsonSerializerOptions, IInternalFoldersService internalFoldersService, ILogger<LocalCacheService> logger)
        {
            _fileSystem = fileSystem;
            _jsonSerializerOptions = jsonSerializerOptions;
            _internalFoldersService = internalFoldersService;
            _logger = logger;
        }

        public async Task SaveToCacheAsync<T>(string fileName, T data)
        {
            try
            {
                var filePath = _fileSystem.Path.Combine(_internalFoldersService.LocalCacheFolderPath, fileName);

                using var stream = _fileSystem.File.Create(filePath);

                await JsonSerializer.SerializeAsync(stream, data, _jsonSerializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save data to cache file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<T?> GetFromCacheAsync<T>(string fileName)
        {
            try
            {
                var filePath = _fileSystem.Path.Combine(_internalFoldersService.LocalCacheFolderPath, fileName);

                using var stream = _fileSystem.FileStream.New(filePath, FileMode.OpenOrCreate);

                return await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
            }
            catch (JsonException e)
            {
                _logger.LogWarning(e, "Cache file {FileName} contains invalid JSON. It may be corrupted", fileName);
                return default;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error retrieving data from cache file: {FileName}", fileName);
                throw;
            }
        }
    }
}
