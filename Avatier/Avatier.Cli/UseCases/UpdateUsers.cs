using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Domain.Models;

namespace Avatier.Cli.UseCases;

public class UpdateAttributes: AbstractUseCases
{
    public UpdateAttributes(ILdapService ldap, ILogger logger) : base(ldap, logger)
    {
    }

    override public async Task<int> ExecuteAsync(string[] args)
    {
        var dn = GetArg(args, "--dn");
        var name = GetArg(args, "--given-name");
        var email = GetArg(args, "--email");

        EnsureNotNull((dn, "--dn"));

        var attributes = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(name))
            attributes["givenName"] = name;

        if (!string.IsNullOrWhiteSpace(email))
            attributes["mail"] = email;

        if (!attributes.Any())
            throw new ArgumentException("No attributes provided.");

        await _ldap.UpdateAttributesAsync(dn!, attributes);

        Console.WriteLine("Attributes updated successfully.");
        return ProcessExitCode.Success.Code;
    }
}
