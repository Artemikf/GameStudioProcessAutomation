using System.IO;

namespace WebApplication1.Patterns.Observer
{
    // Наблюдатель для логирования в файл
    public class LogObserver : IObserver
    {
        private string _logPath;
        private string _name = "FileLogger";

        public LogObserver(string logPath = "logs/notifications.txt")
        {
            _logPath = logPath;

            // Создаем директорию если не существует
            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Update(string message, string entityType, string action)
        {
            try
            {
                File.AppendAllText(_logPath, message + System.Environment.NewLine);
                System.Diagnostics.Debug.WriteLine($"Logged: {message}");
            }
            catch { }
        }

        public string GetName() => _name;
    }
}