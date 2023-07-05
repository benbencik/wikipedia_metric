using System;

namespace WikipediaMetric
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    class Logger
    {
        private readonly string _instanceName;
        public Logger(string instanceName) => _instanceName = instanceName;

        private void Log(LogLevel level, object message)
        {
            string logEntry = $"{DateTime.Now} - [{level}] - {message}";

            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White; // Set color for Info logs
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow; // Set color for Warning logs
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red; // Set color for Error logs
                    break;
            }
            Console.WriteLine("(" + _instanceName + ")" + ": " + logEntry);  // Log to console
            Console.ResetColor(); // Reset console color
        }

        public void Info(object message) => Log(LogLevel.Info, message);

        public void Warning(object message) => Log(LogLevel.Warning, message);

        public void Error(object message) => Log(LogLevel.Error, message);
    }
}
