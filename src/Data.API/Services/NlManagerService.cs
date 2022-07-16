using Common.Models;
using Data.API.Models;
using Data.API.Options;
using Microsoft.Extensions.Options;

namespace Data.API.Services;

public class NlManagerService : INlManagerService
{
    private readonly Dictionary<int, NlManagerInfo> _nlManagers;

    public NlManagerService(IOptions<List<NlManagerInfoOptions>> nlManagerOptions)
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

    private static NlManagerInfo MapNlManagerInfo(NlManagerInfoOptions options)
    {
        return new NlManagerInfo(options.Id, options.Name, new Uri(options.Uri), new Region(options.Region));
    }
}
