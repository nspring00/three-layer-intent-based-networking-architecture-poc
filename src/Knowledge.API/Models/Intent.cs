namespace Knowledge.API.Models;

public record Intent(Region At, Efficiency Set);

public record Region(string Name);

public record Efficiency(TargetMode TargetMode, float TargetValue);

public enum TargetMode
{
    Min,
    Max
}
