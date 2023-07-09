using System;

namespace wikipedia_metric
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    internal class Logger
    {
        private readonly string _instanceName;
        public Logger(string instanceName) => _instanceName = instanceName;

        private void Log(LogLevel level, object message)
        {
            var logEntry = $"{DateTime.Now} - [{level}] - {message}";

            Console.ForegroundColor = level switch
            {
                LogLevel.Info => ConsoleColor.White // Set color for Info logs
                ,
                LogLevel.Warning => ConsoleColor.Yellow // Set color for Warning logs
                ,
                LogLevel.Error => ConsoleColor.Red // Set color for Error logs
                ,
                _ => Console.ForegroundColor
            };
            Console.WriteLine("(" + _instanceName + ")" + ": " + logEntry); // Log to console
            Console.ResetColor(); // Reset console color
        }

        public void Info(object message) => Log(LogLevel.Info, message);

        public void Warning(object message) => Log(LogLevel.Warning, message);

        public void Error(object message) => Log(LogLevel.Error, message);
    }
}