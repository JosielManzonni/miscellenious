using Avatier.Core.Domain.Interfaces;
using System.Collections.Generic;
using Xunit;

public class TestLogger : ILogger
{
    public List<string> Messages { get; } = new();

    public void Info(string msg, params object[] args) =>
        Messages.Add("INFO: " + string.Format(msg, args));

    public void Warn(string msg, params object[] args) =>
        Messages.Add("WARN: " + string.Format(msg, args));

    public void Error(string msg, params object[] args) =>
        Messages.Add("ERROR: " + string.Format(msg, args));
}

