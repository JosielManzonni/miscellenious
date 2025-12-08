

using Avatier.Core.Domain.Interfaces;

namespace Avatier.Core.Infra.Logging;

public class ConsoleLogger : ILogger
{
   public void Info(string message, params object[] args)
        {
            Console.WriteLine($"[INFO ] {Timestamp()} - {Format(message, args)}");
        }

        public void Warn(string message, params object[] args)
        {
            Console.WriteLine($"[WARN ] {Timestamp()} - {Format(message, args)}");
        }

        public void Error(string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {Timestamp()} - {Format(message, args)}");
        }

        private static string Timestamp()
            => DateTime.UtcNow.ToString("o");

        private static string Format(string message, object[] args)
                => args == null || args.Length == 0
                    ? message
                    : string.Format(message, args);
}
