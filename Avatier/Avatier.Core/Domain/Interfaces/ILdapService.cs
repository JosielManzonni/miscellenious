using Avatier.Core.Domain.Entities;

namespace Avatier.Core.Domain.Interfaces;

public interface ILdapService
{
    Task<IEnumerable<User>> ListUsersAsync(string baseDn, string filter);
    Task UpdateAttributesAsync(string dn, Dictionary<string, string> attributes);
    Task AddUserToGroupAsync(string userDn, string groupDn);
    Task RemoveUserFromGroupAsync(string userDn, string groupDn);
    Task ChangePasswordAsync(string userDn, string newPassword);
    Task<bool> AuthenticateAsync(string dn, string password);
}
