using Microsoft.Extensions.Configuration;

namespace Library_Data
{
    public class clsLoggerData
    {
        private static string _logFileName;
        private static string _logFilePath;
        private static string _logsDirectory;

        public static void Initialize(IConfiguration configuration)
        {
            _logsDirectory = configuration["LogsFilePath"];

            if (!Directory.Exists(_logsDirectory))
                Directory.CreateDirectory(_logsDirectory);

            _logFilePath = Path.Combine(_logsDirectory, "Logs.txt");
        }

        public static void Log(string message)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logMessage = timeStamp + " " + message;
            File.AppendAllText(_logFilePath, logMessage);
        }
    }
}
