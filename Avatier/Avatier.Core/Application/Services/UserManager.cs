using Avatier.Core.Domain.Entities;
using Avatier.Core.Domain.Interfaces;

namespace Avatier.Core.Application.Services;

public class UserManager
{
    private readonly ILdapService _ldap;
    public UserManager(ILdapService ldap) => _ldap = ldap;

    public async Task<IEnumerable<User>> ListAllUsersAsync()
=> await _ldap.ListUsersAsync("dc=example,dc=org", "(objectClass=inetOrgPerson)");
    public Task<IEnumerable<User>> ListUsersAsync(string baseDn, string filter)
        => _ldap.ListUsersAsync(baseDn, filter);

    public Task UpdateAttributesAsync(string dn, Dictionary<string, string> attributes)
        => _ldap.UpdateAttributesAsync(dn, attributes);

    public Task AddToGroupAsync(string userDn, string groupDn)
        => _ldap.AddUserToGroupAsync(userDn, groupDn);

    public Task RemoveFromGroupAsync(string userDn, string groupDn)
        => _ldap.RemoveUserFromGroupAsync(userDn, groupDn);

    public Task ChangePasswordAsync(string userDn, string newPassword)
        => _ldap.ChangePasswordAsync(userDn, newPassword);

    public Task<bool> AuthenticateAsync(string dn, string password)
        => _ldap.AuthenticateAsync(dn, password);
}