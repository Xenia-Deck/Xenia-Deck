using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;

namespace XeniaDeck.Services
{
    internal partial class AppStateService : ObservableObject
    {
        [ObservableProperty]
        public partial bool IsLanguageChanged { get; set; }

        public LanguageOption LanguageOption { get; init; }

        public AppStateService(LanguageSettingsService languageSettingsService)
        {
            var languageOptions = languageSettingsService.GetSupportedLanguages();

            LanguageOption = languageOptions.First(x => x.CultureInfo.Name == languageSettingsService.GetSavedLanguageCode());
        }
    }
}
