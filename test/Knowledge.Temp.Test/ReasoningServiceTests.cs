using Common.Models;
using FluentAssertions;
using Knowledge.API.Models;
using Knowledge.API.Repository;
using Knowledge.API.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Knowledge.Temp.Test;

public class ReasoningServiceTests
{
    private readonly ReasoningService _sut;
    private readonly IWorkloadRepository _workloadRepository;
    private readonly IIntentService _intentService;

    public ReasoningServiceTests()
    {
        var logger = Substitute.For<ILogger<ReasoningService>>();
        _workloadRepository = Substitute.For<IWorkloadRepository>();
        _intentService = Substitute.For<IIntentService>();
        _sut = new ReasoningService(logger, _workloadRepository, _intentService);
    }

    [Fact]
    public void QuickReasoning_WhenWorkloadIsNotAvailable_ShouldReturnFalse()
    {
        var region = new Region("Vienna");
        _workloadRepository.GetLatest(region).Returns(null as WorkloadInfo);

        // Act
        var result = _sut.QuickReasoningForRegions(new List<Region>
        {
            region
        });

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(region, false);
    }

    [Theory]
    [InlineData(10, 0.7f, 0.7f, 0.5f, 0.9f, 0.5f, 0.9f, false)] // OK
    [InlineData(10, 0.4f, 0.7f, 0.5f, 0.9f, 0.5f, 0.9f, true)] // eff too low
    [InlineData(10, 1f, 0.7f, 0.5f, 0.9f, 0.5f, 0.9f, true)] // eff too high
    [InlineData(10, 0.7f, 0.4f, 0.5f, 0.9f, 0.5f, 0.9f, true)] // avail too low
    [InlineData(10, 0.7f, 1f, 0.5f, 0.9f, 0.5f, 0.9f, true)] // avail too high
    public void QuickReasoning_WithGivenParams_ShouldReturnGivenResult(
        int deviceCount,
        float avgEfficiency,
        float avgAvailability,
        float minEfficiency,
        float maxEfficiency,
        float minAvailability,
        float maxAvailability,
        bool expectedResult)
    {
        //Arrange
        var region = new Region("Vienna");
        var workload = new WorkloadInfo
        {
            Id = 1,
            Timestamp = new DateTime(2022, 1, 1),
            DeviceCount = deviceCount,
            AvgEfficiency = avgEfficiency,
            AvgAvailability = avgAvailability
        };
        var kpiDict = new Dictionary<KeyPerformanceIndicator, MinMaxTarget>
        {
            {
                KeyPerformanceIndicator.Efficiency, new MinMaxTarget(minEfficiency, maxEfficiency)
            },
            {
                KeyPerformanceIndicator.Availability, new MinMaxTarget(minAvailability, maxAvailability)
            }
        };

        _workloadRepository.GetLatest(region).Returns(workload);
        _intentService.GetKpiTargetsForRegion(region).Returns(kpiDict);

        // Act
        var result = _sut.QuickReasoningForRegions(new List<Region>
        {
            region
        });

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(region, expectedResult);
    }

    [Fact]
    public void GenerateKpiTrends_WhenConstantWorkload_ThenConstantTrend()
    {
        // Arrange
        var kpis = new List<KeyPerformanceIndicator>
        {
            KeyPerformanceIndicator.Efficiency, KeyPerformanceIndicator.Availability
        };
        var infos = new List<WorkloadInfo>
        {
            new()
            {
                Id = 3,
                Timestamp = new DateTime(2022, 01, 03),
                DeviceCount = 10,
                AvgEfficiency = 0.3f,
                AvgAvailability = 0.4f
            },
            new()
            {
                Id = 2,
                Timestamp = new DateTime(2022, 01, 02),
                DeviceCount = 10,
                AvgEfficiency = 0.3f,
                AvgAvailability = 0.4f
            },
            new()
            {
                Id = 1,
                Timestamp = new DateTime(2022, 01, 01),
                DeviceCount = 10,
                AvgEfficiency = 0.3f,
                AvgAvailability = 0.4f
            }
        };

        // Act
        var trends = ReasoningService.GenerateKpiTrends(infos, kpis);

        // Assert
        trends.Should().HaveCount(2);
        trends.Should().Contain(KeyPerformanceIndicator.Efficiency, 0.3f);
        trends.Should().Contain(KeyPerformanceIndicator.Availability, 0.4f);
    }

