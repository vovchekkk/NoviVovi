namespace NoviVovi.Application.Common.Exceptions;

public class NotFoundException(string message) : ApplicationException(message);