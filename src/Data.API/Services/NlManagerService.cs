using Common.Models;
using Data.API.Configs;
using Data.API.Models;
using Microsoft.Extensions.Options;

namespace Data.API.Services;

public class NlManagerService : INlManagerService
{
    private readonly Dictionary<int, NlManagerInfo> _nlManagers;

    public NlManagerService(IOptions<List<NlManagerInfoConfig>> nlManagerOptions)
    {
        _nlManagers = nlManagerOptions.Value
            .Select(MapNlManagerInfo)
            .ToDictionary(x => x.Id, x => x);
    }

    public Uri? GetUriById(int nlId)
    {
        if (!_nlManagers.ContainsKey(nlId))
        {
            return null;
        }

        return _nlManagers[nlId].Uri;
    }

    private static NlManagerInfo MapNlManagerInfo(NlManagerInfoConfig config)
    {
        return new NlManagerInfo(config.Id, config.Name, new Uri(config.Uri), new Region(config.Region));
    }
}
