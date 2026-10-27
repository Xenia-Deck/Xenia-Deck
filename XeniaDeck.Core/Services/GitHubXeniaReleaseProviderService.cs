using Octokit;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public class GitHubXeniaReleaseProviderService : IXeniaReleaseProviderService
    {
        private readonly IGitHubClient _gitHubClient;

        public GitHubXeniaReleaseProviderService(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        public async Task<EmulatorReleaseInfo> GetLatestReleaseAsync(XeniaVersionDefinition xeniaVersionDefinition)
        {
            var latestRelease = await _gitHubClient.Repository.Release.GetLatest(xeniaVersionDefinition.GithubRepositoryId);

            var releaseAsset = latestRelease.Assets.First(a => a.Name == xeniaVersionDefinition.ReleaseAssetName);

            return new EmulatorReleaseInfo(latestRelease.Name, releaseAsset.BrowserDownloadUrl);
        }
    }
}
