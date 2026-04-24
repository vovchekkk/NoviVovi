namespace NoviVovi.Infrastructure.Common.Exceptions;

public class InfrastructureException(string message, Exception? innerException = null) 
    : Exception(message, innerException);