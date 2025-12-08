namespace Avatier.Core.Domain.Entities;

public class User
{
    public string DistinguishedName { get; init; }
    public string? GivenName { get; set; }
    public string? Email  { get; set; }

    public User(string distinguishedName, string givenName, string email) {
        DistinguishedName = distinguishedName;
        GivenName = givenName;
        Email = email;
    }

    public User() { }
}
