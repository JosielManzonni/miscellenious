namespace Avatier.Core.Domain.Exceptions;
public class LdapAuthenticationException : Exception
{
    public LdapAuthenticationException(string message, Exception? inner = null)
            : base(message, inner)
    {
    }
}