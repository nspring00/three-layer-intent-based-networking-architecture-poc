﻿using Data.Console.Models;
using Data.Console.Services;
using Microsoft.Extensions.Logging;

namespace Data.Console;

public class ServiceRunner
{
    private readonly ILogger<ServiceRunner> _logger;
    private readonly INetworkLayerService _networkLayerService;
    private readonly INetworkObjectService _networkObjectService;

    private readonly IDictionary<Region, IList<Uri>> _uris = new Dictionary<Region, IList<Uri>>
    {
        {
            new Region("Vienna"), new List<Uri>
            {
                new("https://localhost:7071")
            }
        }
    };

    public ServiceRunner(
        ILogger<ServiceRunner> logger,
        INetworkLayerService networkLayerService,
        INetworkObjectService networkObjectService)
    {
        _logger = logger;
        _networkLayerService = networkLayerService;
        _networkObjectService = networkObjectService;
    }

    public async Task Run()
    {
        // TODO do this (maybe multiple times) first
        var startTime = DateTime.UtcNow;
        Thread.Sleep(5000);
        await FetchAllUpdates();
        Thread.Sleep(5000);
        await FetchAllUpdates();

        var endTime = DateTime.UtcNow;
            
        var updates = _networkObjectService.AggregateUpdates(startTime, endTime);
        _networkObjectService.Reset();

        await SendUpdatesToKnowledge(updates);


        System.Console.ReadLine();
    }

    private async Task SendUpdatesToKnowledge(IDictionary<Region, NetworkUpdate> updates)
    {
        throw new NotImplementedException();
    }

    private async Task FetchAllUpdates()
    {
        foreach (var (region, uris) in _uris)
        {
            foreach (var uri in uris)
            {
                await _networkLayerService.FetchAllUpdates(region, uri);
            }
        }
    }
}
