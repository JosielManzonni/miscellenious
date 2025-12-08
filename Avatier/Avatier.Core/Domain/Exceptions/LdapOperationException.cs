namespace Avatier.Core.Domain.Exceptions;

public class LdapOperationException : Exception
{
    
    public LdapOperationException(string message, Exception? inner = null)
        : base(message, inner)
    {
        
    }
}
