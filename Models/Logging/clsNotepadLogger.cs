using Microsoft.Extensions.Configuration;

namespace Infrastructure.Logging
{
    public class clsNotepadLogger : ILogger
    {
		private readonly IConfiguration _Configuration;
        private readonly string _LogsDirectory;
        private readonly string _LogFilePath;
        public clsNotepadLogger(IConfiguration Configuration)
        {
            _Configuration = Configuration;
            _LogsDirectory = _Configuration["LogsFilePath"];

			if (!Directory.Exists(_LogsDirectory))
                Directory.CreateDirectory(_LogsDirectory);

            _LogFilePath = Path.Combine(_LogsDirectory, "Logs.txt");
        }
        public void Log(string IP, string UserID, string EventType)
        {
			string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string LogMessage = 
                $"{timeStamp} [{EventType}] -> IP: {IP}, UserID: ${UserID}.\n";
			File.AppendAllText(_LogFilePath, LogMessage);
        }
    }
}
