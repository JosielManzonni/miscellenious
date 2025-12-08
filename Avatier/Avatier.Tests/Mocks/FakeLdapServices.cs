using Avatier.Core.Domain.Entities;
using Avatier.Core.Domain.Interfaces;

namespace Avatier.Tests.Mocks
{
    public sealed class FakeLdapService : ILdapService
    {
        public bool AuthenticateResult { get; set; } = true;

        public Task<bool> AuthenticateAsync(string dn, string password)
            => Task.FromResult(AuthenticateResult);

        public Task<IEnumerable<User>> ListUsersAsync(string baseDn, string filter)
            => Task.FromResult<IEnumerable<User>>(new[]
            {
            new User("cn=John,dc=example,dc=org", "John", "john@test.com")
            });

        public Task UpdateAttributesAsync(string dn, Dictionary<string, string> attributes)
            => Task.CompletedTask;

        public Task AddUserToGroupAsync(string userDn, string groupDn)
            => Task.CompletedTask;

        public Task RemoveUserFromGroupAsync(string userDn, string groupDn)
            => Task.CompletedTask;

        public Task ChangePasswordAsync(string userDn, string newPassword)
            => Task.CompletedTask;
    }
}
