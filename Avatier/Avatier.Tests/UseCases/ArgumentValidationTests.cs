using Avatier.Cli.UseCases;
using Avatier.Tests.Mocks;

namespace Avatier.Tests.UseCases
{
    public class ArgumentValidationTests
    {
        [Fact]
        public async Task Missing_BaseDn_Returns_ExitCode_1()
        {
            var ldap = new FakeLdapService();
            var logger = new FakeLogger();

            var command = new ListUsers(ldap, logger);

            var args = new[]
            {
            "list-users",
            "--bind-dn", "cn=test",
            "--bind-password", "pass"
            };

            var exitCode = await command.RunAsync(args);

            Assert.Equal(1, exitCode);

        }
    }
}
