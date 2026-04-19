namespace Infrastructure.Logging
{
    public class clsLoggerService
    {
        private readonly ILogger _Logger;
        public clsLoggerService(ILogger Logger) => _Logger = Logger;
        public void Log(string IP, string UserID, string EventType) => _Logger.Log(IP, UserID, EventType);
    }
}
