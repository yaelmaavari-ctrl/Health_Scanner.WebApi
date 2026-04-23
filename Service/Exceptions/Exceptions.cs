
namespace Service.Exceptions
{
    public class NotFoundException(string message) : Exception(message)
    {
    }

    public class BadRequestException(string message) : Exception(message)
    {
    }

    public class ConflictException(string message) : Exception(message)
    {
    }

    public class UnauthorizedException(string message) : Exception(message)
    {
    }

    public class ExternalServiceException(string message, Exception? innerException = null)
    : Exception(message, innerException)
    {
    }

    public class UnsupportedLanguageException(string message) : Exception(message)
    {
    }
}
