using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    // TODO: this class looks absolutely terrible and will need a big rework

    public class XeniaCanaryManagerService : IXeniaCanaryManagerService
    {
        private const string emulatorVersionName = "Xenia Canary";
        private const string emulatorExecutableName = "xenia_canary.exe";
        private const string installedReleaseCacheFileName = "installedReleaseCacheFileName.json";

        private readonly IFileSystem _fileSystem;
        private readonly IXeniaReleaseProviderService _xeniaReleaseProvider;
        private readonly ILocalCacheService _localCacheService;
        private readonly IArchiveDownloadService _archiveDownloadService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<XeniaCanaryManagerService> _logger;

        public XeniaCanaryManagerService(IFileSystem fileSystem, IXeniaReleaseProviderService xeniaReleaseProvider, ILocalCacheService localCacheService, IArchiveDownloadService archiveDownloadService, IInternalFoldersService internalFoldersService, ILogger<XeniaCanaryManagerService> logger)
        {
            _fileSystem = fileSystem;
            _xeniaReleaseProvider = xeniaReleaseProvider;
            _localCacheService = localCacheService;
            _archiveDownloadService = archiveDownloadService;
            _internalFoldersService = internalFoldersService;
            _logger = logger;
        }

        public bool IsXeniaCanaryInstalled()
        {
            var emulatorExecutablePath = _fileSystem.Path.Combine(GetEmulatorFolderPath(), emulatorExecutableName);

            return _fileSystem.File.Exists(emulatorExecutablePath);
        }

        public string GetEmulatorFolderPath()
        {
            return _fileSystem.Path.Combine(_internalFoldersService.GetEmulatorsFolderPath(), emulatorVersionName);
        }

        public async Task InstallXeniaCanaryAsync()
        {
            var latestEmulatorReleaseInfo = await _xeniaReleaseProvider.GetLatestReleaseAsync();

            var installedReleaseInfo = await _localCacheService.GetFromCacheAsync<EmulatorReleaseInfo>(installedReleaseCacheFileName);

            if (latestEmulatorReleaseInfo != null)
            {
                if (!IsXeniaCanaryInstalled() || latestEmulatorReleaseInfo != installedReleaseInfo)
                {
                    var emulatorFolderPath = GetEmulatorFolderPath();
                    _fileSystem.Directory.CreateDirectory(emulatorFolderPath);

                    await _archiveDownloadService.DownloadAndExtractAsync(latestEmulatorReleaseInfo.DownloadUrl, emulatorFolderPath);

                    await _localCacheService.SaveToCacheAsync(installedReleaseCacheFileName, latestEmulatorReleaseInfo);
                }
            }
        }

        public void UninstallXeniaCanary()
        {
            var emulatorFolderPath = GetEmulatorFolderPath();

            if (!string.IsNullOrEmpty(emulatorFolderPath))
            {
                _fileSystem.Directory.Delete(emulatorFolderPath, true);
            }

            var installedReleaseCacheFilePath = _fileSystem.Path.Combine(_internalFoldersService.LocalCacheFolderPath, installedReleaseCacheFileName);

            _fileSystem.File.Delete(installedReleaseCacheFilePath);
        }
    }
}
