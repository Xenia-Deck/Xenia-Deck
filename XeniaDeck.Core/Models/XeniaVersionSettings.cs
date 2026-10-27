namespace XeniaDeck.Core.Models
{
    public sealed class XeniaVersionSettings
    {
        public required XeniaVersionDefinition XeniaVersionDefinition { get; init; }
        public EmulatorReleaseInfo? EmulatorReleaseInfo { get; set; }
    }
}
