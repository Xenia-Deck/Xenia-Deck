namespace XeniaDeck.Core.Models
{
    public sealed record XeniaVersionDefinition(
        string Id,
        string DisplayName,
        string FolderName,
        string ExecutableName,
        string CacheFileName,
        int GithubRepositoryId,
        string ReleaseAssetName
    );
}
