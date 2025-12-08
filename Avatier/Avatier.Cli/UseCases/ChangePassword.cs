
using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Domain.Models;

namespace Avatier.Cli.UseCases
{
    public class ChangePassword : AbstractUseCases
    {

        public ChangePassword(ILdapService ldap, ILogger logger)
            : base(ldap, logger)
        {
        }

        public override async Task<int> ExecuteAsync(string[] args)
        {
            var userDn = GetArg(args, "--user-dn");
            var newPassword = GetArg(args, "--new-password");

            EnsureNotNull(
                (userDn, "--user-dn"),
                (newPassword, "--new-password")
            );

            await _ldap.ChangePasswordAsync(userDn!, newPassword!);

            Console.WriteLine("Password changed successfully.");

            return ProcessExitCode.Success.Code;
        }
    }
}
