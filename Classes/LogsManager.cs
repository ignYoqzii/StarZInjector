using System.IO;
using System.Windows;

namespace StarZInjector.Classes
{
    public static class LogsManager
    {
        private static readonly string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StarZ Injector", "Logs");

        static LogsManager()
        {
            // Ensure log directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public static void Log(string message, string logFileName)
        {
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Handle logging errors gracefully
                MessageBox.Show($"Failed to write to log file: {ex.Message}", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
