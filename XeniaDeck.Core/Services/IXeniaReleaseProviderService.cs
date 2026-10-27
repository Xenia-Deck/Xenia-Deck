using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public interface IXeniaReleaseProviderService
    {
        Task<EmulatorReleaseInfo> GetLatestReleaseAsync(XeniaVersionDefinition xeniaVersionDefinition);
    }
}