using Avatier.Core.Domain.Interfaces;
using Avatier.Cli.UseCases;

public static class ActionRouter
{
    public static AbstractUseCases Create(
        string[] args,
        ILdapService ldap,
        ILogger logger)
    {
        if (args.Length == 0)
            throw new ArgumentException("No command specified.");

        return args[0] switch
        {
            "list-users" => new ListUsers(ldap, logger),
            "update-user" => new UpdateAttributes(ldap, logger),
            "add-to-group" => new AddUserToGroup(ldap, logger),
            "remove-from-group" => new RemoveUserFromGroup(ldap, logger),
            "change-password" => new ChangePassword(ldap, logger),

            _ => throw new ArgumentException($"Unknown command: {args[0]}")
        };
    }
}
