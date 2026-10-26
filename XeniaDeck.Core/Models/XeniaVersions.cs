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

        // TODO: Don't make this static, it caries over sht from other tests
        public static IReadOnlyList<XeniaVersionSettings> AllXeniaVersions { get; } = [new() { XeniaVersionDefinition = Canary }];
    }
}
