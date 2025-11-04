# MongoDB Generic and Multi-Tenant Ready Repository

This is a MongoDB and .NET project with a generic Repository that is ready to read data from the JWT on the HTTP Authorization request header
and filter directly on the database with the tenant id, or select the tenant database depending on the user's role.

All the operations are stored in a list of commands managed by the `MongoContext` and executed by the `UnitOfWork`.

You can clone the repository, copy what you need or install it as package to test it before with:

```bash
dotnet add package CarlosSaraiva.MongoDBRepository
```
