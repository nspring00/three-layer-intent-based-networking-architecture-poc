using Data.Core.Models;

namespace Data.Core.Services;
public interface IEfficiencyService
{
    float ComputeAvgEfficiency(Utilization utilization);
}
