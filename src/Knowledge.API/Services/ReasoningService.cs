using System.Diagnostics;
using Common.Models;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using MathNet.Numerics;

namespace Knowledge.API.Services;

public class ReasoningService : IReasoningService
{
    public int MaxInfosForReasoning => 5;

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

                    var value = GetKpiComputedValue(info, kpi);

                    const float increasingFactor = 0.1f;

                    if (minMaxTarget.HasMin)
                    {
                        // For availability, increase by raising (value is <1, therefore 1-increasingFactor)
                        // Else scale by 10%
                        var minThreshold = kpi == KeyPerformanceIndicator.Availability 
                            ? Math.Pow(minMaxTarget.Min!.Value, 1 - increasingFactor)
                            : minMaxTarget.Min!.Value * (1 + increasingFactor);
                        
                        if (minThreshold > value)
                        {
                            return true;
                        }
                    }

                    if (minMaxTarget.HasMax)
                    {
                        var maxThreshold = kpi == KeyPerformanceIndicator.Availability
                            ? Math.Pow(minMaxTarget.Max!.Value, 1 + increasingFactor)
                            : minMaxTarget.Max!.Value * (1 - increasingFactor);

                        if (maxThreshold < value)
                        {
                            return true;
                        }
                    }
                }

                return false;
            });
    }

    private static float GetKpiRawValue(WorkloadInfo info, KeyPerformanceIndicator kpi)
    {
        return kpi switch
        {
            KeyPerformanceIndicator.Efficiency => info.AvgEfficiency * info.DeviceCount,
            KeyPerformanceIndicator.Availability => info.AvgAvailability,
            _ => throw new Exception($"Unknown KPI {kpi.ToString()}")
        };
    }
    
    private static float GetKpiComputedValue(WorkloadInfo info, KeyPerformanceIndicator kpi)
    {
        return kpi switch
        {
            KeyPerformanceIndicator.Efficiency => info.AvgEfficiency,
            KeyPerformanceIndicator.Availability => 1 - (float) Math.Pow(1 - info.AvgAvailability, info.DeviceCount),
            _ => throw new Exception($"Unknown KPI {kpi.ToString()}")
        };
    }

    public Dictionary<KeyPerformanceIndicator, float> GenerateKpiTrends(IList<WorkloadInfo> infos,
        IList<KeyPerformanceIndicator> kpis)
    {
        if (infos.Count == 0)
        {
            return new Dictionary<KeyPerformanceIndicator, float>();
        }

        var dc = infos.MaxBy(x => x.Id)?.DeviceCount;

        return kpis.ToDictionary(
            kpi => kpi,
            kpi =>
            {
                // Use linear regression to calculate the trend

                // var data = infos.Select(x => new Tuple<double, double>(x.Id, GetKpiValue(x, kpi))).ToList();

                var xValues = infos.Select(x => (double)x.Id).ToArray();
                var yValues = infos.Select(x => (double)GetKpiRawValue(x, kpi)).ToArray();

                Debug.Assert(xValues.Length == yValues.Length);

                // Square polynomial regression requires at least 3 data points
                if (yValues.Length < 3)
                {
                    if (kpi == KeyPerformanceIndicator.Efficiency)
                    {
                        return (float)(yValues.First() / dc!);
                    }

                    return (float)yValues.First();
                }

                // Execute polynomial regression with order 2
                // and compute the trend for the next-next period
                var p = Fit.Polynomial(xValues, yValues, 2);
                var trend = Polynomial.Evaluate(infos.MaxBy(x => x.Id)!.Id + 2, p);

                // Old approach of using linear regression
                // var data = xValues.Zip(yValues, (x, y) => new Tuple<double, double>(x, y));
                // var (a, b) = SimpleRegression.Fit(data);
                // var trend = a + b * (infos.MaxBy(x => x.Id)!.Id + 2);


                // when using efficiency, this value is actually the workload trend which needs to be transformed 
                // into efficiency trend
                if (kpi == KeyPerformanceIndicator.Efficiency)
                {
                    trend = (double)(trend / dc)!;
                }

                // Clamp the trend to the min/max values (should not occur)
                if (trend < 0)
                {
                    _logger.LogWarning("Negative trend for {Kpi}", kpi);
                    trend = 0;
                }

                if (trend > 1)
                {
                    _logger.LogWarning("Trend > 1 for {Kpi}", kpi);
                    trend = 1;
                }

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

        var kpis = new[]
        {
            KeyPerformanceIndicator.Efficiency,
            KeyPerformanceIndicator.Availability
        };
        var trends = GenerateKpiTrends(infos, kpis);
        
        _logger.LogInformation("Trends: {Trends}", trends);

        if (!kpiTargets.ContainsKey(KeyPerformanceIndicator.Efficiency) &&
            !kpiTargets.ContainsKey(KeyPerformanceIndicator.Availability))
        {
            _logger.LogWarning("No trends for region {Region}", region.Name);
            return new ReasoningComposition(false);
        }

        var deviceCount = infos.MaxBy(x => x.Id)!.DeviceCount;
        _logger.LogInformation("Device count {Region}: {DeviceCount}", region.Name, deviceCount);

        // Currently only efficiency and availability supported
        bool effOk = true;
        bool availOk = true;

        if (kpiTargets.ContainsKey(KeyPerformanceIndicator.Efficiency))
        {
            effOk = kpiTargets[KeyPerformanceIndicator.Efficiency]
                .IsInRange(trends[KeyPerformanceIndicator.Efficiency]);
            _logger.LogInformation("Efficiency trend: {EfficiencyTrend}, goal: {EfficiencyGoal}, ok: {EfficiencyOk}",
                trends[KeyPerformanceIndicator.Efficiency],
                kpiTargets[KeyPerformanceIndicator.Efficiency],
                effOk);
        }

        // Formula described in paper
        if (kpiTargets.ContainsKey(KeyPerformanceIndicator.Availability))
        {
            var totalAvailability = 1 - (float)Math.Pow(1 - trends[KeyPerformanceIndicator.Availability], deviceCount);
            availOk = kpiTargets[KeyPerformanceIndicator.Availability].IsInRange(totalAvailability);
            _logger.LogInformation(
                "Availability trend: {AvailabilityTrend}, goal: {AvailabilityGoal}, ok: {AvailabilityOk}",
                totalAvailability,
                kpiTargets[KeyPerformanceIndicator.Availability],
                availOk);
        }

        if (effOk && availOk)
        {
            _logger.LogInformation("Region {Region} is ok", region.Name);
            return new ReasoningComposition(false);
        }

        var bounds = new List<(int, int)>();

        if (kpiTargets.ContainsKey(KeyPerformanceIndicator.Efficiency))
        {
            bounds.Add(GetEfficiencyDeviceCountBounds(deviceCount, trends[KeyPerformanceIndicator.Efficiency],
                kpiTargets[KeyPerformanceIndicator.Efficiency]));
        }

        if (kpiTargets.ContainsKey(KeyPerformanceIndicator.Availability))
        {
            bounds.Add(GetAvailabilityDeviceCountBounds(trends[KeyPerformanceIndicator.Availability],
                kpiTargets[KeyPerformanceIndicator.Availability]));
        }

        _logger.LogInformation("Bounds for region {Region}: {Bounds}", region.Name, bounds);

        var delta = ComputeScalingDelta(deviceCount, bounds);
        if (delta == 0)
        {
            _logger.LogInformation("No scaling needed for region {Region}", region.Name);
            return new ReasoningComposition(false);
        }

        _logger.LogInformation("Current count: {CC}, target: {TC}, delta: {Delta}", deviceCount, deviceCount + delta,
            delta);
        return new ReasoningComposition(true, new AgentAction(delta));
    }

    public int ComputeScalingDelta(int deviceCount, IList<(int, int)> bounds)
    {
        var highestMin = bounds.MaxBy(x => x.Item1).Item1;
        var lowestMax = bounds.MinBy(x => x.Item2).Item2;

        _logger.LogInformation("Highest min: {HighestMin}, lowest max: {LowestMax}", highestMin, lowestMax);

        int delta;
        if (highestMin <= deviceCount && deviceCount <= lowestMax)
        {
            return 0;
        }

        if (highestMin <= lowestMax)
        {
            // All bounds are compatible
            // --> find lowest scaling number that is compatible with all bounds
            delta = deviceCount < highestMin ? highestMin - deviceCount : lowestMax - deviceCount;
            _logger.LogInformation("All bounds compatible. Delta: {Delta}", delta);
            return delta;
        }

        // There are conflicting bounds
        delta = ComputeScalingDeltaForConflicts(deviceCount, bounds);
        _logger.LogInformation("Conflicting bounds. Delta: {Delta}", delta);
        return delta;
    }

    private int ComputeScalingDeltaForConflicts(int deviceCount, IList<(int, int)> bounds)
    {
        var lowestMin = bounds.MinBy(x => x.Item1).Item1;
        var highestMax = bounds.MaxBy(x => x.Item2).Item2;

        if (lowestMin > highestMax)
        {
            _logger.LogError("No scaling possible. Lowest min: {LowestMin}, highest max: {HighestMax}", lowestMin,
                highestMax);
            return 0;
        }

        var result = Enumerable.Range(lowestMin, highestMax - lowestMin + 1)
            .MinBy(count => ComputeError(count, bounds));

        return result - deviceCount;
    }

    private static int ComputeError(int deviceCount, IEnumerable<(int, int)> bounds)
    {
        return bounds.Sum(bound =>
        {
            var lowerError = bound.Item1 < deviceCount ? 0 : (int)Math.Pow(bound.Item1 - deviceCount, 2);
            var upperError = bound.Item2 > deviceCount ? 0 : (int)Math.Pow(deviceCount - bound.Item2, 2);
            return lowerError + upperError;
        });
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
}
