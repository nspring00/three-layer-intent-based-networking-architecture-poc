using System.Diagnostics;

const string outputFile = "reasoning_plot.csv";

var bounds = new List<(int, int)>
{
    (100, 120), (90, 110), (400, 410)
};

var result = ComputeScalingDeltaForConflicts(210, bounds);

Debug.Assert(result == 0);

Console.WriteLine("DONE");




static int ComputeScalingDeltaForConflicts(int deviceCount, IList<(int, int)> bounds)
{
    var lowestMin = bounds.MinBy(x => x.Item1).Item1;
    var highestMax = bounds.MaxBy(x => x.Item2).Item2;

    var result = Enumerable.Range(lowestMin, highestMax - lowestMin + 1)
        .MinBy(count => ComputeError(count, bounds));

    var dcs = new List<int>();
    var errors = new List<int>();
    
    var best = lowestMin;
    var bestError = int.MaxValue;
    for (var i = lowestMin; i <= highestMax; i++)
    {
        // Generate error value for each possible device count and choose the one with the lowest error
        var error = ComputeError(i, bounds);
        
        dcs.Add(i);
        errors.Add(error);
        
        if (error >= bestError)
        {
            continue;
        }

        best = i;
        bestError = error;
    }

    // Currently computing this twice
    // TODO remove one
    Debug.Assert(result == best);

    var outString = string.Join(",", dcs) + "\n" + string.Join(",", errors) + "\n";
    File.WriteAllText(outputFile, outString);

    return result - deviceCount;
}

static int ComputeError(int deviceCount, IEnumerable<(int, int)> bounds)
{
    return bounds.Sum(bound =>
    {
        var lowerError = bound.Item1 < deviceCount ? 0 : (int)Math.Pow(bound.Item1 - deviceCount, 2);
        var upperError = bound.Item2 > deviceCount ? 0 : (int)Math.Pow(deviceCount - bound.Item2, 2);
        return lowerError + upperError;
    });
}
