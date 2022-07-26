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
    private readonly IIntentRepository _intentRepository;
    private readonly INetworkInfoRepository _networkInfoRepository;

    public ReasoningServiceTests()
    {
        var logger = Substitute.For<ILogger<ReasoningService>>();
        _networkInfoRepository = Substitute.For<INetworkInfoRepository>();
        _intentRepository = Substitute.For<IIntentRepository>();
        _sut = new ReasoningService(logger, _intentRepository, _networkInfoRepository);
    }

    [Theory]
    [InlineData("Region", 5, 0.7f, 0.8f, -1)]
    [InlineData("Region", 5, 0.6f, 0.8f, -2)]
    [InlineData("Region", 100, 0.6f, 0.8f, -25)]
    public void TestMinIntentScaling(string regionName, int count, float average, float min, int expectedScale)
    {
        var region = new Region(regionName);
        _networkInfoRepository.GetForRegion(region).Returns(
            Enumerable.Repeat(new NetworkDevice(1, region, "", new Utilization
            {
                CpuUtilization = average
            }), count).ToList());
        _intentRepository.GetForRegion(region).Returns(
            new List<Intent>
            {
                new(region, new Efficiency(TargetMode.Min, min))
            });

        var response = _sut.ReasonForRegion(region);

        response.ActionRequired.Should().BeTrue();
        response.Action.Should().NotBeNull().And.Match<AgentAction>(a => a.Scale == expectedScale);
    }

    [Theory]
    [InlineData("Region", 5, 0.8f, 0.7f, 1)]
    [InlineData("Region", 5, 0.8f, 0.6f, 2)]
    [InlineData("Region", 100, 0.8f, 0.6f, 34)]
    public void TestMaxIntentScaling(string regionName, int count, float average, float max, int expectedScale)
    {
        var region = new Region(regionName);
        _networkInfoRepository.GetForRegion(region).Returns(
            Enumerable.Repeat(new NetworkDevice(1, region, "", new Utilization
            {
                CpuUtilization = average
            }), count).ToList());
        _intentRepository.GetForRegion(region).Returns(
            new List<Intent>
            {
                new(region, new Efficiency(TargetMode.Max, max))
            });

        var response = _sut.ReasonForRegion(region);

        response.ActionRequired.Should().BeTrue();
        response.Action.Should().NotBeNull().And.Match<AgentAction>(a => a.Scale == expectedScale);
    }
}
