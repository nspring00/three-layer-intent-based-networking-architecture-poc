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
            KeyPerformanceIndicator.Efficiency,
            KeyPerformanceIndicator.Availability
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
            KeyPerformanceIndicator.Efficiency,
            KeyPerformanceIndicator.Availability
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

    // [Theory]
    // [InlineData("Region", 5, 0.7f, 0.8f, -1)]
    // [InlineData("Region", 5, 0.6f, 0.8f, -2)]
    // [InlineData("Region", 100, 0.6f, 0.8f, -25)]
    // public void TestMinIntentScaling(string regionName, int count, float average, float min, int expectedScale)
    // {
    //     var region = new Region(regionName);
    //     _workloadRepository.GetLatest(region).Returns(new WorkloadInfo
    //     {
    //         DeviceCount = count, AvgEfficiency = average, AvgAvailability = 1f
    //     });
    //     _intentRepository.GetForRegion(region).Returns(
    //         new List<Intent>
    //         {
    //             new(region, new Efficiency(TargetMode.Min, min))
    //         });
    //
    //     var response = _sut.ReasonForRegion(region);
    //
    //     response.ActionRequired.Should().BeTrue();
    //     response.Action.Should().NotBeNull().And.Match<AgentAction>(a => a.Scale == expectedScale);
    // }
    //
    // [Theory]
    // [InlineData("Region", 5, 0.8f, 0.7f, 1)]
    // [InlineData("Region", 5, 0.8f, 0.6f, 2)]
    // [InlineData("Region", 100, 0.8f, 0.6f, 34)]
    // public void TestMaxIntentScaling(string regionName, int count, float average, float max, int expectedScale)
    // {
    //     var region = new Region(regionName);
    //     _workloadRepository.GetLatest(region).Returns(new WorkloadInfo
    //     {
    //         DeviceCount = count, AvgEfficiency = average, AvgAvailability = 1f
    //     });
    //     _intentRepository.GetForRegion(region).Returns(
    //         new List<Intent>
    //         {
    //             new(region, new Efficiency(TargetMode.Max, max))
    //         });
    //
    //     var response = _sut.ReasonForRegion(region);
    //
    //     response.ActionRequired.Should().BeTrue();
    //     response.Action.Should().NotBeNull().And.Match<AgentAction>(a => a.Scale == expectedScale);
    // }
}
