using Avatier.Cli.UseCases;
using Avatier.Tests.Mocks;
using FluentAssertions;


namespace Avatier.Tests.UseCases
{
    public class ListUsersCommandTests
    {
        [Fact]
        public async Task ListUsers_Succeeds_When_Authenticated()
        {
            var ldap = new FakeLdapService();
            var logger = new FakeLogger();

            var command = new ListUsers(ldap, logger);

            var args = new[]
            {
            "list-users",
            "--bind-dn", "cn=test",
            "--bind-password", "pass",
            "--base-dn", "dc=example,dc=org"
        };

            var exitCode = await command.RunAsync(args);

            Assert.Equal(0, exitCode);

        }
    }
}
