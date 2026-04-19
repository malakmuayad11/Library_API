namespace Infrastructure.Logging
{
    public interface ILogger
    {
        public void Log(string IP, string UserID, string EventType);
    }
}
