using Avatier.Core.Domain.Dto;
using Avatier.Core.Domain.Interfaces;
using Avatier.Infra.Ldap;
using Moq;
using System.DirectoryServices.Protocols;


public class LdapServiceTest
{
    private readonly Mock<ILdapConnectionFactory> _factory;
    private readonly Mock<ISimpleLdapConnection> _conn;
    private readonly Mock<ILogger> _logger;

    private readonly LdapService _service;

    public LdapServiceTest()
    {
        _factory = new Mock<ILdapConnectionFactory>();
        _conn = new Mock<ISimpleLdapConnection>();
        _logger = new Mock<ILogger>();

        _factory.Setup(f => f.Create())
                .Returns(_conn.Object);

        _service = new LdapService(_factory.Object, _logger.Object);
    }

    
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnTrue_WhenBindSucceeds()
    {
        // Arrange: Bind succeeds (no exception)
        string dn = "cn=svc,dc=example,dc=com";
        string password = "pass";

        // Act
        var result = await _service.AuthenticateAsync(dn, password);

        // Assert
        Assert.True(result);
        _conn.Verify(c => c.Bind(dn, password), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnFalse_WhenBindThrows()
    {
        string dn = "cn=wrong,dc=example,dc=com";

        _conn.Setup(c => c.Bind(dn, "x"))
             .Throws(new LdapException("Invalid credentials"));

        var result = await _service.AuthenticateAsync(dn, "x");

        Assert.False(result);
    }

    
    [Fact]
    public async Task ListUsersAsync_ShouldReturnMappedUsers()
    {
        // Step 1: authenticate so serviceDn/password are set
        await _service.AuthenticateAsync("cn=svc", "pwd");

        var entry = new LdapEntryDto(
            distinguishedName: "cn=john,dc=example,dc=com",
            attributes: new Dictionary<string, string?>
            {
                { "cn", "John Smith" },
                { "mail", "john@example.com" }
            });

        _conn.Setup(c => c.Search(
                "dc=example,dc=com",
                "(objectClass=person)",
                It.IsAny<string[]>()
            ))
            .Returns(new[] { entry });

        // Act
        var list = await _service.ListUsersAsync("dc=example,dc=com", "(objectClass=person)");

        // Assert
        var user = list.Single();
        Assert.Equal("cn=john,dc=example,dc=com", user.DistinguishedName);
        Assert.Equal("John Smith", user.GivenName);
        Assert.Equal("john@example.com", user.Email);
    }

    
    [Fact]
    public async Task UpdateAttributesAsync_ShouldSendModifyRequest()
    {
        await _service.AuthenticateAsync("cn=svc,dc=example,dc=com", "pwd");

        var attrs = new Dictionary<string, string> {
            { "givenName", "John" },
            { "sn", "Doe" }
        };

        ModifyRequest? captured = null;
        _conn.Setup(c => c.Modify(It.IsAny<ModifyRequest>()))
             .Callback<ModifyRequest>(req => captured = req);

        await _service.UpdateAttributesAsync("cn=john,dc=example,dc=com", attrs);

        Assert.NotNull(captured);
        Assert.Equal("cn=john,dc=example,dc=com", captured!.DistinguishedName);

        var mods = captured.Modifications.Cast<DirectoryAttributeModification>().ToList();
        Assert.Equal(2, mods.Count);
        Assert.Contains(mods, m => m.Name == "givenName" && m.Count > 0 && m[0]?.ToString() == "John");
        Assert.Contains(mods, m => m.Name == "sn" && m.Count > 0 && m[0]?.ToString() == "Doe");
    }

    
    [Fact]
    public async Task AddUserToGroupAsync_ShouldCallModify()
    {
        await _service.AuthenticateAsync("cn=svc", "pwd");

        ModifyRequest? captured = null;

        _conn.Setup(c => c.Modify(It.IsAny<ModifyRequest>()))
             .Callback<ModifyRequest>(req => captured = req);

        await _service.AddUserToGroupAsync(
            userDn: "cn=john,dc=example,dc=com",
            groupDn: "cn=admins,dc=example,dc=com");

        Assert.NotNull(captured);
        Assert.Equal("cn=admins,dc=example,dc=com", captured!.DistinguishedName);

        var mod = captured.Modifications[0];
        Assert.Equal("member", mod.Name);
        Assert.Equal(DirectoryAttributeOperation.Add, mod.Operation);
        Assert.Equal("cn=john,dc=example,dc=com", mod[0]);
    }

    
    [Fact]
    public async Task RemoveUserFromGroupAsync_ShouldCallModify()
    {
        await _service.AuthenticateAsync("cn=svc", "pwd");

        ModifyRequest? captured = null;

        _conn.Setup(c => c.Modify(It.IsAny<ModifyRequest>()))
             .Callback<ModifyRequest>(req => captured = req);

        await _service.RemoveUserFromGroupAsync(
            userDn: "cn=john,dc=example,dc=com",
            groupDn: "cn=admins,dc=example,dc=com");

        Assert.NotNull(captured);
        Assert.Equal("cn=admins,dc=example,dc=com", captured!.DistinguishedName);

        var mod = captured.Modifications[0];
        Assert.Equal("member", mod.Name);
        Assert.Equal(DirectoryAttributeOperation.Delete, mod.Operation);
        Assert.Equal("cn=john,dc=example,dc=com", mod[0]);
    }

    
    [Fact]
    public async Task ChangePasswordAsync_ShouldReplaceUserPassword()
    {
        await _service.AuthenticateAsync("cn=svc", "pwd");

        ModifyRequest? captured = null;

        _conn.Setup(c => c.Modify(It.IsAny<ModifyRequest>()))
             .Callback<ModifyRequest>(req => captured = req);

        await _service.ChangePasswordAsync(
            userDn: "cn=john,dc=example,dc=com",
            newPassword: "new123");

        Assert.NotNull(captured);

        var mod = captured!.Modifications[0];
        Assert.Equal("userPassword", mod.Name);
        Assert.Equal(DirectoryAttributeOperation.Replace, mod.Operation);
        Assert.Equal("new123", mod[0]);
    }
}
