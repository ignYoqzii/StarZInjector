using System.IO;

namespace StarZInjector.Classes
{
    // This class handles the Config.txt file
    public static class ConfigManager
    {
        // Default values for the config
        private static readonly Dictionary<string, object> DefaultSettings = new()
        {
            { "DiscordRPC", false },
            { "DiscordRPCStatus", "In the Injector" },
            { "Theme", "Light" },
            { "ExeName", string.Empty },
            { "DllPath", string.Empty },
            { "InjectionMethod", "LoadLibraryA" },
            { "AutoInject", false },
            { "InjectionDelay", 5 }
        };

        private static readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StarZ Injector", "Config.txt");
        private static readonly Dictionary<string, object> settings = new(DefaultSettings);

        static ConfigManager()
        {
            if (!File.Exists(configFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath)!);
                File.Create(configFilePath).Close();
                WriteSettingsToFile(DefaultSettings);
            }
            else
            {
                LoadSettingsFromFile();
            }
        }

        private static void WriteSettingsToFile(Dictionary<string, object> settingsToWrite)
        {
            using StreamWriter writer = new(configFilePath);
            foreach (var kvp in settingsToWrite)
            {
                writer.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
        }

        private static void LoadSettingsFromFile()
        {
            if (!File.Exists(configFilePath)) return;

            using StreamReader reader = new(configFilePath);
            string line;
            while ((line = reader.ReadLine()!) != null)
            {
                var parts = line.Split('=').Select(part => part.Trim()).ToArray();
                if (parts.Length == 2 && settings.ContainsKey(parts[0]))
                {
                    var key = parts[0];
                    var value = parts[1];

                    settings[key] = ConvertValue(settings[key].GetType(), value);
                }
            }
        }

        private static object ConvertValue(Type targetType, string value)
        {
            if (targetType == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (targetType == typeof(int))
            {
                return int.Parse(value);
            }
            if (targetType == typeof(string))
            {
                return value;
            }
            throw new InvalidOperationException($"Unsupported type {targetType}");
        }

        private static void UpdateSetting(string key, object newValue)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = newValue;
                WriteSettingsToFile(settings);
            }
        }

        public static bool GetDiscordRPC() => (bool)settings["DiscordRPC"];
        public static string GetDiscordRPCStatus() => (string)settings["DiscordRPCStatus"];
        public static string GetTheme() => (string)settings["Theme"];
        public static string GetExeName() => (string)settings["ExeName"];
        public static string GetDllPath() => (string)settings["DllPath"];
        public static string GetInjectionMethod() => (string)settings["InjectionMethod"];
        public static bool GetAutoInject() => (bool)settings["AutoInject"];
        public static int GetInjectionDelay() => (int)settings["InjectionDelay"];

        public static void SetTheme(string newTheme) => UpdateSetting("Theme", newTheme);
        public static void SetDiscordRPC(bool newvalue) => UpdateSetting("DiscordRPC", newvalue);
        public static void SetDiscordRPCStatus(string newIdleStatus) => UpdateSetting("DiscordRPCStatus", newIdleStatus);
        public static void SetExeName(string newExeName) => UpdateSetting("ExeName", newExeName);
        public static void SetDllPath(string newDllPath) => UpdateSetting("DllPath", newDllPath);
        public static void SetInjectionMethod(string newInjectionMethod) => UpdateSetting("InjectionMethod", newInjectionMethod);
        public static void SetAutoInject(bool newvalue) => UpdateSetting("AutoInject", newvalue);
        public static void SetInjectionDelay(int newDelay) => UpdateSetting("InjectionDelay", newDelay);
    }
}