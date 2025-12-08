using Avatier.Core.Domain.Interfaces;
using NSubstitute;
using System.DirectoryServices.Protocols;

namespace Avatier.Tests.Mocks;



public static class FakeLdapConnectionFactory
{
    public static (ILdapConnectionFactory factory, ISimpleLdapConnection connection) Create()
    {
        var ldapConnection = Substitute.ForPartsOf<ISimpleLdapConnection>(new LdapDirectoryIdentifier("fake", 389));

        var factory = Substitute.For<ILdapConnectionFactory>();
        factory.Create().Returns(ldapConnection);

        return (factory, ldapConnection);
    }
}
