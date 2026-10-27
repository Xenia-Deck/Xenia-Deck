using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using System.Diagnostics;
using XeniaDeck.Core.Models;
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

        private readonly IXeniaManagerService _xeniaManagerService;
        private readonly IDialogService _dialogService;

        public ManageXeniaViewModel(IXeniaManagerService xeniaManagerService, IDialogService dialogService)
        {
            _xeniaManagerService = xeniaManagerService;
            _dialogService = dialogService;
        }

        public void RefreshInstallationStatus()
        {
            // TODO: prototype, make one for every xenia version

            IsXeniaCanaryInstalled = _xeniaManagerService.IsInstalled(XeniaVersions.Canary);
        }

        [RelayCommand]
        public async Task CheckForUpdate()
        {
            try
            {
                // TODO: prototype, make one for every xenia version

                await _xeniaManagerService.InstallAsync(XeniaVersions.Canary);
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
                // TODO: prototype, make one for every xenia version

                await _xeniaManagerService.InstallAsync(XeniaVersions.Canary);

                var fileName = Path.Combine(_xeniaManagerService.GetEmulatorFolderPath(XeniaVersions.Canary), XeniaVersions.Canary.ExecutableName);

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
                await _xeniaManagerService.UninstallAsync(XeniaVersions.Canary);

                RefreshInstallationStatus();
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrorAsync("Error", e.Message);
            }
        }
    }
}
