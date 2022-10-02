using Common.Models;
using Data.Core.Models;
using Data.Core.Repositories;
using Data.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Data.Test;

public class NetworkObjectServiceTests
{
    private readonly INetworkObjectService _sut;
    private readonly INetworkObjectRepository _networkObjectRepository;
    private readonly IEfficiencyService _efficiencyService;

    public NetworkObjectServiceTests()
    {
        var logger = Substitute.For<ILogger<NetworkObjectService>>();
        _networkObjectRepository = Substitute.For<INetworkObjectRepository>();
        _efficiencyService = Substitute.For<IEfficiencyService>();
        _sut = new NetworkObjectService(logger, _networkObjectRepository, _efficiencyService);
    }

    [Fact]
    public void WhenAggregateSingleThenSameValue()
    {
        var baseDate = new DateTime(2022, 01, 01);
        var region = new Region("Vienna");
        var nos = new List<NetworkObject>()
        {
            new()
            {
                Created = baseDate.AddDays(-10),
                Id = 1,
                Region = region,
                Infos =
                {
                    {
                        baseDate, new NetworkObjectInfo
                        {
                            Utilization = new Utilization
                            {
                                CpuUtilization = 0.7f, MemoryUtilization = 0.8f
                            },
                            Availability = 0.9f
                        }
                    }
                }
            },
            new()
            {
                Created = baseDate.AddDays(-9),
                Id = 2,
                Region = region,
                Infos =
                {
                    {
                        baseDate, new NetworkObjectInfo
                        {
                            Utilization = new Utilization
                            {
                                CpuUtilization = 0.1f, MemoryUtilization = 0.4f
                            },
                            Availability = 0.5f
                        }
                    }
                }
            }
        };
        var dict = new Dictionary<Region, IList<NetworkObject>>
        {
            {
                region, nos
            }
        };

        _networkObjectRepository.GetAll().Returns(dict);
        _efficiencyService.ComputeAvgEfficiency(new Utilization
        {
            CpuUtilization = 0.7f, MemoryUtilization = 0.8f
        }).Returns(0.7f);
        _efficiencyService.ComputeAvgEfficiency(new Utilization
        {
            CpuUtilization = 0.1f,
            MemoryUtilization = 0.4f
        }).Returns(0.5f);

        var value = _sut.AggregateUpdates(baseDate.AddMinutes(-2), baseDate);

        value.Keys.Count.Should().Be(1);
        value.Keys.First().Should().Be(region);
        var update = value[region];

        update.DeviceCount.Should().Be(2);
        update.AvgEfficiency.Should().Be(0.6f);
        update.AvgAvailability.Should().Be(0.7f);
    }

    [Fact]
    public void WhenAggregateTwoInfosForOneValueThenAverage()
    {
        var baseDate = new DateTime(2022, 01, 01);
        var region = new Region("Vienna");
        var nos = new List<NetworkObject>()
        {
            new()
            {
                Created = baseDate.AddDays(-10),
                Id = 1,
                Region = region,
                Infos =
                {
                    {
                        baseDate.AddMinutes(-1), new NetworkObjectInfo
                        {
                            Utilization = new Utilization
                            {
                                CpuUtilization = 0.1f, MemoryUtilization = 0.4f
                            },
                            Availability = 0.2f
                        }
                    },
                    {
                        baseDate, new NetworkObjectInfo
                        {
                            Utilization = new Utilization
                            {
                                CpuUtilization = 0.3f, MemoryUtilization = 0.6f
                            },
                            Availability = 0.6f
                        }
                    }
                }
            }
        };
        var dict = new Dictionary<Region, IList<NetworkObject>>
        {
            {
                region, nos
            }
        };

        _networkObjectRepository.GetAll().Returns(dict);
        _efficiencyService.ComputeAvgEfficiency(new Utilization
        {
            CpuUtilization = 0.2f,
            MemoryUtilization = 0.5f
        }).Returns(0.3f);

        var value = _sut.AggregateUpdates(baseDate.AddMinutes(-2), baseDate);

        value.Keys.Count.Should().Be(1);
        value.Keys.First().Should().Be(region);
        var update = value[region];

        update.DeviceCount.Should().Be(1);
        update.AvgEfficiency.Should().Be(0.3f);
        update.AvgAvailability.Should().Be(0.4f);
    }

    // TODO fix other test cases
    //[Fact]
    //public void WhenAggregateTwoInfosNotEndingAtEndForOneValueThenAverage()
    //{
    //    var baseDate = new DateTime(2022, 01, 01);
    //    var region = new Region("Vienna");
    //    var nos = new List<NetworkObject>()
    //    {
    //        new()
    //        {
    //            Created = baseDate.AddDays(-10),
    //            Id = 1,
    //            Region = region,
    //            Infos =
    //            {
    //                {
    //                    baseDate.AddMinutes(-2), new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.1f, MemoryUtilization = 0.4f
    //                        },
    //                        Availability = 0.2f
    //                    }
    //                },
    //                {
    //                    baseDate.AddMinutes(-1), new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.3f, MemoryUtilization = 0.6f
    //                        },
    //                        Availability = 0.6f
    //                    }
    //                }
    //            }
    //        }
    //    };
    //    var dict = new Dictionary<Region, IList<NetworkObject>>
    //    {
    //        {
    //            region, nos
    //        }
    //    };

    //    _networkObjectRepository.GetAll().Returns(dict);

    //    var value = _sut.AggregateUpdates(baseDate.AddMinutes(-4), baseDate);

    //    value.Keys.Count.Should().Be(1);
    //    value.Keys.First().Should().Be(region);
    //    var update = value[region];

