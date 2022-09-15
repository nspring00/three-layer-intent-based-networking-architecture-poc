using Common.Models;

namespace Knowledge.API.Models;

public record Intent(Region Region, KpiTarget Target);

public record KpiTarget(KeyPerformanceIndicator Kpi, TargetMode TargetMode, float TargetValue);

public enum TargetMode
{
    Min,
    Max
}

public enum KeyPerformanceIndicator
{
    Efficiency,
    Availability
}
