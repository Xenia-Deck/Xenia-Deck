using Octokit;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    public class GitHubXeniaReleaseProviderService : IXeniaReleaseProviderService
    {
        private const int repositoryId = 200145485;
        private const string targetFileName = "xenia_canary_windows.7z";
        private readonly IGitHubClient _gitHubClient;

        public GitHubXeniaReleaseProviderService(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        public async Task<EmulatorReleaseInfo> GetLatestReleaseAsync()
        {
            var latestRelease = await _gitHubClient.Repository.Release.GetLatest(repositoryId);

            var releaseAsset = latestRelease.Assets.First(a => a.Name == targetFileName);

            return new EmulatorReleaseInfo(latestRelease.Name, releaseAsset.BrowserDownloadUrl);
        }
    }
}
