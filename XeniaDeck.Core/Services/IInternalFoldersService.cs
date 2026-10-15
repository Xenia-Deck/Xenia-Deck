namespace XeniaDeck.Core.Services
{
    public interface IInternalFoldersService
    {
        string LocalCacheFolderPath { get; }
        string LocalFolderPath { get; }

        string GetConfigFolderPath();
        string GetDatabaseFolderPath();
        string GetEmulatorsFolderPath();
        string GetGamesFolderPath();
    }
}