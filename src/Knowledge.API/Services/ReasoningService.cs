using System.Diagnostics;
using Common.Models;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using MathNet.Numerics.LinearRegression;

namespace Knowledge.API.Services;

public class ReasoningService : IReasoningService
{
    private readonly ILogger<ReasoningService> _logger;
    private readonly IWorkloadRepository _workloadRepository;
    private readonly IIntentService _intentService;

    public ReasoningService(
        ILogger<ReasoningService> logger,
        IWorkloadRepository workloadRepository,
        IIntentService intentService)
    {
        _logger = logger;
        _workloadRepository = workloadRepository;
        _intentService = intentService;
    }

    public IDictionary<Region, bool> QuickReasoningForRegions(IList<Region> regions)
    {
        _logger.LogInformation("Quick reasoning for {RegionCount} regions", regions.Count);
        return regions.ToDictionary(
            region => region,
            region =>
            {
                var info = _workloadRepository.GetLatest(region);
                if (info is null) return false;

                foreach (var (kpi, minMaxTarget) in _intentService.GetKpiTargetsForRegion(region))
                {
                    Debug.Assert(minMaxTarget.HasMin || minMaxTarget.HasMax);

                    var value = GetKpiValue(info, kpi);

                    if (minMaxTarget.HasMin && value < minMaxTarget.Min)
                        return true;

                    if (minMaxTarget.HasMax && value > minMaxTarget.Max)
                        return true;
                }

                return false;
            });
    }

    private static float GetKpiValue(WorkloadInfo info, KeyPerformanceIndicator kpi)
    {
        return kpi switch
        {
            KeyPerformanceIndicator.Efficiency => info.AvgEfficiency,
            KeyPerformanceIndicator.Availability => info.AvgAvailability,
            _ => throw new Exception($"Unknown KPI {kpi.ToString()}")
        };
    }

    public static Dictionary<KeyPerformanceIndicator, float> GenerateKpiTrends(IList<WorkloadInfo> infos,
        IList<KeyPerformanceIndicator> kpis)
    {
        return kpis.ToDictionary(
            kpi => kpi,
            kpi =>
            {
                // Use linear regression to calculate the trend
                var data = infos.Select(x => new Tuple<double, double>(x.Id, GetKpiValue(x, kpi)));
                var (a, b) = SimpleRegression.Fit(data);
                var trend = a + b * (infos.First().Id + 1);
                return (float)trend;
            });
    }

    // Check if efficiency goal is reached for region
    public ReasoningComposition ReasonForRegion(Region region)
    {
        _logger.LogInformation("Reasoning for region {Region}", region.Name);

        var infos = _workloadRepository.GetForRegion(region, 5);

        if (infos.Count == 0)
        {
            _logger.LogWarning("No workload info for region {Region}", region.Name);
            return new ReasoningComposition(false);
        }

        _logger.LogInformation("Last 5 workload ids: {WorkloadIds}", infos.Select(i => i.Id));


        //          
        //          var intents = _intentService.GetForRegion(region);
        //          var latestWorkloadInfo = _workloadRepository.GetLatest(region);
        //          if (latestWorkloadInfo is null)
        //          {
        //              _logger.LogError("No workload info for region {Region}", region);
        //              return new ReasoningComposition(false);
        //          }
        //          
        //          var avg = latestWorkloadInfo.AvgEfficiency;
        //          var deviceCount = latestWorkloadInfo.DeviceCount;
        //  
        //          // Get min and max from intents
        //          var min = intents.FirstOrDefault(i => i.Set.TargetMode == TargetMode.Min);
        //          var max = intents.FirstOrDefault(i => i.Set.TargetMode == TargetMode.Max);
        //  
        //          _logger.LogInformation("Min: {Min}, Max: {Max}, Avg: {Avg}, Count: {Count}", min?.Set.TargetValue, max?.Set.TargetValue, avg, deviceCount);
        //  
        //          
        //          //if (min is not null && max is not null && (avg < min.Set.TargetValue || avg > max.Set.TargetValue))
        //          //{
        //          //    // If both min and max are set, target their mean
        //          //    var target = (min.Set.TargetValue + max.Set.TargetValue) / 2f;
        //          //    var delta = (int)Math.Round(CalculateDelta(deviceCount, avg, target));
        //          //    _logger.LogInformation("Min/Max Delta: {Delta}", delta);
        //          //    return new ReasoningComposition(true, new AgentAction(delta));
        //          //}
        //          
        //          if (max is not null && avg > max.Set.TargetValue)
        //          {
        //              // If only max, then scale up minimum required amount (ceil)
        //              var delta = (int) Math.Ceiling(CalculateDelta(deviceCount, avg, max.Set.TargetValue));
        //              _logger.LogInformation("Max Delta: {Delta}", delta);
        //              return new ReasoningComposition(true, new AgentAction(delta));
        //          }
        //  
        //          if (min is not null && avg < min.Set.TargetValue)
        //          {
        //              // If only min, then scale down minimum required amount (floor)
        //              var delta = (int)Math.Floor(CalculateDelta(deviceCount, avg, min.Set.TargetValue));
        //              _logger.LogInformation("Min Delta: {Delta}", delta);
        //              return new ReasoningComposition(true, new AgentAction(delta));
        //          }

        return new ReasoningComposition(false);
    }

    // Formula see notion
    private static float CalculateDelta(int count, float currentAvg, float targetAvg)
    {
        return count * (currentAvg / targetAvg - 1);
    }
}
