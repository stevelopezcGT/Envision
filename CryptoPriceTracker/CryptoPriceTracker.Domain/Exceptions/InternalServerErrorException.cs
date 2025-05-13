namespace CryptoPriceTracker.Domain.Exceptions;

[Serializable]
public class InternalServerErrorException : BusinessException
{
    public InternalServerErrorException()
    {
    }

    public InternalServerErrorException(string message) : base(message)
    {
    }

    public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}