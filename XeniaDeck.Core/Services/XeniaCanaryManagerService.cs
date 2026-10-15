using Microsoft.Extensions.Logging;
using System.IO.Abstractions;

namespace XeniaDeck.Core.Services
{
    // TODO: this class looks absolutely terrible and will need a big rework

    public class XeniaCanaryManagerService : IXeniaCanaryManagerService
    {
        private const string emulatorVersionName = "Xenia Canary";
        private const int repositoryId = 200145485;
        private const string fileName = "xenia_canary_windows.7z";
        private const string emulatorExecutableName = "xenia_canary.exe";
        private const string cacheFileName = "latestRelease.json";

        private readonly IFileSystem _fileSystem;
        private readonly IGitHubUpdateService _gitHubUpdateService;
        private readonly IArchiveDownloadService _archiveDownloadService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<XeniaCanaryManagerService> _logger;

        public XeniaCanaryManagerService(IFileSystem fileSystem, IGitHubUpdateService gitHubUpdateService, IArchiveDownloadService archiveDownloadService, IInternalFoldersService internalFoldersService, ILogger<XeniaCanaryManagerService> logger)
        {
            _fileSystem = fileSystem;
            _gitHubUpdateService = gitHubUpdateService;
            _archiveDownloadService = archiveDownloadService;
            _internalFoldersService = internalFoldersService;
            _logger = logger;
        }

        public bool IsXeniaCanaryInstalled()
        {
            try
            {
                var emulatorExecutablePath = _fileSystem.Path.Combine(GetEmulatorFolderPath(), emulatorExecutableName);

                return _fileSystem.File.Exists(emulatorExecutablePath);
            }
            catch
            {
                return false;
            }
        }

        public string GetEmulatorFolderPath()
        {
            return _fileSystem.Path.Combine(_internalFoldersService.GetEmulatorsFolderPath(), emulatorVersionName);
        }

        public string CreateEmulatorFolderPath()
        {
            try
            {
                var emulatorFolderPath = GetEmulatorFolderPath();

                _fileSystem.Directory.CreateDirectory(emulatorFolderPath);

                return emulatorFolderPath;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create emulator folder");
                throw new IOException("Failed to create emulator folder", e);
            }
        }

        public async Task CheckForUpdateAsync()
        {
            try
            {
                await _gitHubUpdateService.GetLatestEmulatorAsync(repositoryId, fileName, cacheFileName);
            }
            catch
            {
                throw;
            }
        }

        public async Task InstallXeniaCanaryAsync()
        {
            try
            {
                var emulatorData = await _gitHubUpdateService.GetCachedEmulatorAsync(cacheFileName);
                if (emulatorData == null) return;

                var emulatorFolderPath = CreateEmulatorFolderPath();

                await _archiveDownloadService.DownloadAndExtractAsync(emulatorData.DownloadUrl, emulatorFolderPath);
            }
            catch
            {
                throw;
            }
        }

        public void UninstallXeniaCanary()
        {
            var emulatorFolderPath = GetEmulatorFolderPath();

            if (!string.IsNullOrEmpty(emulatorFolderPath))
            {
                _fileSystem.Directory.Delete(emulatorFolderPath, true);
            }
        }
    }
}
