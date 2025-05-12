using CryptoPriceTracker.Application.Dto;

namespace CryptoPriceTracker.Application.Interfaces;

public interface ICryptoPriceService
{
    Task UpdatePricesAsync();

    Task<List<CryptoCurrencyDto>> GetCryptoCurrenciesAsync();
}