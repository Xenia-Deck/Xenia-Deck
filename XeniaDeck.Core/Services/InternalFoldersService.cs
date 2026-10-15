using System.IO.Abstractions;

namespace XeniaDeck.Core.Services
{
    // TODO: i'm starting to see less and less reasons to have this class but we'll see

    public class InternalFoldersService : IInternalFoldersService
    {
        public string LocalFolderPath { get; }
        public string LocalCacheFolderPath { get; }

        private readonly IFileSystem _fileSystem;

        public InternalFoldersService(IFileSystem fileSystem, string localFolder, string localCacheFolder)
        {
            _fileSystem = fileSystem;
            LocalFolderPath = localFolder;
            LocalCacheFolderPath = localCacheFolder;
        }

        public string GetConfigFolderPath()
        {
            return EnsureDirectory(_fileSystem.Path.Combine(LocalFolderPath, "Config"));
        }

        public string GetEmulatorsFolderPath()
        {
            return EnsureDirectory(_fileSystem.Path.Combine(LocalFolderPath, "Emulators"));
        }

        public string GetGamesFolderPath()
        {
            return EnsureDirectory(_fileSystem.Path.Combine(LocalFolderPath, "Games"));
        }

        public string GetDatabaseFolderPath()
        {
            return EnsureDirectory(_fileSystem.Path.Combine(LocalCacheFolderPath, "Database"));
        }

        private string EnsureDirectory(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
            {
                _fileSystem.Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
