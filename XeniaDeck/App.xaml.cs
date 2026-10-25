using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage;
using Octokit;
using Serilog;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using XeniaDeck.Core.Services;
using XeniaDeck.Core.ViewModels;
using XeniaDeck.Services;
using Application = Microsoft.UI.Xaml.Application;

namespace XeniaDeck
{
    public partial class App : Application
    {
        public Window? window;

        public IHost Host { get; private set; }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices");
            }

            return service;
        }

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(x =>
                    {
                        x.ClearProviders();
                    });

                    services.AddSerilog(x =>
                    {
                        x.MinimumLevel.Information()
                            .WriteTo.File(path: Path.Combine(ApplicationData.GetDefault().LocalFolder.Path, "Logs", "log-.txt"),
                                    rollingInterval: RollingInterval.Day,
                                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
                    });

                    services.AddHttpClient<IArchiveDownloadService, ArchiveDownloadService>();
                    services.AddHttpClient<IXbox360DatabaseService, Xbox360DatabaseService>(x =>
                    {
                        x.BaseAddress = new Uri("https://xenia-manager.github.io/x360db/");
                    });

                    services.AddSingleton<IGitHubClient, GitHubClient>(x => new GitHubClient(new ProductHeaderValue("xenia_manager")));
                    services.AddTransient(x =>
                        new JsonSerializerOptions()
                        {
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                        }
                    );

                    services.AddTransient<IInternalFoldersService, InternalFoldersService>(x =>
                        new InternalFoldersService(
                            x.GetRequiredService<IFileSystem>(),
                            ApplicationData.GetDefault().LocalFolder.Path,
                            ApplicationData.GetDefault().LocalCacheFolder.Path
                        )
                    );

                    services.AddTransient<IFileSystem, FileSystem>();

                    services.AddTransient<IFilePickerService, FilePickerService>();
                    services.AddTransient<IDialogService, DialogService>();
                    services.AddTransient<ILauncherService, LauncherService>();
                    services.AddTransient<ThemeSettingsService>();
                    services.AddTransient<LanguageSettingsService>();

                    services.AddSingleton<AppStateService>();

                    services.AddTransient<IGameLibraryService, GameLibraryService>();
                    services.AddTransient<ILocalCacheService, LocalCacheService>();
                    services.AddTransient<IGameFileExtensionIdentifierService, GameFileExtensionIdentifierService>();
                    services.AddTransient<IXeniaCanaryManagerService, XeniaCanaryManagerService>();
                    services.AddTransient<IXeniaReleaseProviderService, GitHubXeniaReleaseProviderService>();

                    services.AddTransient<GameLibraryViewModel>();
                    services.AddTransient<ManageXeniaViewModel>();
                    services.AddTransient<XeniaSettingsViewModel>();

                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            window = GetService<MainWindow>();

            var settingsService = GetService<ThemeSettingsService>();

            var selectedTheme = settingsService.GetElementTheme();

            if (window.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = selectedTheme;
            }

            window.Activate();

            var titleBar = window.AppWindow.TitleBar;

            if (AppWindowTitleBar.IsCustomizationSupported() && titleBar != null)
            {
                titleBar.PreferredTheme = selectedTheme switch
                {
                    ElementTheme.Light => TitleBarTheme.Light,
                    ElementTheme.Dark => TitleBarTheme.Dark,
                    _ => TitleBarTheme.UseDefaultAppMode,
                };
            }
        }
    }
}
