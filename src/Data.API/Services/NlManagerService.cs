using Common.Models;
using Data.API.Models;
using Data.API.Options;
using Microsoft.Extensions.Options;

namespace Data.API.Services;

public class NlManagerService : INlManagerService
{
    private readonly ILogger<NlManagerService> _logger;
    private readonly Dictionary<int, NlManagerInfo> _nlManagers;

    public NlManagerService(ILogger<NlManagerService> logger, IOptions<List<NlManagerInfoOptions>> nlManagerOptions)
    {
        _logger = logger;
        _nlManagers = nlManagerOptions.Value
            .Select(MapNlManagerInfo)
            .ToDictionary(x => x.Id, x => x);
        
        _logger.LogInformation("Initialized with {NlManagerCount} NL managers: {NlManagerNames}", _nlManagers.Count,
            string.Join(", ", _nlManagers.Values));
    }

    public Uri? GetUriById(int nlId)
    {
        if (!_nlManagers.ContainsKey(nlId))
        {
            _logger.LogWarning("Uri not found for NL manager with id {NlManagerId}", nlId);
            return null;
        }

        return _nlManagers[nlId].Uri;
    }

    private static NlManagerInfo MapNlManagerInfo(NlManagerInfoOptions options)
    {
        return new NlManagerInfo(options.Id, options.Name, new Uri(options.Uri), new Region(options.Region));
    }
}
