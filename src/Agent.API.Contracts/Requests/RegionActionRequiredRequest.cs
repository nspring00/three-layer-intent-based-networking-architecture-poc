using System.Text.Json.Serialization;

namespace Agent.API.Contracts.Requests;

public class RegionActionRequiredRequest
{
    [JsonPropertyName("Region")]
    public string Region { get; set; } = default!;
}
