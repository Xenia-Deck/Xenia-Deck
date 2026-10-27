using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.IO.Abstractions.TestingHelpers;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class XeniaManagerServiceTests
    {
        private readonly XeniaManagerService _sut;
        private readonly MockFileSystem _fileSystem;
        private readonly IXeniaReleaseProviderService _xeniaReleaseProviderService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IArchiveDownloadService _archiveDownloadService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<XeniaManagerService> _logger;
        private const string _fakeEmulatorsFolderPath = @"C:\\Emulators";

        public XeniaManagerServiceTests()
        {
            _fileSystem = new MockFileSystem();
            _xeniaReleaseProviderService = Substitute.For<IXeniaReleaseProviderService>();
            _appSettingsService = Substitute.For<IAppSettingsService>();
            _archiveDownloadService = Substitute.For<IArchiveDownloadService>();
            _internalFoldersService = Substitute.For<IInternalFoldersService>();
            _logger = Substitute.For<ILogger<XeniaManagerService>>();

            _sut = new(_fileSystem,
                _xeniaReleaseProviderService,
                _appSettingsService,
                _archiveDownloadService,
                _internalFoldersService,
                _logger);
        }

        [Fact]
        public void IsInstalled_WhenFileExists_ReturnsFalse()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);
            _fileSystem.AddFile($@"{_fakeEmulatorsFolderPath}\Xenia Canary\xenia_canary.exe", null);

            // Act
            var result = _sut.IsInstalled(XeniaVersions.Canary);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInstalled_WhenEmulatorsFolderDoesntExist_ReturnsFalse()
        {
            // Arrage

            // Act
            var result = _sut.IsInstalled(XeniaVersions.Canary);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsInstalled_WhenFileDoesntExist_ReturnsFalse()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);
            _fileSystem.AddDirectory($@"{_fakeEmulatorsFolderPath}\Xenia Canary");

            // Act
            var result = _sut.IsInstalled(XeniaVersions.Canary);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetEmulatorFolderPath_WhenCalled_ReturnsEmulatorFolderPath()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);

            // Act
            var result = _sut.GetEmulatorFolderPath(XeniaVersions.Canary);

            // Assert
            Assert.Equal($@"{_fakeEmulatorsFolderPath}\Xenia Canary", result);
        }

        [Fact]
        public async Task InstallAsync_WhenExceptionThrown_RethrowException()
        {
            // Arrange
            _xeniaReleaseProviderService.GetLatestReleaseAsync(XeniaVersions.Canary).ThrowsAsync<Exception>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => { await _sut.InstallAsync(XeniaVersions.Canary); });
        }

        [Fact]
        public async Task InstallAsync_WhenEmulatorDataExists_DownloadsAndExtracts()
        {
            // Arrange
            var fakeData = new EmulatorReleaseInfo("xenia_canary", "https://example.com");
            var fakeAppSettings = new AppSettings(XeniaVersions.AllXeniaVersions.Select(x => new XeniaVersionSettings() { XeniaVersionDefinition = x }).ToList());

            _xeniaReleaseProviderService.GetLatestReleaseAsync(XeniaVersions.Canary).Returns(fakeData);
            _appSettingsService.LoadAsync().Returns(fakeAppSettings);

            // Act
            await _sut.InstallAsync(XeniaVersions.Canary);

            // Assert
            await _archiveDownloadService.Received(1)
                .DownloadAndExtractAsync(fakeData.DownloadUrl, Arg.Any<string>());
        }

        [Fact]
        public async Task InstallAsync_WhenLatestEmulatorInstalled_ReturnsEarlyWithoutDownloading()
        {
            // Arrange
            var fakeAppSettings = new AppSettings(XeniaVersions.AllXeniaVersions.Select(x => new XeniaVersionSettings() { XeniaVersionDefinition = x }).ToList());

            _xeniaReleaseProviderService.GetLatestReleaseAsync(XeniaVersions.Canary).ReturnsNull();
            _appSettingsService.LoadAsync().Returns(fakeAppSettings);
            _fileSystem.AddFile($@"{_fakeEmulatorsFolderPath}\Xenia Canary\xenia_canary.exe", null);
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);

            // Act
            await _sut.InstallAsync(XeniaVersions.Canary);

            // Assert
            await _archiveDownloadService.DidNotReceive()
                .DownloadAndExtractAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task UninstallAsync_WhenCalled_DeletesEmulatorFolder()
        {
            // Arrange
            var fakeAppSettings = new AppSettings(XeniaVersions.AllXeniaVersions.Select(x => new XeniaVersionSettings() { XeniaVersionDefinition = x }).ToList());
            _fileSystem.AddDirectory($@"{_fakeEmulatorsFolderPath}\Xenia Canary");

            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);
            _appSettingsService.LoadAsync().Returns(fakeAppSettings);

            // Act
            await _sut.UninstallAsync(XeniaVersions.Canary);

            // Assert
            Assert.False(_fileSystem.Directory.Exists($@"{_fakeEmulatorsFolderPath}\Xenia Canary"));
        }
    }
}
