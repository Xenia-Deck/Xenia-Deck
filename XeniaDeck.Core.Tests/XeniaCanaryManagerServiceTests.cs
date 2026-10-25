using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.IO.Abstractions.TestingHelpers;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class XeniaCanaryManagerServiceTests
    {
        private readonly XeniaCanaryManagerService _sut;
        private readonly MockFileSystem _fileSystem;
        private readonly IXeniaReleaseProviderService _xeniaReleaseProviderService;
        private readonly ILocalCacheService _localCacheService;
        private readonly IArchiveDownloadService _archiveDownloadService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILogger<XeniaCanaryManagerService> _logger;
        private const string _fakeEmulatorsFolderPath = @"C:\\Emulators";

        public XeniaCanaryManagerServiceTests()
        {
            _fileSystem = new MockFileSystem();
            _xeniaReleaseProviderService = Substitute.For<IXeniaReleaseProviderService>();
            _localCacheService = Substitute.For<ILocalCacheService>();
            _archiveDownloadService = Substitute.For<IArchiveDownloadService>();
            _internalFoldersService = Substitute.For<IInternalFoldersService>();
            _logger = Substitute.For<ILogger<XeniaCanaryManagerService>>();

            _sut = new(_fileSystem,
                _xeniaReleaseProviderService,
                _localCacheService,
                _archiveDownloadService,
                _internalFoldersService,
                _logger);
        }

        [Fact]
        public void IsXeniaCanaryInstalled_WhenFileExists_ReturnsFalse()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);
            _fileSystem.AddFile($@"{_fakeEmulatorsFolderPath}\Xenia Canary\xenia_canary.exe", null);

            // Act
            var result = _sut.IsXeniaCanaryInstalled();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsXeniaCanaryInstalled_WhenEmulatorsFolderDoesntExist_ReturnsFalse()
        {
            // Arrage

            // Act
            var result = _sut.IsXeniaCanaryInstalled();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsXeniaCanaryInstalled_WhenFileDoesntExist_ReturnsFalse()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);
            _fileSystem.AddDirectory($@"{_fakeEmulatorsFolderPath}\Xenia Canary");

            // Act
            var result = _sut.IsXeniaCanaryInstalled();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetEmulatorFolderPath_WhenCalled_ReturnsEmulatorFolderPath()
        {
            // Arrage
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);

            // Act
            var result = _sut.GetEmulatorFolderPath();

            // Assert
            Assert.Equal($@"{_fakeEmulatorsFolderPath}\Xenia Canary", result);
        }

        [Fact]
        public async Task CheckForUpdateAsync_WhenExceptionThrown_RethrowException()
        {
            // Arrange
            _xeniaReleaseProviderService.GetLatestReleaseAsync().ThrowsAsync<Exception>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(_sut.InstallXeniaCanaryAsync);
        }

        [Fact]
        public async Task InstallXeniaCanaryAsync_WhenEmulatorDataExists_DownloadsAndExtracts()
        {
            // Arrange
            var fakeData = new EmulatorReleaseInfo("xenia", "https://example.com");

            _xeniaReleaseProviderService.GetLatestReleaseAsync().Returns(fakeData);

            // Act
            await _sut.InstallXeniaCanaryAsync();

            // Assert
            await _archiveDownloadService.Received(1)
                .DownloadAndExtractAsync(fakeData.DownloadUrl, Arg.Any<string>());
        }

        [Fact]
        public async Task InstallXeniaCanaryAsync_WhenEmulatorDataIsNull_ReturnsEarlyWithoutDownloading()
        {
            // Arrange
            _xeniaReleaseProviderService.GetLatestReleaseAsync().ReturnsNull();

            // Act
            await _sut.InstallXeniaCanaryAsync();

            // Assert
            await _archiveDownloadService.DidNotReceive()
                .DownloadAndExtractAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void UninstallXeniaCanary_WhenCalled_DeletesEmulatorFolder()
        {
            // Arrange
            _internalFoldersService.GetEmulatorsFolderPath().Returns(_fakeEmulatorsFolderPath);

            _fileSystem.AddDirectory($@"{_fakeEmulatorsFolderPath}\Xenia Canary");

            // Act
            _sut.UninstallXeniaCanary();

            // Assert
            Assert.False(_fileSystem.Directory.Exists($@"{_fakeEmulatorsFolderPath}\Xenia Canary"));
        }
    }
}
