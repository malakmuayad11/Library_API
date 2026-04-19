using Infrastructure.Logging;
using System;

namespace Library_Data
{
    public class clsLoggerData
    {
        public static clsLoggerService Logger;

        private static readonly string _logFileName;
        private static string _logFilePath;
        private static readonly string _logsDirectory;

        static clsLoggerData()
        {
            _logFileName = "LibrarySystemLogs";
            _logsDirectory = @"C:\Logs";

            if (!Directory.Exists(_logsDirectory))
                Directory.CreateDirectory(_logsDirectory);

            _logFilePath = Path.Combine(_logsDirectory, _logFileName);
        }

        public static void Log(string message)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logMessage = timeStamp + " " + message;
            File.AppendAllText(_logFilePath, logMessage);
        }
    }
}
