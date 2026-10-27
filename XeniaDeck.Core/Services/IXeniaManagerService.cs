using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IXeniaManagerService
    {
        string GetEmulatorFolderPath(XeniaVersionDefinition xeniaVersionDefinition);
        string GetExecutablePath(XeniaVersionDefinition xeniaVersionDefinition);
        Task InstallAsync(XeniaVersionDefinition xeniaVersionDefinition);
        bool IsInstalled(XeniaVersionDefinition xeniaVersionDefinition);
        Task UninstallAsync(XeniaVersionDefinition version);
    }
}