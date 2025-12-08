namespace Avatier.Core.Domain.Interfaces
{
    public interface ILdapConnectionFactory
    {
        ISimpleLdapConnection Create();
    }
}