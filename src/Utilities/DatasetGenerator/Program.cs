// See https://aka.ms/new-console-template for more information

using System.Globalization;

var data = GenerateOscillatingEffDataset();

var lines = data.Select(x => x.ToString()).ToList();
lines.Insert(0, GetHeader());

const string datasetFileName = "SinEfficiency.csv";

File.WriteAllLines(datasetFileName, lines);


List<DatasetLine> GenerateOscillatingEffDataset()
{
    const int minWorkload = 450;
    const float avail = 0.8f;

    const int steps = 400;
    const double stepSize = 2 * Math.PI / steps;
    const int factor = 300;

    var localData = new List<DatasetLine>();

    for (var i = 0; i < steps; i++)
    {
        var workload = minWorkload + (float)Math.Sin(i * stepSize) * factor;
        localData.Add(new DatasetLine(
            i + 1,
            workload,
            workload,
            avail
        ));
    }

    return localData;
}

string GetHeader()
{
    return "Id;CPU;Memory;Availability";
}

record DatasetLine(int Id, float Cpu, float Memory, float Availability)
{
    public override string ToString()
    {
        return
            $"{Id};{Cpu.ToString(CultureInfo.InvariantCulture)};{Memory.ToString(CultureInfo.InvariantCulture)};{Availability.ToString(CultureInfo.InvariantCulture)}";
    }
};
