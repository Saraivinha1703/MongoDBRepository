namespace MongoDBRepository.Exceptions;

public class UnableToGetTenantDatabaseNameException(Exception? innerException = null) 
    : Exception("Not able to get the tenant database name through the TenantId, change its type (recommend Guid) or clone the repository and change the properties.", innerException);