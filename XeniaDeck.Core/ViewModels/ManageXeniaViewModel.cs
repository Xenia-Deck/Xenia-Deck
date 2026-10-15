using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using System.Diagnostics;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.ViewModels
{
    public partial class ManageXeniaViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(XeniaCanaryStatusDescription))]
        public partial bool IsXeniaCanaryInstalled { get; set; }

        // TODO: Implement this into the UI with 2 different TextBlocks with a SwitchPresenter instead of this
        public string XeniaCanaryStatusDescription => IsXeniaCanaryInstalled ? "Installed" : "Not Installed";

        private readonly IXeniaCanaryManagerService _xeniaCanaryManagerService;
        private readonly IDialogService _dialogService;

        public ManageXeniaViewModel(IXeniaCanaryManagerService xeniaCanaryManagerService, IDialogService dialogService)
        {
            _xeniaCanaryManagerService = xeniaCanaryManagerService;
            _dialogService = dialogService;
        }

        public void RefreshInstallationStatus()
        {
            IsXeniaCanaryInstalled = _xeniaCanaryManagerService.IsXeniaCanaryInstalled();
        }

        [RelayCommand]
        public async Task CheckForUpdate()
        {
            try
            {
                await _xeniaCanaryManagerService.CheckForUpdateAsync();

                // TODO: also install the update or smth like that
            }
            catch (RateLimitExceededException e)
            {
                var remaining = e.Reset - DateTimeOffset.UtcNow;

                var timeLeft = remaining.TotalMinutes < 1
                                        ? "1 minute"
                                        : $"{Math.Ceiling(remaining.TotalMinutes)} minutes";

                var errorMessage = $"Rate limit applied, try again after {timeLeft}";

                await _dialogService.ShowErrorAsync("Error", errorMessage);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrorAsync("Error", e.Message);
            }
        }

        [RelayCommand]
        public async Task InstallXeniaCanary()
        {
            try
            {
                await _xeniaCanaryManagerService.CheckForUpdateAsync();

                await _xeniaCanaryManagerService.InstallXeniaCanaryAsync();

                var fileName = Path.Combine(_xeniaCanaryManagerService.GetEmulatorFolderPath(), "xenia_canary.exe");

                using var executableProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = fileName,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };

                if (executableProcess.Start())
                {
                    await Task.Delay(800);

                    executableProcess.Kill();
                }

                RefreshInstallationStatus();
            }
            catch (RateLimitExceededException e)
            {
                var remaining = e.Reset - DateTimeOffset.UtcNow;

                var timeLeft = remaining.TotalMinutes < 1
                                        ? "1 minute"
                                        : $"{Math.Ceiling(remaining.TotalMinutes)} minutes";

                var errorMessage = $"Rate limit applied, try again after {timeLeft}";

                await _dialogService.ShowErrorAsync("Error", errorMessage);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrorAsync("Error", e.Message);
            }
        }

        [RelayCommand]
        public async Task UninstallXeniaCanary()
        {
            try
            {
                _xeniaCanaryManagerService.UninstallXeniaCanary();

                RefreshInstallationStatus();
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrorAsync("Error", e.Message);
            }
        }
    }
}
