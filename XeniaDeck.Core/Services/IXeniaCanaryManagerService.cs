namespace XeniaDeck.Core.Services
{
    public interface IXeniaCanaryManagerService
    {
        /// <summary>
        /// Gets the folder path of the emulator
        /// </summary>
        /// <returns> Emulator folder path </returns>
        string GetEmulatorFolderPath();

        /// <summary>
        /// Installs the emulator to disk
        /// </summary>
        /// <returns> The task object representing the asynchronous operation </returns>
        Task InstallXeniaCanaryAsync();

        /// <summary>
        /// Checks if the emulator is installed on disk
        /// </summary>
        /// <returns> Whether the emulator is installed or not </returns>
        bool IsXeniaCanaryInstalled();

        /// <summary>
        /// Removes the emulator from disk
        /// </summary>
        void UninstallXeniaCanary();
    }
}