    [Fact]
    public void GenerateKpiTrends_WhenLinearWorkload_ThenLinearTrend()
    {
        // Arrange
        var kpis = new List<KeyPerformanceIndicator>
        {
            KeyPerformanceIndicator.Efficiency, KeyPerformanceIndicator.Availability
        };
        var infos = new List<WorkloadInfo>
        {
            new()
            {
                Id = 3,
                Timestamp = new DateTime(2022, 01, 03),
                DeviceCount = 10,
                AvgEfficiency = 0.3f,
                AvgAvailability = 0.6f
            },
            new()
            {
                Id = 2,
                Timestamp = new DateTime(2022, 01, 02),
                DeviceCount = 10,
                AvgEfficiency = 0.4f,
                AvgAvailability = 0.5f
            },
            new()
            {
                Id = 1,
                Timestamp = new DateTime(2022, 01, 01),
                DeviceCount = 10,
                AvgEfficiency = 0.5f,
                AvgAvailability = 0.4f
            }
        };

        // Act
        var trends = ReasoningService.GenerateKpiTrends(infos, kpis);

        // Assert
        trends.Should().HaveCount(2);
        trends.Should().ContainKey(KeyPerformanceIndicator.Efficiency).WhoseValue.Should()
            .BeApproximately(0.2f, 0.0001f);
        trends.Should().ContainKey(KeyPerformanceIndicator.Availability).WhoseValue.Should()
            .BeApproximately(0.7f, 0.0001f);
    }

    [Theory]
    [InlineData(10, 0.5f, 0.3f, 0.7f, 8, 16)]
    [InlineData(100, 0.8f, 0.7f, 0.9f, 89, 114)]
    public void GetEfficiencyDeviceCountBounds_ReturnsCorrectValue(int currentCount, float currentEff, float minEff,
        float maxEff, int minResult, int maxResult)
    {
        // Arrange
        var target = new MinMaxTarget(minEff, maxEff);

        // Act
        var result = ReasoningService.GetEfficiencyDeviceCountBounds(currentCount, currentEff, target);

        // Assert
        var expectedResult = (minResult, maxResult);
        result.Should().Be(expectedResult);
    }    
    
    [Theory]
    [InlineData(0.8f, 0.7f, 0.9f, 1, 1)]
    [InlineData(0.3f, 0.7f, 0.9f, 4, 6)]
    [InlineData(0.3f, 0.99f, 0.999f, 13, 19)]
    public void GetAvailabilityDeviceCountBounds_ReturnsCorrectValue(float avgAvailability,
        float minAvailability, float maxAvailability, int minResult, int maxResult)
    {
        // Arrange
        var target = new MinMaxTarget(minAvailability, maxAvailability);

        // Act
        var result = ReasoningService.GetAvailabilityDeviceCountBounds(avgAvailability, target);

        // Assert
        var expectedResult = (minResult, maxResult);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void ReasonForRegion_WhenWorkloadIsNotAvailable_ShouldReturnFalse()
    {
        var region = new Region("Vienna");
        _workloadRepository.GetForRegion(region, ReasoningService.MaxInfosForReasoning)
            .Returns(new List<WorkloadInfo>());

        // Act
        var result = _sut.ReasonForRegion(region);

        // Assert
        result.ActionRequired.Should().BeFalse();
    }

    [Theory]
    [InlineData(100, 0, 80, 120)] // Ok
    [InlineData(80, 0, 80, 120)] // Ok
    [InlineData(120, 0, 80, 120)] // Ok
    [InlineData(100, 0, 80, 120, 70, 110)] // Ok, many bounds
    [InlineData(100, 0, 80, 120, 70, 110, 0, 130)] // Ok, many bounds
    [InlineData(121, -1, 80, 120)] // Compatible bounds
    [InlineData(79, 1, 80, 120)] // Compatible bounds
    [InlineData(63, 17, 80, 120, 70, 100)] // Compatible bounds
    [InlineData(134, -34, 80, 120, 70, 100)] // Compatible bounds
    [InlineData(83, 0, 80, 82, 84, 89)] // Conflicting intents
    [InlineData(89, 0, 80, 89, 80, 89, 90, 100)] // Conflicting intents
    [InlineData(210, 0, 100, 120, 90, 110, 400, 410)] // Conflicting intents
    public void ComputeScalingDelta_WhenGivenInput_ThenGivenOutput(int deviceCount, int expectedResult, params int[] boundValues)
    {
        // Arrange
        if (boundValues.Length % 2 != 0)
        {
            throw new ArgumentException("boundValues must have an even number of elements");
        }
        var bounds = boundValues.Chunk(2).Select(x => (x[0], x[1])).ToList(); 
        
        // Act
        var result = _sut.ComputeScalingDelta(deviceCount, bounds);
        
        // Assert
        result.Should().Be(expectedResult);
    }
}
