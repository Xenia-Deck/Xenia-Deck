using Octokit;
using XeniaDeck.Core.Models;

namespace XeniaDeck.Core.Services
{
    // TODO: make this class more generic

    public class GitHubUpdateService : IGitHubUpdateService
    {
        private readonly IGitHubClient _gitHubClient;
        private readonly ILocalCacheService _localCacheService;

        public GitHubUpdateService(IGitHubClient gitHubClient, ILocalCacheService localCacheService)
        {
            _gitHubClient = gitHubClient;
            _localCacheService = localCacheService;
        }

        public Task<EmulatorData?> GetCachedEmulatorAsync(string cacheFileName)
        {
            return _localCacheService.GetFromCacheAsync<EmulatorData>(cacheFileName);
        }

        public async Task<EmulatorData> GetLatestEmulatorAsync(long repositoryId, string fileName, string cacheFileName)
        {
            try
            {
                var latestRelease = await _gitHubClient.Repository.Release.GetLatest(repositoryId);

                var releaseAsset = latestRelease.Assets.FirstOrDefault(a => a.Name == fileName)
                    ?? throw new InvalidOperationException($"No release asset found matching platform: {fileName}");

                var latestEmulator = new EmulatorData(latestRelease.Name, releaseAsset.Name, releaseAsset.BrowserDownloadUrl);

                await _localCacheService.SaveToCacheAsync(cacheFileName, latestEmulator);

                return latestEmulator;
            }
            catch
            {
                throw;
            }
        }
    }
}
