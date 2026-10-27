namespace XeniaDeck.Core.Models
{
    public static class XeniaVersions
    {
        public static readonly XeniaVersionDefinition Canary = new(
            Id: "xenia_canary",
            DisplayName: "Xenia Canary (Recommended)",
            FolderName: "Xenia Canary",
            ExecutableName: "xenia_canary.exe",
            CacheFileName: "installedReleaseCacheFileName.json",
            GithubRepositoryId: 200145485,
            ReleaseAssetName: "xenia_canary_windows.7z"
        );

        public static IReadOnlyList<XeniaVersionDefinition> AllXeniaVersions { get; } = [Canary];
    }
}
