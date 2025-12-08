using Avatier.Core.Domain.Dto;
using System.DirectoryServices.Protocols;

namespace Avatier.Core.Domain.Interfaces;

public interface ISimpleLdapConnection : IDisposable
{
    IEnumerable<LdapEntryDto> Search(string baseDn, string filter, string[] attributes);

    void Modify(ModifyRequest request);

    void Bind(string bindDn, string password);
}

