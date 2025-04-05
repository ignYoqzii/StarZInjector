using Wpf.Ui.Appearance;
using Wpf.Ui.Abstractions.Controls;
using System.Windows.Media;
using StarZInjector.Classes;
using System.IO;

namespace StarZInjector.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentApplicationTheme = ParseTheme(ConfigManager.GetTheme());

        [ObservableProperty]
        private bool _discordRPC = false;

        [ObservableProperty]
        private string _discordRPCStatus = "In the Injector";

        [ObservableProperty]
        private string _injectionMethod = "LoadLibraryA";

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            AppVersion = $"{GetAssemblyVersion()}";
            DiscordRPC = ConfigManager.GetDiscordRPC(); // bool
            DiscordRPCStatus = ConfigManager.GetDiscordRPCStatus(); // string
            InjectionMethod = ConfigManager.GetInjectionMethod(); // string

            _isInitialized = true;
        }

        private static string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        private static ApplicationTheme ParseTheme(string theme)
        {
            if (Enum.TryParse(theme, out ApplicationTheme appTheme))
            {
                return appTheme;
            }
            return ApplicationTheme.Light; // Default theme
        }

        partial void OnCurrentApplicationThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
        {
            CurrentApplicationTheme = newValue;
            ApplicationThemeManager.Apply(newValue);
            ConfigManager.SetTheme(newValue.ToString());
        }

        partial void OnDiscordRPCChanged(bool oldValue, bool newValue)
        {
            DiscordRPC = newValue;
            ConfigManager.SetDiscordRPC(newValue);
        }

        partial void OnDiscordRPCStatusChanged(string? oldValue, string newValue)
        {
            DiscordRPCStatus = newValue;
            ConfigManager.SetDiscordRPCStatus(newValue);
        }

        partial void OnInjectionMethodChanged(string? oldValue, string newValue)
        {
            InjectionMethod = newValue;
            ConfigManager.SetInjectionMethod(newValue);
        }

        [RelayCommand]
        private static void OpenAppFolder()
        {
            string appFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StarZ Injector");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = appFolderPath,
                UseShellExecute = true
            });
        }
    }
}
