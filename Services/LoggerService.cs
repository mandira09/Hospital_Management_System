
using System.Diagnostics;
namespace Hospital_Management.Services
{
    public class LoggerService
    {
        private readonly string logPath = "logs.txt";

        public void Log(string user, string operation, long milliseconds)
        {
            var message =
                $"[{DateTime.Now}] | User: {user} | Operation: {operation} | Time Taken: {milliseconds} ms";

            File.AppendAllText(logPath, message + Environment.NewLine);
        }
    }
}
