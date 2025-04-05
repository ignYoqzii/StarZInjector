using Microsoft.Extensions.Primitives;
using StarZInjector.Classes;
using StarZInjector.Helpers;
using System.IO;
using Wpf.Ui.Abstractions.Controls;
using System.Diagnostics;

namespace StarZInjector.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private bool _isInjecting = false;
        private CancellationTokenSource _autoInjectCts = new();

        [ObservableProperty] public string _exeName = string.Empty;
        [ObservableProperty] public string _dllPath = string.Empty;
        [ObservableProperty] private bool _autoInject = false;
        [ObservableProperty] private int _injectionDelay = 0;
        [ObservableProperty] private string _status = "Status: Waiting for injection...";

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            ExeName = ConfigManager.GetExeName();
            DllPath = ConfigManager.GetDllPath();
            AutoInject = ConfigManager.GetAutoInject();
            InjectionDelay = ConfigManager.GetInjectionDelay();
            Status = "Status: Waiting for injection...";
            _isInitialized = true;
        }

        partial void OnExeNameChanged(string value)
        {
            ExeName = value;
            ConfigManager.SetExeName(value);
        }

        partial void OnDllPathChanged(string value)
        {
            DllPath = value;
            ConfigManager.SetDllPath(value);
        }

        partial void OnAutoInjectChanged(bool oldValue, bool newValue)
        {
            if (newValue && (string.IsNullOrEmpty(ExeName) || string.IsNullOrEmpty(DllPath)))
            {
                ShowMessage("Error", "Please provide both the executable name and the DLL path before enabling Automatic Injection.");
                AutoInject = false;
                return;
            }

            AutoInject = newValue;
            ConfigManager.SetAutoInject(newValue);

            if (AutoInject)
            {
                StartAutoInjectionLoop();
            }
            else
            {
                _autoInjectCts.Cancel();
            }
        }

        private void StartAutoInjectionLoop()
        {
            _autoInjectCts?.Cancel();
            _autoInjectCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                bool hasInjected = false;

                while (AutoInject && !_autoInjectCts.Token.IsCancellationRequested)
                {
                    if (!ProcessManager.IsProcessRunning(ExeName))
                    {
                        Status = $"Status: Waiting for {ExeName} to start...";
                        hasInjected = false;
                        await Task.Delay(1000);
                        continue;
                    }

                    if (!hasInjected)
                    {
                        Status = $"Status: Found {ExeName}. Waiting {InjectionDelay}s before injection...";
                        await Task.Delay(InjectionDelay * 1000);

                        if (!ProcessManager.IsProcessRunning(ExeName)) continue;

                        Status = $"Status: Injecting {Path.GetFileName(DllPath)} into {ExeName}...";

                        if (TryInjectDLL())
                        {
                            Status = $"Status: Injection successful !";
                            hasInjected = true;
                        }
                        else
                        {
                            ShowMessage("Error", "Injection failed. Auto Injection will be disabled.");
                            AutoInject = false;
                        }
                        await Task.Delay(1000);
                    }
                    else if (!ProcessManager.IsProcessRunning(ExeName))
                    {
                        Status = $"Status: {ExeName} closed. Waiting for restart...";
                        hasInjected = false;
                        await Task.Delay(3000);
                    }
                }

                if (!AutoInject)
                    Status = "Status: Waiting for injection...";

            }, _autoInjectCts.Token);
        }

        private bool TryInjectDLL()
        {
            return ConfigManager.GetInjectionMethod() switch
            {
                "LoadLibraryA" => LoadLibraryAInjector.Inject(DllPath, ExeName),
                "LoadLibraryW" => LoadLibraryWInjector.Inject(DllPath, ExeName),
                "LoadLibraryExA" => LoadLibraryExAInjector.Inject(DllPath, ExeName),
                "LoadLibraryExW" => LoadLibraryExWInjector.Inject(DllPath, ExeName),
                _ => false
            };
        }

        [RelayCommand]
        private void Inject()
        {
            if (_isInjecting)
            {
                ShowMessage("Error", "Injection is already in progress. Please wait.");
                return;
            }

            if (string.IsNullOrEmpty(ExeName) || string.IsNullOrEmpty(DllPath))
            {
                ShowMessage("Error", "Please provide both the executable name and the DLL path.");
                return;
            }

            Status = $"Status: Starting injection of {Path.GetFileName(DllPath)} into {ExeName}.";
            _isInjecting = true;

            bool success = TryInjectDLL();

            if (success)
            {
                Status = $"Status: Injection successful!";
                ShowMessage("Success", $"Injection of {Path.GetFileName(DllPath)} into {ExeName} completed successfully !");
            }
            else
            {
                Status = $"Status: Injection failed.";
                ShowMessage("Error", $"Failed to inject DLL into {ExeName}.");
            }

            _isInjecting = false;
            Status = "Status: Waiting for injection...";
        }

        [RelayCommand]
        private void SelectDllPath()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select DLL File",
                Filter = "DLL Files (*.dll)|*.dll|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
                DllPath = openFileDialog.FileName;
        }

        private static async void ShowMessage(string title, string content)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "OK",
                IsSecondaryButtonEnabled = false
            };
            await messageBox.ShowDialogAsync();
        }
    }
}