# CLI – LDAP Identity Management Tool

This project provides a cross-platform command-line interface (CLI) for interacting with an LDAP server.
It is built using .NET (6+), follows Clean Architecture principles, and relies on LDAP bind authentication before executing any operation.

## The CLI is designed for:

- Automation
- Scripting
- Administrative tasks
- Technical evaluation of LDAP operations

## Architecture Overview

The CLI is intentionally thin and delegates all business logic to the Core layer. ALl CLI is based on zero-trust approach where the auth is stateless and always validate

### Key characteristics

- No platform-specific dependencies
- Authentication is mandatory before any operation (zero trust architecture)
- Commands are isolated use cases
- Logging is console-based and portable
- Easy to extend with new commands

## Project Structure

```md
Cli/
├── Program.cs                # Entry point
├── ActionRouter.cs         # Command routing & execution
├── UseCases/
│   ├── Authentication.cs     # LDAP bind (login)
│   ├── ListUsers.cs          # List LDAP users
│   ├── UpdateAttributes.cs   # Update user attributes
│   ├── AddUserToGroup.cs     # Add user to group
│   └── RemoveUserFromGroup.cs# Remove user from group
└── Logging/
    └── ConsoleLogger.cs      # Cross-platform logger
```

## COnnection settings

Need to change on Program.cs to refer the correct host and port
```csharp
var ldapFactory = new LdapConnectionFactory(
    host: "35.94.192.121",
    port: 636,
    useSsl: true,
    acceptAllCertificates: true
);

var ldapService = new LdapService(ldapFactory, logger);
```

## Authentication Model

All commands require LDAP bind authentication.
### Authentication is performed using:
- Bind DN
- Password

Once authenticated, the same credentials are reused for subsequent LDAP operations during the command execution.

There is **no anonymous access**.


## Running the CLI

All commands are executed using the dotnet run command from the solution root.

- Ensure the LDAP server is reachable and credentials are valid.


Authenticate (Bind)
```bash
dotnet run --project Cli auth \
  "cn=testAccount1,dc=example,dc=org" \
  testAccount1
```

List Users
```bash
dotnet run --project Cli list-users \
  --bind-dn "cn=testAccount1,dc=example,dc=org" \
  --bind-password testAccount1 \
  --base-dn "dc=example,dc=org"
```

Add User to Group
```bash
dotnet run --project Cli add-to-group \
  --bind-dn "cn=testAccount1,dc=example,dc=org" \
  --bind-password testAccount1 \
  --user-dn "cn=John,dc=example,dc=org" \
  --group-dn "cn=Admins,dc=example,dc=org"
```
Update User Attributes
```bash
dotnet run --project Cli update-user \
  --bind-dn "cn=testAccount1,dc=example,dc=org" \
  --bind-password testAccount1 \
  --dn "cn=John,dc=example,dc=org" \
  --mail john@test.com

```

Remove User from Group
```bash
dotnet run --project Cli remove-from-group \
  --bind-dn "cn=testAccount1,dc=example,dc=org" \
  --bind-password testAccount1 \
  --user-dn "cn=John,dc=example,dc=org" \
  --group-dn "cn=Admins,dc=example,dc=org"
```

Change user password
```bash
dotnet run --project Cli change-password \
  --bind-dn "cn=testAccount1,dc=example,dc=org" \
  --bind-password testAccount1 \
  --user-dn "cn=John,dc=example,dc=org" \
  --new-password "StrongPassword123"

```

## Extending the CLI

To add a new command:

- Create a new class under UseCases/
- Implement the required logic
- Register the command in ActionRouter.cs
- No changes to Core or Infrastructure layers are required.

## Notes for Evaluators
- The CLI intentionally contains no LDAP logic
- All LDAP operations are performed via ILdapService
### The design prioritizes:
- Testability
- Separation of concerns
- Cross-platform compatibility