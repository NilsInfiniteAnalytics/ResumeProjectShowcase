using System.Text.Json.Serialization;

namespace ClassLibrary.Models.GeocodingService;
public record GeocodingResponse
{
    [JsonPropertyName("results")]
    public Result[] Results { get; init; }
    [JsonPropertyName("status")]
    public string Status { get; init; }
}
