using System.Diagnostics;
using Common.Models;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using MathNet.Numerics.LinearRegression;

namespace Knowledge.API.Services;

public class ReasoningService : IReasoningService
{
    public const int MaxInfosForReasoning = 5;

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
                var trend = a + b * (infos.MaxBy(x => x.Id)!.Id + 1);
                return (float)trend;
            });
    }

    // Check if efficiency goal is reached for region
    public ReasoningComposition ReasonForRegion(Region region)
    {
        _logger.LogInformation("Reasoning for region {Region}", region.Name);

        var infos = _workloadRepository.GetForRegion(region, MaxInfosForReasoning);
        var kpiTargets = _intentService.GetKpiTargetsForRegion(region);

        if (infos.Count == 0)
        {
            _logger.LogWarning("No workload info for region {Region}", region.Name);
            return new ReasoningComposition(false);
        }

        if (kpiTargets.Count == 0)
        {
            _logger.LogWarning("No KPI targets for region {Region}", region.Name);
            return new ReasoningComposition(false);
        }

        _logger.LogInformation("Last 5 workload ids: {WorkloadIds}", infos.Select(i => i.Id));

        var kpis = new KeyPerformanceIndicator[]
        {
            KeyPerformanceIndicator.Efficiency,
            KeyPerformanceIndicator.Availability
        };
        var trends = GenerateKpiTrends(infos, kpis);

        // Currently only efficiency and availability supported
        var effOk = kpiTargets[KeyPerformanceIndicator.Efficiency]
            .IsInRange(trends[KeyPerformanceIndicator.Efficiency]);

        // Formula described in paper
        var deviceCount = infos.MaxBy(x => x.Id)!.DeviceCount;
        var totalAvailability = 1 - (float)Math.Pow(1 - trends[KeyPerformanceIndicator.Availability], deviceCount);
        var availOk = kpiTargets[KeyPerformanceIndicator.Availability].IsInRange(totalAvailability);

        if (effOk && availOk)
        {
            return new ReasoningComposition(false);
        }

        // TODO: Add more reasoning here


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

    public static (int, int) GetEfficiencyDeviceCountBounds(int currentCount, float currentEff, MinMaxTarget target)
    {
        var minCount = 0;
        var maxCount = int.MaxValue;
        var (minEfficiency, maxEfficiency) = target;

        if (maxEfficiency.HasValue)
        {
            minCount = (int)Math.Ceiling((currentCount * currentEff) / maxEfficiency.Value);
        }

        if (minEfficiency.HasValue)
        {
            maxCount = (int)Math.Floor((currentCount * currentEff) / minEfficiency.Value);
        }

        return (minCount, maxCount);
    }

    public static (int, int) GetAvailabilityDeviceCountBounds(float avgAvailability, MinMaxTarget target)
    {
        var minCount = 0;
        var maxCount = int.MaxValue;
        var (minAvailability, maxAvailability) = target;

        if (minAvailability.HasValue)
        {
            minCount = (int)Math.Ceiling(Math.Log(1 - minAvailability.Value) / Math.Log(1 - avgAvailability));
        }

        if (maxAvailability.HasValue)
        {
            maxCount = (int)Math.Floor(Math.Log(1 - maxAvailability.Value) / Math.Log(1 - avgAvailability));
        }

        return (minCount, maxCount);
    }

    // Formula see notion
    private static float CalculateDelta(int count, float currentAvg, float targetAvg)
    {
        return count * (currentAvg / targetAvg - 1);
    }
}
