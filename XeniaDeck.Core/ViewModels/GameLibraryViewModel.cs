using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Abstractions;
using XeniaDeck.Core.Models;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.ViewModels
{
    public partial class GameLibraryViewModel : ObservableObject
    {
        public ObservableCollection<Game> Games { get; } = [];

        private readonly IFileSystem _fileSystem;
        private readonly IDialogService _dialogService;
        private readonly IFilePickerService _filePickerService;
        private readonly IGameLibraryService _gameLibraryService;
        private readonly IXeniaCanaryManagerService _xeniaVersionManagerService;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly ILauncherService _launcherService;

        public GameLibraryViewModel(IFileSystem fileSystem, IDialogService dialogService, IFilePickerService filePickerService, IGameLibraryService gameLibraryService, IXeniaCanaryManagerService xeniaVersionManagerService, IInternalFoldersService internalFoldersService, ILauncherService launcherService)
        {
            _fileSystem = fileSystem;
            _dialogService = dialogService;
            _filePickerService = filePickerService;
            _gameLibraryService = gameLibraryService;
            _xeniaVersionManagerService = xeniaVersionManagerService;
            _internalFoldersService = internalFoldersService;
            _launcherService = launcherService;
        }

        public async Task LoadGamesAsync()
        {
            Games.Clear();

            var games = await _gameLibraryService.GetGamesAsync();

            foreach (var game in games)
            {
                Games.Add(game);
            }
        }

        [RelayCommand]
        public async Task AddGamesAsync()
        {
            var extensions = new Dictionary<string, IList<string>>
            {
                { "Xbox360 ROM", [".iso", ".xex", ".zar", ".xiso"] },
                { "All files",["*"] }
            };

            var files = await _filePickerService.PickFilesAsync(extensions);
            await AddGamesFromFilesAsync(files);
        }

        [RelayCommand]
        public async Task AddGamesFromFolderAsync()
        {
            var files = await _filePickerService.PickFilesFromFolderAsync();
            await AddGamesFromFilesAsync(files);
        }

        private async Task AddGamesFromFilesAsync(IEnumerable<string> files)
        {
            // TODO: if you await this, the game search stops, i will need to figure this out
            _dialogService.ShowLoadingAsync();

            foreach (var file in files)
            {
                var game = await _gameLibraryService.GetGameDetailsAsync(file);

                if (game is null) { return; }

                Games.Add(game);

                await _gameLibraryService.SaveGamesAsync(Games);
            }

            _dialogService.HideLoading();
        }

        [RelayCommand]
        public async Task OpenGamesFolderAsync()
        {
            await _launcherService.OpenFolderAsync(_internalFoldersService.GetGamesFolderPath());
        }

        [RelayCommand]
        public async Task DeleteGameAsync(Game game)
        {
            if (game != null)
            {
                Games.Remove(game);
                await _gameLibraryService.SaveGamesAsync(Games);
            }
        }

        [RelayCommand]
        public async Task PlayGameAsync(Game game)
        {
            if (game != null)
            {
                var filePaths = _fileSystem.Directory.GetFiles(_xeniaVersionManagerService.GetEmulatorFolderPath());

                var executablePath = filePaths.First(x => x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = executablePath,
                    Arguments = $"\"{game.FilePath}\""
                };

                using var executableProcess = Process.Start(processStartInfo);
            }
        }
    }
}
