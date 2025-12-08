using Avatier.Tests.Mocks;
using Avatier.Cli.UseCases;
using FluentAssertions;
namespace Avatier.Tests.UseCases
{
    public class AuthenticationTests
    {
        [Fact]
        public async Task Command_Fails_When_Authentication_Fails()
        {
            var ldap = new FakeLdapService { AuthenticateResult = false };
            var logger = new FakeLogger();

            var command = new ListUsers(ldap, logger);

            var args = new[]
            {
            "list-users",
            "--bind-dn", "cn=invalid",
            "--bind-password", "wrong",
            "--base-dn", "dc=example,dc=org"
        };

            var exitCode = await command.RunAsync(args);

            Assert.Equal(2, exitCode);
            Assert.Contains(logger.Messages, m => m.Contains("authentication"));
        }
    }
}
