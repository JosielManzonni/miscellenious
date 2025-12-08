using Avatier.Core.Domain.Interfaces;

namespace Cli.Logging;

public class ConsoleLogger : ILogger
{
    public void Info(string message, params object[] args)
        => Console.WriteLine("[INFO] " + string.Format(message, args));

    public void Warn(string message, params object[] args)
        => Console.WriteLine("[WARN] " + string.Format(message, args));

    public void Error(string message, params object[] args)
        => Console.WriteLine("[ERROR] " + string.Format(message, args));
}
