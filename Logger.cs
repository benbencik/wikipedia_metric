

using System;

namespace WikipediaMetric
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        public static void Log(LogLevel level, string message)
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
            Console.WriteLine(logEntry);  // Log to console
            Console.ResetColor(); // Reset console color
        }

        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public static void Error(string message)
        {
            Log(LogLevel.Error, message);
        }
    }
}
