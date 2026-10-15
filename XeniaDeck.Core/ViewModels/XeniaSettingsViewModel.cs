using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Tomlyn;
using Tomlyn.Model;
using Tomlyn.Serialization;
using Tomlyn.Syntax;
using XeniaDeck.Core.Services;

namespace XeniaDeck.Core.ViewModels
{
    public class TomlSetting
    {
        public required string Key { get; set; }
        public required object Value { get; set; }
        public required string Description { get; set; }
    }

    public class TomlSection
    {
        public required string Name { get; set; }
        public ObservableCollection<TomlSetting> Settings { get; set; } = [];
    }

    // TODO: Do something custom. There are too many options that don't have user facing names, and some that need enums instead of ints or something

    public partial class XeniaSettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<TomlSection> TomlSections { get; set; } = [];

        private readonly IFileSystem _fileSystem;
        private readonly IInternalFoldersService _internalFoldersService;
        private readonly IXeniaCanaryManagerService _xeniaCanaryManagerService;

        public XeniaSettingsViewModel(IFileSystem fileSystem, IInternalFoldersService internalFoldersService, IXeniaCanaryManagerService xeniaCanaryManagerService)
        {
            _fileSystem = fileSystem;
            _internalFoldersService = internalFoldersService;
            _xeniaCanaryManagerService = xeniaCanaryManagerService;
        }

        public void LoadSettingsAsync()
        {
            if (_xeniaCanaryManagerService.IsXeniaCanaryInstalled())
            {
                ExecLoadSettingsAsync();
            }
        }

        public void ExecLoadSettingsAsync()
        {
            TomlSections.Clear();

            var path = _fileSystem.Path.Combine(_internalFoldersService.GetEmulatorsFolderPath(), @"Xenia Canary\xenia-canary.config.toml");

            using var stream = _fileSystem.FileStream.New(path, FileMode.Open);

            var tomlMetadataStore = new TomlMetadataStore();

            TomlSerializerOptions options = new()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                MetadataStore = tomlMetadataStore
            };

            if (TomlSerializer.TryDeserialize<TomlTable>(stream, out var tomlTable, options))
            {
                foreach (var (key, value) in tomlTable)
                {
                    if (value is TomlTable innerTable)
                    {
                        var section = new TomlSection
                        {
                            Name = key
                        };

                        if (tomlMetadataStore.TryGetProperties(innerTable, out var propertiesMetadata) && propertiesMetadata is not null)
                        {
                            foreach (var settingPair in innerTable)
                            {
                                var sb = new StringBuilder();

                                if (propertiesMetadata.TryGetProperty(settingPair.Key, out var propertyMetadata) && propertyMetadata is not null)
                                {
                                    if (propertyMetadata.TrailingTrivia is not null)
                                    {
                                        foreach (var item in propertyMetadata.TrailingTrivia)
                                        {
                                            if (item.Kind == TokenKind.Comment)
                                            {
                                                sb.Append(item.Text?[2..]);
                                            }
                                        }
                                    }
                                }

                                section.Settings.Add(new TomlSetting
                                {
                                    Key = settingPair.Key,
                                    Value = settingPair.Value,
                                    Description = sb.ToString()
                                });
                            }

                            TomlSections.Add(section);
                        }
                    }
                }
            }
        }
    }
}
