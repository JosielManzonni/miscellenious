namespace Avatier.Cli.Options;

public sealed class LdapAuthOptions
{
    public string BindDn { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
