using Data.Console.Models;

namespace Data.Console.Services;
public interface IEfficiencyService
{
    float ComputeAvgEfficiency(Utilization utilization);
}
