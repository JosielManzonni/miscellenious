using Avatier.Core.Domain.Interfaces;
using System.DirectoryServices.Protocols;

namespace Avatier.Core.Infra.Ldap;

public class LdapConnectionFactory: ILdapConnectionFactory
{
    public string Host { get; }
    public int Port { get; }
    public bool UseSsl { get; }
    public bool AcceptAllCertificates { get; }

    public LdapConnectionFactory(string host, int port = 636, bool useSsl = true, bool acceptAllCertificates = false)
    {
        Host = host;
        Port = port;
        UseSsl = useSsl;
        AcceptAllCertificates = acceptAllCertificates;
    }

    public ISimpleLdapConnection Create()
    {
        var identifier = new LdapDirectoryIdentifier(Host, Port, false, false);
        var conn = new LdapConnection(identifier)
        {
            AuthType = AuthType.Basic,
            Timeout = TimeSpan.FromSeconds(30)
        };

        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = UseSsl;

        if (UseSsl && AcceptAllCertificates)
            conn.SessionOptions.VerifyServerCertificate += (c, cert) => true;

        return new LdapConnectionAdapter(conn);
    }
}
