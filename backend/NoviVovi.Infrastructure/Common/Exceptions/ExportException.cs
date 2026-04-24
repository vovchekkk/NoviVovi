namespace NoviVovi.Infrastructure.Common.Exceptions;

public class ExportException(string message, Exception? innerException = null) 
    : InfrastructureException(message, innerException);