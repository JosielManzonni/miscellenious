using Avatier.Core.Domain.Interfaces;


namespace Avatier.Core.Application.Services;

public class AuthService
{
    private readonly ILdapService _ldap;


    public AuthService(ILdapService ldap)
    {
        _ldap = ldap;
    }


    public Task<bool> Login(string username, string password)
    {
        var bindDn = $"{username},dc=example,dc=org";
        return _ldap.AuthenticateAsync(bindDn, password);
    }
}