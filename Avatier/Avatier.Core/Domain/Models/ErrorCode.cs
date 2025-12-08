using System;
using System.Collections.Generic;
namespace Avatier.Core.Domain.Models
{
    public sealed class ProcessExitCode
    {
        public static readonly ProcessExitCode Success = new ProcessExitCode(0, "Success");
        public static readonly ProcessExitCode InvalidArguments = new ProcessExitCode(1, "Invalid arguments");
        public static readonly ProcessExitCode AuthenticationFailure = new ProcessExitCode(2, "Authentication failure");
        public static readonly ProcessExitCode UnexpectedError = new ProcessExitCode(99, "Unexpected error");

        
        public int Code { get; }
        public string Meaning { get; }

        private ProcessExitCode(int code, string meaning)
        {
            Code = code;
            Meaning = meaning;
        }

        public override string ToString() => Meaning;
    }
}
