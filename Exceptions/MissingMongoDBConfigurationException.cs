namespace MongoDBRepository.Exceptions;

public class MissingMongoDBConfigurationException(string property, Exception? innerException = null)
    : Exception($"You are missing the property \"{property}\", configure it in the appsettings.json file", innerException);