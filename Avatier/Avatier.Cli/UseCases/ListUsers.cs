using Avatier.Cli.UseCases;
using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Domain.Models;

namespace Avatier.Cli.UseCases;

public class ListUsers: AbstractUseCases
{
    
    public ListUsers(ILdapService ldap, ILogger logger): base(ldap, logger)
    {
        
    }

    public override async Task<int> ExecuteAsync(string[] args)
    {
        var baseDn = GetArg(args, "--base-dn");
        EnsureNotNull((baseDn, "--base-dn"));

        var users = await _ldap.ListUsersAsync(
            baseDn!,
            "(objectClass=person)"
        );

        foreach (var u in users)
            Console.WriteLine($"{u.GivenName} | {u.Email}");

        return ProcessExitCode.Success.Code;

    }
}
