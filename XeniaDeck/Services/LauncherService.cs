using System;
using System.Threading.Tasks;
using Windows.System;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Services
{
    internal class LauncherService : ILauncherService
    {
        public async Task OpenFolderAsync(string folderPath)
        {
            await Launcher.LaunchFolderPathAsync(folderPath);
        }
    }
}
