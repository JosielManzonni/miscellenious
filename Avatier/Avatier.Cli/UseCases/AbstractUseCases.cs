using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Domain.Models;

namespace Avatier.Cli.UseCases
{
    public abstract class AbstractUseCases
    {

        protected readonly ILdapService _ldap;
        protected readonly ILogger _logger;

        protected AbstractUseCases(ILdapService ldap, ILogger logger)
        {
            _ldap = ldap;
            _logger = logger;
        }
        protected string? GetArg(string[] args, string name)
        {
            var index = Array.IndexOf(args, name);

            if (index < 0 || index >= args.Length - 1)
                return null;

            return args[index + 1];
        }

        protected void EnsureNotNull(params (string? Value, string Name)[] args)
        {
            var missing = args
                .Where(a => string.IsNullOrWhiteSpace(a.Value))
                .Select(a => a.Name)
                .ToList();

            if (missing.Any())
            {
                throw new ArgumentException(
                    $"Missing required arguments: {string.Join(", ", missing)}");
            }
        }

        protected async Task AuthenticateAsync(string[] args)
        {
            var bindDn = GetArg(args, "--bind-dn");
            var password = GetArg(args, "--bind-password");

            EnsureNotNull(
                (bindDn, "--bind-dn"),
                (password, "--bind-password")
            );

            var ok = await _ldap.AuthenticateAsync(bindDn!, password!);

            if (!ok)
            {
                _logger.Error("LDAP authentication failed for {0}", bindDn);
                throw new UnauthorizedAccessException("LDAP authentication failed.");
            }

            _logger.Info("Authenticated as {0}", bindDn);
        }

        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                await AuthenticateAsync(args);
                await ExecuteAsync(args);
                return ProcessExitCode.Success.Code;
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex.Message);
                return ProcessExitCode.InvalidArguments.Code;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Warn(ex.Message);
                return ProcessExitCode.AuthenticationFailure.Code;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error: {0}", ex.Message);
                return ProcessExitCode.UnexpectedError.Code;
            }
        }

        public abstract Task<int> ExecuteAsync(string[] args);
    }
}
