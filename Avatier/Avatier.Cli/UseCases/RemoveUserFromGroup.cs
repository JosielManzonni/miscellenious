using Avatier.Cli.UseCases;
using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Domain.Models;

namespace Avatier.Cli.UseCases;

public class RemoveUserFromGroup : AbstractUseCases
{
    public RemoveUserFromGroup(ILdapService ldap, ILogger logger) : base(ldap, logger)
    {
    }

    override public async Task<int> ExecuteAsync(string[] args)
    {
        var userDn = GetArg(args, "--user-dn");
        var groupDn = GetArg(args, "--group-dn");

        EnsureNotNull(
            (userDn, "--user-dn"),
            (groupDn, "--group-dn")
        );

        await _ldap.RemoveUserFromGroupAsync(userDn!, groupDn!);

        Console.WriteLine("User removed from group.");
        return ProcessExitCode.Success.Code;
    }
}
