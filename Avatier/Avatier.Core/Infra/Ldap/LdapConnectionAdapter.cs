using Avatier.Core.Domain.Dto;
using Avatier.Core.Domain.Interfaces;
using System.DirectoryServices.Protocols;
using System.Net;

namespace Avatier.Core.Infra.Ldap;

public class LdapConnectionAdapter : ISimpleLdapConnection
{
    private readonly LdapConnection _conn;

    public LdapConnectionAdapter(LdapConnection conn)
    {
        _conn = conn ?? throw new ArgumentNullException(nameof(conn));
    }

    public void Bind(string bindDn, string password)
    {
        if (string.IsNullOrWhiteSpace(bindDn))
            throw new ArgumentException("Bind DN cannot be null or empty");

        if (password == null)
            throw new ArgumentNullException(nameof(password));

        _conn.Credential = new NetworkCredential(bindDn, password);
        _conn.AuthType = AuthType.Basic;

        // executa o bind real
        _conn.Bind();
    }


    public IEnumerable<LdapEntryDto> Search(string baseDn, string filter, string[] attributes)
    {
        var req = new SearchRequest(baseDn, filter, SearchScope.Subtree, attributes);
        var resp = (SearchResponse)_conn.SendRequest(req);

        foreach (SearchResultEntry e in resp.Entries)
        {
            var dto = new LdapEntryDto
            {
                DistinguishedName = e.DistinguishedName,
                Attributes = new Dictionary<string, string?>()
            };

            foreach (DirectoryAttribute attr in e.Attributes.Values)
            {
                dto.Attributes[attr.Name] = attr.Count > 0 ? attr[0]?.ToString() : null;
            }

            yield return dto;
        }
    }

    public void Modify(ModifyRequest request)
    {
        _conn.SendRequest(request);
    }

    public void Dispose()
    {
        _conn?.Dispose();
    }
}
