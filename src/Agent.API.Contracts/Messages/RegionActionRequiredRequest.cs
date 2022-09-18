using System.Text.Json.Serialization;
using Common.Messages;

namespace Agent.API.Contracts.Messages;

public class RegionActionRequiredRequest : IMessage
{
    [JsonPropertyName("Region")]
    public string Region { get; set; } = default!;
    
    [JsonIgnore]
    public string MessageTypeName => nameof(RegionActionRequiredRequest);
}
