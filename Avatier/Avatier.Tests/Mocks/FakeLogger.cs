using Avatier.Core.Domain.Interfaces;

namespace Avatier.Tests.Mocks
{
    public sealed class FakeLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public void Info(string message, params object[] args)
            => Messages.Add("INFO: " + string.Format(message, args));

        public void Warn(string message, params object[] args)
            => Messages.Add("WARN: " + string.Format(message, args));

        public void Error(string message, params object[] args)
            => Messages.Add("ERROR: " + string.Format(message, args));
    }
}
