using Avatier.Core.Domain.Interfaces;
using Avatier.Core.Infra.Ldap;
using Avatier.Core.Infra.Logging;
using Avatier.Infra.Ldap;
using Microsoft.Extensions.DependencyInjection;


class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        ILogger logger = new ConsoleLogger();

        ILdapConnectionFactory factory =
            new LdapConnectionFactory(
                host: "192.168.10.150",
                port: 389,
                useSsl: false,
                acceptAllCertificates: true
            );

        ILdapService ldapService = new LdapService(factory, logger);

        var app = ActionRouter.Create(args, ldapService, logger);
        await app.RunAsync(args);
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  auth <dn> <password>");
        Console.WriteLine("  list-users <baseDn> <filter>");
        Console.WriteLine("  update <dn> key=value");
        Console.WriteLine("  add-group <userDn> <groupDn>");
        Console.WriteLine("  remove-group <userDn> <groupDn>");
    }
}