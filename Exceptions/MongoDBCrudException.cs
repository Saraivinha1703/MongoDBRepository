namespace MongoDBRepository.Exceptions;

public class MongoDBCrudException(string operation, Exception? innerException = null) 
    : Exception($"There was a problem while trying to {operation} your collection", innerException);