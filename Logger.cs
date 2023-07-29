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

        public void LoadingBar(int totalIterations, int currentIteration)
        {
            // Calculate the progress percentage
            double progressPercentage = (currentIteration / (double)totalIterations) * 100;

            // Generate the loading bar
            int barLength = 50;
            int completedBars = (int)Math.Floor(progressPercentage / (100 / (double)barLength));
            int remainingBars = barLength - completedBars;

            // Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{_instanceName}: [");

            // Print the completed
            Console.Write(new string('#', completedBars));

            // Print the remaining
            Console.Write(new string('-', remainingBars));

            Console.Write($"] {progressPercentage:F2}%\r");

            // When the task is completed, move to a new line
            if (currentIteration == totalIterations)
            {
                Console.WriteLine();
                Console.ResetColor();
            }
        }
    }
}