using Avatier.Core.Domain.Interfaces;

namespace Cli.Core;

public class Authentication
{
    private readonly ILdapService _ldap;
    private readonly ILogger _logger;

    public Authentication(ILdapService ldap, ILogger logger)
    {
        _ldap = ldap;
        _logger = logger;
    }

    public async Task ExecuteAsync(string dn, string password)
    {
        var success = await _ldap.AuthenticateAsync(dn, password);
        _logger.Info(success ? "Authentication successful" : "Authentication failed");
    }
}
