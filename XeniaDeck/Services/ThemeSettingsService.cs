using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage;

namespace XeniaDeck.Services
{
    internal class ThemeSettingsService
    {
        private const string ThemeKey = "AppTheme";

        public void SaveTheme(string theme)
        {
            ApplicationData.GetDefault().LocalSettings.Values[ThemeKey] = theme;
        }

        public string GetSavedTheme()
        {
            if (ApplicationData.GetDefault().LocalSettings.Values.TryGetValue(ThemeKey, out var value))
            {
                if (value != null)
                {
                    var savedTheme = value.ToString();

                    if (!string.IsNullOrEmpty(savedTheme))
                    {
                        return savedTheme;
                    }
                }
            }

            return string.Empty;
        }

        public ElementTheme GetElementTheme()
        {
            return GetSavedTheme() switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default,
            };
        }
    }
}