    //    update.Added.Count.Should().Be(0);
    //    update.Removed.Count.Should().Be(0);
    //    update.Timestamp.Should().Be(baseDate);
    //    update.Updates.Count.Should().Be(1);
    //    var update1 = update.Updates[1];
    //    update1.Utilization.CpuUtilization.Should().Be(0.2f);
    //    update1.Utilization.MemoryUtilization.Should().Be(0.5f);
    //    update1.Availability.Should().Be(0.4f);
    //}

    //[Fact]
    //public void WhenAggregateTwoInfosDifferentDurationForOneValueThenWeightedAverage()
    //{
    //    var baseDate = new DateTime(2022, 01, 01);
    //    var region = new Region("Vienna");
    //    var nos = new List<NetworkObject>()
    //    {
    //        new()
    //        {
    //            Created = baseDate.AddDays(-10),
    //            Id = 1,
    //            Region = region,
    //            Infos =
    //            {
    //                {
    //                    baseDate.AddMinutes(-1), new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.1f, MemoryUtilization = 0.2f
    //                        },
    //                        Availability = 0f
    //                    }
    //                },
    //                {
    //                    baseDate, new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.5f, MemoryUtilization = 1f
    //                        },
    //                        Availability = 1f
    //                    }
    //                }
    //            }
    //        }
    //    };
    //    var dict = new Dictionary<Region, IList<NetworkObject>>
    //    {
    //        {
    //            region, nos
    //        }
    //    };

    //    _networkObjectRepository.GetAll().Returns(dict);

    //    var value = _sut.AggregateUpdates(baseDate.AddMinutes(-4), baseDate);

    //    value.Keys.Count.Should().Be(1);
    //    value.Keys.First().Should().Be(region);
    //    var update = value[region];

    //    update.Added.Count.Should().Be(0);
    //    update.Removed.Count.Should().Be(0);
    //    update.Timestamp.Should().Be(baseDate);
    //    update.Updates.Count.Should().Be(1);
    //    var update1 = update.Updates[1];
    //    update1.Utilization.CpuUtilization.Should().Be(0.2f);
    //    update1.Utilization.MemoryUtilization.Should().Be(0.4f);
    //    update1.Availability.Should().Be(0.25f);
    //}

    //[Fact]
    //public void WhenAggregateSingleCreatedAfterBeginningThenSameValue()
    //{
    //    var baseDate = new DateTime(2022, 01, 01);
    //    var region = new Region("Vienna");
    //    var nos = new List<NetworkObject>
    //    {
    //        new()
    //        {
    //            Created = baseDate.AddMinutes(-1),
    //            Id = 1,
    //            Region = region,
    //            Infos =
    //            {
    //                {
    //                    baseDate.AddMinutes(-1), new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.7f, MemoryUtilization = 0.8f
    //                        },
    //                        Availability = 0.9f
    //                    }
    //                }
    //            }
    //        }
    //    };
    //    var dict = new Dictionary<Region, IList<NetworkObject>>
    //    {
    //        {
    //            region, nos
    //        }
    //    };

    //    _networkObjectRepository.GetAll().Returns(dict);

    //    var value = _sut.AggregateUpdates(baseDate.AddMinutes(-4), baseDate);

    //    value.Keys.Count.Should().Be(1);
    //    value.Keys.First().Should().Be(region);
    //    var update = value[region];

    //    update.Added.Count.Should().Be(0);
    //    update.Removed.Count.Should().Be(0);
    //    update.Timestamp.Should().Be(baseDate);
    //    update.Updates.Count.Should().Be(1);
    //    var update1 = update.Updates[1];
    //    update1.Utilization.CpuUtilization.Should().Be(0.7f);
    //    update1.Utilization.MemoryUtilization.Should().Be(0.8f);
    //    update1.Availability.Should().Be(0.9f);
    //}

    //[Fact]
    //public void WhenAggregateSingleWithMultipleInfosCreatedAfterBeginningThenSameValue()
    //{
    //    var baseDate = new DateTime(2022, 01, 01);
    //    var region = new Region("Vienna");
    //    var nos = new List<NetworkObject>
    //    {
    //        new()
    //        {
    //            Created = baseDate.AddMinutes(-2),
    //            Id = 1,
    //            Region = region,
    //            Infos =
    //            {
    //                {
    //                    baseDate.AddMinutes(-1), new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.1f, MemoryUtilization = 0.2f
    //                        },
    //                        Availability = 0.3f
    //                    }
    //                },
    //                {
    //                    baseDate, new NetworkObjectInfo
    //                    {
    //                        Utilization = new Utilization
    //                        {
    //                            CpuUtilization = 0.3f, MemoryUtilization = 0.4f
    //                        },
    //                        Availability = 0.5f
    //                    }
    //                }
    //            }
    //        }
    //    };
    //    var dict = new Dictionary<Region, IList<NetworkObject>>
    //    {
    //        {
    //            region, nos
    //        }
    //    };

    //    _networkObjectRepository.GetAll().Returns(dict);

    //    var value = _sut.AggregateUpdates(baseDate.AddMinutes(-10), baseDate);

    //    value.Keys.Count.Should().Be(1);
    //    value.Keys.First().Should().Be(region);
    //    var update = value[region];

    //    update.Added.Count.Should().Be(0);
    //    update.Removed.Count.Should().Be(0);
    //    update.Timestamp.Should().Be(baseDate);
    //    update.Updates.Count.Should().Be(1);
    //    var update1 = update.Updates[1];
    //    update1.Utilization.CpuUtilization.Should().Be(0.2f);
    //    update1.Utilization.MemoryUtilization.Should().Be(0.3f);
    //    update1.Availability.Should().Be(0.4f);
    //}
}
