using Avatier.Core.Domain.Dto;
using Avatier.Core.Domain.Entities;
using Avatier.Core.Domain.Exceptions;
using Avatier.Core.Domain.Interfaces;
using System.DirectoryServices.Protocols;

namespace Avatier.Infra.Ldap
{
    public class LdapService : ILdapService
    {
        private readonly ILdapConnectionFactory _factory;
        private readonly ILogger _logger;

        private string? _serviceDn;
        private string? _servicePassword;

        public LdapService(ILdapConnectionFactory factory, ILogger logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> AuthenticateAsync(string dn, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var conn = _factory.Create();
                    conn.Bind(dn, password);
                    _serviceDn = dn;
                    _servicePassword = password;

                    _logger.Info("Authentication succeeded for {0}", dn);
                    return true;
                }
                catch (LdapException lex)
                {
                    _logger.Warn("Authentication failed for {0}: {1}", dn, lex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.Error("Unexpected error during LDAP authenticate for {0}: {1}", dn, ex.Message);
                    return false;
                }
            });
        }

        private ISimpleLdapConnection CreateServiceBoundConnection()
        {
            if (string.IsNullOrEmpty(_serviceDn) || string.IsNullOrEmpty(_servicePassword))
                throw new LdapAuthenticationException("Service is not authenticated. Call AuthenticateAsync first.");

            var conn = _factory.Create();
            conn.Bind(_serviceDn, _servicePassword);
            return conn;
        }

        public async Task<IEnumerable<User>> ListUsersAsync(string baseDn, string filter)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var conn = CreateServiceBoundConnection();

                    var attrs = new[] { "cn", "mail", "uid", "givenName", "sn" };

                    IEnumerable<LdapEntryDto> entries = conn.Search(baseDn, filter, attrs);

                    var users = entries
                        .Select(e =>
                        {
                            e.Attributes.TryGetValue("cn", out var cn);
                            e.Attributes.TryGetValue("mail", out var mail);
                            e.Attributes.TryGetValue("givenName", out var givenName);

                            var given = string.IsNullOrEmpty(givenName) ? cn : givenName;

                            return new User(
                                e.DistinguishedName,
                                given ?? string.Empty,
                                mail ?? string.Empty
                            );
                        })
                        .ToList();

                    _logger.Info("ListUsersAsync found {0} entries", users.Count);
                    return (IEnumerable<User>)users;
                }
                catch (LdapAuthenticationException) 
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error("Error listing users: {0}", ex.Message);
                    throw new LdapOperationException("Failed to list users.", ex);
                }
            });
        }

        
        public async Task UpdateAttributesAsync(string dn, Dictionary<string, string> attributes)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var conn = CreateServiceBoundConnection();

                    var mods = attributes.Select(kv =>
                    {
                        var mod = new DirectoryAttributeModification
                        {
                            Name = kv.Key,
                            Operation = DirectoryAttributeOperation.Replace
                        };
                        mod.Add(kv.Value);
                        return mod;
                    }).ToArray();

                    var request = new ModifyRequest(dn, mods);
                    conn.Modify(request);

                    _logger.Info("Updated attributes for {0}", dn);
                }
                catch (LdapAuthenticationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed updating attributes for {0}: {1}", dn, ex.Message);
                    throw new LdapOperationException("Failed to update attributes.", ex);
                }
            });
        }


        public async Task AddUserToGroupAsync(string userDn, string groupDn)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var conn = CreateServiceBoundConnection();

                    var mod = new DirectoryAttributeModification
                    {
                        Name = "member",
                        Operation = DirectoryAttributeOperation.Add
                    };
                    mod.Add(userDn);

                    var req = new ModifyRequest(groupDn, mod);
                    conn.Modify(req);

                    _logger.Info("Added {0} to group {1}", userDn, groupDn);
                }
                catch (LdapAuthenticationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to add user {0} to group {1}: {2}", userDn, groupDn, ex.Message);
                    throw new LdapOperationException("Failed to add user to group.", ex);
                }
            });
        }

        public async Task RemoveUserFromGroupAsync(string userDn, string groupDn)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var conn = CreateServiceBoundConnection();

                    var mod = new DirectoryAttributeModification
                    {
                        Name = "member",
                        Operation = DirectoryAttributeOperation.Delete
                    };
                    mod.Add(userDn);

                    var req = new ModifyRequest(groupDn, mod);
                    conn.Modify(req);

                    _logger.Info("Removed {0} from group {1}", userDn, groupDn);
                }
                catch (LdapAuthenticationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to remove user {0} from group {1}: {2}", userDn, groupDn, ex.Message);
                    throw new LdapOperationException("Failed to remove user from group.", ex);
                }
            });
        }


        public async Task ChangePasswordAsync(string userDn, string newPassword)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var conn = CreateServiceBoundConnection();

                    var mod = new DirectoryAttributeModification
                    {
                        Name = "userPassword",
                        Operation = DirectoryAttributeOperation.Replace
                    };
                    mod.Add(newPassword);

                    var req = new ModifyRequest(userDn, mod);
                    conn.Modify(req);

                    _logger.Info("Password changed for {0}", userDn);
                }
                catch (LdapAuthenticationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to change password for {0}: {1}", userDn, ex.Message);
                    throw new LdapOperationException("Failed to change password.", ex);
                }
            });
        }
    }
}
