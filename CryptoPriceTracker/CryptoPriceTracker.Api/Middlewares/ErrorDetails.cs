using Newtonsoft.Json;

namespace CryptoPriceTracker.Api.Middlewares;

/// <summary>
/// Error details with the StatusCode and the Message Error or Execption Message
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Error Type Description
    /// </summary>
    [JsonProperty(nameof(ErrorType))]
    public string ErrorType { get; set; } = null!;

    /// <summary>
    /// Mesage Error or Exception Message
    /// </summary>
    [JsonProperty(nameof(Errors))]
    public List<string> Errors { get; set; } = null!;

    /// <summary>
    /// Serialize the Objet to response the Details
    /// </summary>
    /// <returns>The Json Serialized</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}