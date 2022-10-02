namespace NetworkLayer.API.Models;

public record NetworkObject
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Utilization Utilization { get; set; } = new();
    public float Availability { get; set; }
}
