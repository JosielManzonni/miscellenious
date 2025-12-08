namespace Avatier.Core.Domain.Dto;

public class LdapEntryDto
{
    public string DistinguishedName { get; set; } = string.Empty;
    public IDictionary<string, string?> Attributes { get; set; } = new Dictionary<string, string?>();

    public LdapEntryDto() { }

    public LdapEntryDto(string distinguishedName, IDictionary<string, string?> attributes)
    {
        DistinguishedName = distinguishedName;
        Attributes = attributes ?? new Dictionary<string, string?>();
    }
}
