using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Text.Json;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private const string SettingsFileName = "settings.json";

        private readonly IFileSystem _fileSystem;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<AppSettingsService> _logger;

        public AppSettingsService(IFileSystem fileSystem, IInternalFoldersService internalFoldersService, JsonSerializerOptions jsonSerializerOptions, ILogger<AppSettingsService> logger)
        {
            _fileSystem = fileSystem;
            _internalFoldersService = internalFoldersService;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        public async Task<AppSettings> LoadAsync()
        {
            var settingsFilePath = GetSettingsFilePath();

            if (!_fileSystem.File.Exists(settingsFilePath))
            {
                var defaults = new AppSettings([.. XeniaVersions.AllXeniaVersions]);

                await SaveAsync(defaults);

                return defaults;
            }

            using var stream = _fileSystem.FileStream.New(settingsFilePath, FileMode.Open, FileAccess.Read);

            var appSettings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, _jsonSerializerOptions);

            return appSettings;
        }

        public async Task SaveAsync(AppSettings appSettings)
        {
            var settingsFilePath = GetSettingsFilePath();

            using var stream = _fileSystem.File.Create(settingsFilePath);

            await JsonSerializer.SerializeAsync(stream, appSettings, _jsonSerializerOptions);
        }

        private string GetSettingsFilePath()
        {
            return _fileSystem.Path.Combine(_internalFoldersService.GetConfigFolderPath(), SettingsFileName);
        }
    }
}
