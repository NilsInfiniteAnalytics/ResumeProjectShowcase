using System.Text.Json.Serialization;
namespace ClassLibrary.Models.GeocodingService;
public record AddressComponent
{
    [JsonPropertyName("long_name")]
    public string LongName { get; init; }
    [JsonPropertyName("short_name")]
    public string ShortName { get; init; }
    [JsonPropertyName("types")]
    public string[] Types { get; init; }
}
