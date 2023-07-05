using System;

namespace WikipediaMetric
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    internal class Logger
    {
        private static string _instanceName;
        private Logger(string instanceName)
        {
            _instanceName = instanceName;
        }
        private static Logger _instance;
        public static Logger GetLogger(string instanceName)
        {
            _instance ??= new Logger(instanceName);
            return _instance;
        }

        private static void Log(LogLevel level, object message)
        {
            var stringMessage = message.ToString();
            string logEntry = $"{DateTime.Now} - [{level}] - {stringMessage}";

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

        public void Info(object message)
        {
            Log(LogLevel.Info, message);
        }

        public void Warning(object message)
        {
            Log(LogLevel.Warning, message);
        }

        public void Error(object message)
        {
            Log(LogLevel.Error, message);
        }
    }
}
