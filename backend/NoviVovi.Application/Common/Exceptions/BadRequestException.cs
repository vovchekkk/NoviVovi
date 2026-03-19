namespace NoviVovi.Application.Common.Exceptions;

public class BadRequestException(string message) : ApplicationException(message);