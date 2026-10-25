using Microsoft.Windows.Globalization;
using Microsoft.Windows.Storage;
using System.Collections.Generic;
using System.Globalization;

namespace XeniaDeck.Services
{
    internal record LanguageOption(string DisplayName, CultureInfo CultureInfo);

    internal class LanguageSettingsService
    {
        private const string LanguageKey = "AppLanguage";

        public List<LanguageOption> GetSupportedLanguages()
        {
            List<LanguageOption> supportedLanguages = [];

            supportedLanguages.Add(new("System (Default)", new CultureInfo("")));

            foreach (var languageCode in ApplicationLanguages.ManifestLanguages)
            {
                var cultureInfo = new CultureInfo(languageCode);

                supportedLanguages.Add(new(cultureInfo.DisplayName, cultureInfo));
            }

            return supportedLanguages;
        }

        public string GetSavedLanguageCode()
        {
            return ApplicationData.GetDefault().LocalSettings.Values[LanguageKey]?.ToString() ?? string.Empty;
        }

        public void SaveAndApplyLanguage(string languageCode)
        {
            ApplicationLanguages.PrimaryLanguageOverride = string.IsNullOrEmpty(languageCode) ?
                CultureInfo.InstalledUICulture.Name :
                languageCode;

            ApplicationData.GetDefault().LocalSettings.Values[LanguageKey] = languageCode;
        }
    }
}