public enum LogLevel
{
    Info,
    Warning,
    Error
}

public class Logger
{
    public void Log(LogLevel level, string message)
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

    public void LogInfo(string message)
    {
        Log(LogLevel.Info, message);
    }

    public void LogWarning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }
}
