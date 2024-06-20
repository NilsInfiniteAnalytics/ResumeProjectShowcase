using System.Text.Json.Serialization;

namespace ClassLibrary.Models.GeocodingService;
public class Result
{
    [JsonPropertyName("address_components")]
    public AddressComponent[] AddressComponents { get; init; }
    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; init; }
}
