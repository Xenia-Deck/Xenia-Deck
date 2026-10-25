using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public class XeniaManagerService : IXeniaManagerService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IXeniaReleaseProviderService _xeniaReleaseProvider;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IArchiveDownloadService _archiveDownloadService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<XeniaManagerService> _logger;

        public XeniaManagerService(IFileSystem fileSystem, IXeniaReleaseProviderService xeniaReleaseProvider, IAppSettingsService appSettingsService, IArchiveDownloadService archiveDownloadService, IInternalFoldersService internalFoldersService, ILogger<XeniaManagerService> logger)
        {
            _fileSystem = fileSystem;
            _xeniaReleaseProvider = xeniaReleaseProvider;
            _appSettingsService = appSettingsService;
            _archiveDownloadService = archiveDownloadService;
            _internalFoldersService = internalFoldersService;
            _logger = logger;
        }

        public string GetEmulatorFolderPath(XeniaVersionDefinition xeniaVersionDefinition)
        {
            return _fileSystem.Path.Combine(_internalFoldersService.GetEmulatorsFolderPath(), xeniaVersionDefinition.FolderName);
        }

        public string GetExecutablePath(XeniaVersionDefinition xeniaVersionDefinition)
        {
            return _fileSystem.Path.Combine(GetEmulatorFolderPath(xeniaVersionDefinition), xeniaVersionDefinition.ExecutableName);
        }

        public bool IsInstalled(XeniaVersionDefinition xeniaVersionDefinition)
        {
            return _fileSystem.File.Exists(GetExecutablePath(xeniaVersionDefinition));
        }

        public async Task InstallAsync(XeniaVersionDefinition xeniaVersionDefinition)
        {
            var latestRelease = await _xeniaReleaseProvider.GetLatestReleaseAsync(xeniaVersionDefinition);

            var settings = await _appSettingsService.LoadAsync();

            var savedVersion = settings.XeniaVersions.Find(x => x.XeniaVersionDefinition.Id == xeniaVersionDefinition.Id);

            if (IsInstalled(xeniaVersionDefinition) && savedVersion!.EmulatorReleaseInfo == latestRelease) return;

            var folderPath = GetEmulatorFolderPath(xeniaVersionDefinition);

            _fileSystem.Directory.CreateDirectory(folderPath);

            await _archiveDownloadService.DownloadAndExtractAsync(latestRelease.DownloadUrl, folderPath);

            savedVersion!.EmulatorReleaseInfo = latestRelease;

            await _appSettingsService.SaveAsync(settings);
        }

        public async Task UninstallAsync(XeniaVersionDefinition version)
        {
            var settings = await _appSettingsService.LoadAsync();

            var savedVersion = settings.XeniaVersions.Find(x => x.XeniaVersionDefinition.Id == version.Id);

            var folderPath = GetEmulatorFolderPath(savedVersion!.XeniaVersionDefinition);

            if (_fileSystem.Directory.Exists(folderPath))
            {
                _fileSystem.Directory.Delete(folderPath, recursive: true);
            }

            savedVersion!.EmulatorReleaseInfo = null;

            await _appSettingsService.SaveAsync(settings);
        }
    }
}
