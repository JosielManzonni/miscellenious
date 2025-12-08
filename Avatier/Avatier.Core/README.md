# Avatier.Core – LDAP Core Library

This project provides a .NET Core library for interacting with LDAP servers. It encapsulates the business logic for LDAP operations, following Clean Architecture and Domain-Driven Design principles. The library is designed to be used by various clients, such as command-line interfaces or web applications, and ensures secure, authenticated access to LDAP resources.

## The Core Library is designed for:

- Providing reusable LDAP functionality
- Enforcing authentication and authorization
- Supporting domain models and use cases
- Enabling testability and maintainability

## Architecture Overview

The Core library follows Clean Architecture, separating concerns into distinct layers:

- **Domain**: Contains entities, value objects, domain services, and interfaces.
- **Application**: Contains application services that orchestrate domain logic.
- **Infrastructure**: Contains implementations of domain interfaces, such as LDAP connections and logging.

### Key characteristics

- No platform-specific dependencies
- Authentication is mandatory for operations (zero trust architecture)
- Domain-driven design with clear separation of concerns
- Dependency injection friendly
- Easy to test with mocked interfaces
- Cross-platform compatibility

## Project Structure

```md
Core/
├── Domain/
│   ├── Dto/
│   │   └── LdapEntryDto.cs       # Data transfer objects for LDAP entries
│   ├── Exceptions/
│   │   ├── LdapAuthenticationException.cs  # Authentication-related exceptions
│   │   └── LdapOperationException.cs       # Operation-related exceptions
│   ├── Interfaces/
│   │   ├── ILdapConnectionFactory.cs       # Factory for LDAP connections
│   │   ├── ILdapService.cs                 # Interface for LDAP operations
│   │   ├── ILogger.cs                      # Logging interface
│   │   └── ISimpleLdapConnection.cs        # Simplified LDAP connection interface
│   ├── Models/
│   │   ├── ErrorCode.cs                    # Error codes
│   │   └── User.cs                         # User model
│   └── ...
├── Application/
│   └── Services/
│       ├── AuthService.cs                  # Authentication service
│       └── UserManager.cs                  # User management service
└── Infra/
    ├── Ldap/
    │   ├── LdapConnectionAdapter.cs        # LDAP connection adapter
    │   ├── LdapConnectionFactory.cs        # Factory implementation
    │   └── LdapService.cs                  # LDAP service implementation
    └── Logging/
        └── ConsoleLogger.cs                # Console logger implementation
```

## Connection Settings

To configure LDAP connections, use the `LdapConnectionFactory`:

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

All operations require LDAP bind authentication.

### Authentication is performed using:
- Bind DN
- Password

Operations are executed using the authenticated connection. There is **no anonymous access**.

## Usage

The Core library provides services that can be injected into your application:

- `ILdapService`: For performing LDAP operations like searching, updating, etc.
- `IAuthService`: For handling authentication.
- `IUserManager`: For user-related operations.

Example usage in a client application:

```csharp

var authResult = await authService.AuthenticateAsync(bindDn, password);

var users = await ldapService.SearchUsersAsync(baseDn, filter);

await ldapService.UpdateUserAsync(userDn, attributes);
```

## Extending the Core Library

To add new functionality:

- Define interfaces in the Domain layer.
- Implement business logic in Application services.
- Provide infrastructure implementations.
- Ensure all operations require authentication.

## Notes for Developers

- The Core library contains all LDAP logic and business rules.
- Interfaces allow for easy mocking and testing.
- Follow Domain-Driven Design principles for new features.
- Prioritize security, testability, and separation of concerns.


## Notes for Avatier

I was not able to close connection with provided services, then I launched a docker from osixia/openldap 

```bash
sudo docker run -p 389:389 -p 636:636 --name openldap --env LDAP_ORGANISATION="Avatier" --env LDAP_DOMAIN="avatier.com" --env LDAP_ADMIN_PASSWORD="a1010a" --detach osixia/openldap:1.5.0
```

and I conducted the validation