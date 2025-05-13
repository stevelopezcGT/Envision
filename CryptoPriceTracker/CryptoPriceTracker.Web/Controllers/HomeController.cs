using CryptoPriceTracker.Application.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CryptoPriceTracker.Web.Controllers;

/// <summary>
/// Controller responsible for handling requests related to the home page and cryptocurrency price data.
/// </summary>
public class HomeController : Controller
{
    private readonly HttpClient _http;

    private readonly SiteSettings _siteSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="http">The HTTP client used for making API requests.</param>
    /// <param name="siteSettings">The site settings containing configuration values.</param>
    public HomeController(HttpClient http, IOptions<SiteSettings> siteSettings)
    {
        _http = http;
        _siteSettings = siteSettings.Value;
    }

    /// <summary>
    /// Serves the main view for the home page.
    /// </summary>
    /// <returns>The main view for the home page.</returns>
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("updatePrices")]
    public async Task<IActionResult> UpdatePrices()
    {
        // Make a POST request to the API endpoint for updating cryptocurrency prices.
        var resp = await _http.PostAsync($"{_siteSettings.EndPointUrl}/api/crypto/update-prices", null);
        // If the response is not successful, return an error message.
        if (!resp.IsSuccessStatusCode)
        {
            return BadRequest("Failed to update prices.");
        }
        // Return a success message.
        return Ok("Prices updated successfully.");
    }

    /// <summary>
    /// Retrieves the latest cryptocurrency prices and returns a partial view with the data.
    /// </summary>
    /// <returns>A partial view containing the list of cryptocurrency prices.</returns>
    [HttpGet("cryptoInfoList")]
    public async Task<PartialViewResult> PricesGrid()
    {
        // Make a GET request to the API endpoint for the latest cryptocurrency prices.
        var resp = await _http.GetAsync($"{_siteSettings.EndPointUrl}/api/crypto/latest-prices");

        // If the response is not successful, return an empty list to the partial view.
        if (!resp.IsSuccessStatusCode)
        {
            return PartialView("_InfoList", Enumerable.Empty<CryptoCurrencyDto>());
        }

        // Deserialize the response content into a collection of CryptoCurrencyDto objects.
        var stream = await resp.Content.ReadAsStreamAsync();
        var model = await JsonSerializer.DeserializeAsync<IEnumerable<CryptoCurrencyDto>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Return the partial view with the deserialized data.
        return PartialView("_InfoList", model);
    }
}