namespace CryptoPriceTracker.Application.Interfaces;

public interface ICryptoPriceService
{
    Task UpdatePricesAsync();
}