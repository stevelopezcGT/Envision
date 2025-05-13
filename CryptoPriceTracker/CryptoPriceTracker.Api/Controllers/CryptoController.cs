namespace CryptoPriceTracker.Api.Controllers;

/// <summary>
/// Controller for handling cryptocurrency-related operations.
/// </summary>
[ApiController]
[Route("api/crypto")]
public class CryptoController : ControllerBase
{
    private readonly ICryptoPriceService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoController"/> class.
    /// </summary>
    /// <param name="service">The service for handling cryptocurrency price operations.</param>
    public CryptoController(ICryptoPriceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Triggers an update of cryptocurrency prices by fetching data from the CoinGecko API
    /// and saving it to the database.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPost("update-prices")]
    public async Task<IActionResult> UpdatePrices()
    {
        await _service.UpdatePricesAsync();

        return Ok("Prices updated."); // Optional: Replace with a real result message
    }

    /// <summary>
    /// Retrieves the latest cryptocurrency prices.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing a list of cryptocurrency details.</returns>
    [HttpGet("latest-prices")]
    public async Task<IActionResult> GetLatestPrices()
    {
        return Ok(await _service.GetCryptoCurrenciesAsync());
    }
}