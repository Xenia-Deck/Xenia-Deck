using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Services
{
    internal class DialogService : IDialogService
    {
        private ContentDialog? _currentLoadingContentDialog;

        private XamlRoot GetCurrentXamlRoot()
        {
            var mainWindow = App.GetService<MainWindow>();

            if (mainWindow?.Content?.XamlRoot == null)
            {
                throw new InvalidOperationException("No active XamlRoot was found");
            }

            return mainWindow.Content.XamlRoot;
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            var errorDialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Ok",
                XamlRoot = GetCurrentXamlRoot(),
                DefaultButton = ContentDialogButton.Primary
            };

            await errorDialog.ShowAsync();
        }

        public async Task ShowLoadingAsync(string title = "Loading", string message = "Please wait...")
        {
            var stackPanel = new StackPanel()
            {
                Spacing = 12
            };

            var progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 40,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var textBlock = new TextBlock
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            stackPanel.Children.Add(progressRing);
            stackPanel.Children.Add(textBlock);

            _currentLoadingContentDialog = new ContentDialog()
            {
                Title = title,
                Content = stackPanel,
                XamlRoot = GetCurrentXamlRoot(),
                CloseButtonText = "",
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false
            };

            await _currentLoadingContentDialog.ShowAsync();
        }

        public void HideLoading()
        {
            _currentLoadingContentDialog?.Hide();

            _currentLoadingContentDialog = null;
        }
    }
}
