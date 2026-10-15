using System.IO.Abstractions.TestingHelpers;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.Tests
{
    public class InternalFoldersServiceTests
    {
        private readonly MockFileSystem _fileSystem;
        private readonly InternalFoldersService _sut;

        private const string localFolderPath = @"C:\XeniaDeck\LocalState";
        private const string localCacheFolderPath = @"C:\XeniaDeck\LocalCache";

        public InternalFoldersServiceTests()
        {
            _fileSystem = new MockFileSystem();
            _sut = new(_fileSystem, localFolderPath, localCacheFolderPath);
        }

        [Theory]
        [InlineData("Config", $@"{localFolderPath}\Config")]
        [InlineData("Emulators", $@"{localFolderPath}\Emulators")]
        [InlineData("Games", $@"{localFolderPath}\Games")]
        [InlineData("Database", $@"{localCacheFolderPath}\Database")]
        public void GetFolderPath_WhenCalled_ReturnsPathAndCreatesDirectory(string folderType, string expectedPath)
        {
            // Arrange

            // Act
            var folderPath = folderType switch
            {
                "Config" => _sut.GetConfigFolderPath(),
                "Emulators" => _sut.GetEmulatorsFolderPath(),
                "Games" => _sut.GetGamesFolderPath(),
                "Database" => _sut.GetDatabaseFolderPath(),
                _ => throw new ArgumentException("Invalid folder type"),
            };

            //Assert
            Assert.Equal(expectedPath, folderPath);
            Assert.True(_fileSystem.Directory.Exists(expectedPath), $"The '{folderType}' folder was not created.");
        }

        [Fact]
        public void GetConfigFolderPath_WhenDirectoryAlreadyExists_ReturnsPathWithoutError()
        {
            // Arrange
            var targetPath = $@"{localFolderPath}\Config";
            _fileSystem.Directory.CreateDirectory(targetPath); // Pre-create the folder

            // Act
            var exception = Record.Exception(_sut.GetConfigFolderPath);

            // Assert
            Assert.Null(exception);
            Assert.True(_fileSystem.Directory.Exists(targetPath));
        }
    }
}
