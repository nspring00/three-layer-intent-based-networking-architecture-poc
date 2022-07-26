using System.Text.Json.Serialization;

namespace Agent.API.Contracts.Messages;

public class RegionActionRequiredRequest
{
    [JsonPropertyName("Region")]
    public string Region { get; set; } = default!;
}
