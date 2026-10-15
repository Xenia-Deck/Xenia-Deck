using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Windows.ApplicationModel;
using XeniaDeck.Services;
using AppInstance = Microsoft.Windows.AppLifecycle.AppInstance;

namespace XeniaDeck.Views
{
    public sealed partial class SettingsPage : Page
    {
        private readonly AppStateService _appStateService;
        private readonly ThemeSettingsService _themeSettingsService;
        private readonly LanguageSettingsService _languageSettingsService;

        private string AppVersion
        {
            get
            {
                try
                {
                    var version = Package.Current.Id.Version;
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                }
                catch
                {
                    return "1.0.0.0 (Unpackaged Dev Build)";
                }
            }
        }

        public SettingsPage()
        {
            InitializeComponent();
            _themeSettingsService = App.GetService<ThemeSettingsService>();
            _languageSettingsService = App.GetService<LanguageSettingsService>();
            _appStateService = App.GetService<AppStateService>();

            InitializeThemeSelection();
            InitializeLanguageSelection();
        }

        private void InitializeLanguageSelection()
        {
            var languages = _languageSettingsService.GetSupportedLanguages();
            LanguageComboBox.ItemsSource = languages;

            var savedLanguageCode = _languageSettingsService.GetSavedLanguageCode();
            var selectedItem = languages.FirstOrDefault(x => x.CultureInfo.Name == savedLanguageCode);

            LanguageComboBox.SelectedItem = selectedItem ?? languages.FirstOrDefault();
        }

        private void InitializeThemeSelection()
        {
            var savedTheme = _themeSettingsService.GetSavedTheme();

            var radioButton = ThemeRadioButtons.Items.Cast<RadioButton>().FirstOrDefault(x => x.Tag?.ToString() == savedTheme || x.Tag?.ToString() == "Default");

            if (radioButton != null)
            {
                ThemeRadioButtons.SelectedItem = radioButton;
            }
        }

        private void ThemeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeRadioButtons.SelectedItem is not RadioButton selectedRadioButton) return;
            if (selectedRadioButton.Tag is not string selectedTheme) return;

            var mainWindow = App.GetService<MainWindow>();
            if (mainWindow == null) return;

            _themeSettingsService.SaveTheme(selectedTheme);

            if (mainWindow.Content is FrameworkElement frameworkElement)
            {
                var elementTheme = _themeSettingsService.GetElementTheme();

                frameworkElement.RequestedTheme = elementTheme;
                UpdateTitleBarTheme(mainWindow.AppWindow.TitleBar, elementTheme);
            }
        }

        private static void UpdateTitleBarTheme(AppWindowTitleBar titleBar, ElementTheme selectedTheme)
        {
            if (titleBar == null) return;

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                titleBar.PreferredTheme = selectedTheme switch
                {
                    ElementTheme.Light => TitleBarTheme.Light,
                    ElementTheme.Dark => TitleBarTheme.Dark,
                    _ => TitleBarTheme.UseDefaultAppMode,
                };
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is not LanguageOption selectedLanguage) return;

            _appStateService.IsLanguageChanged = _appStateService.LanguageOption != selectedLanguage;

            _languageSettingsService.SaveAndApplyLanguage(selectedLanguage.CultureInfo.Name);
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Restart(string.Empty);
        }
    }
}
