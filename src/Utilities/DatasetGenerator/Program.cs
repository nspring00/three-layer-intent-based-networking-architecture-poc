﻿// See https://aka.ms/new-console-template for more information

using System.Globalization;

Create(GenerateOscillatingEffDataset(), "SinEfficiency.csv");
Create(GenerateOscillatingAvailDataset(), "SinAvailability.csv");
Create(GenerateOscillatingFullDataset(), "SinFull.csv");

void Create(IEnumerable<DatasetLine> data, string filename)
{
    var lines = data.Select(x => x.ToString()).ToList();
    lines.Insert(0, GetHeader());
    
    File.WriteAllLines(filename, lines);
}


List<DatasetLine> GenerateOscillatingEffDataset()
{
    const int minWorkload = 450;
    const float avail = 0.8f;

    const int steps = 400;
    const double stepSize = 2 * Math.PI / steps;
    const int factor = 200;

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

List<DatasetLine> GenerateOscillatingAvailDataset()
{
    const int workload = 20;
    const float minAvail = 0.3f;
    
    const int steps = 400;
    const double stepSize = 2 * Math.PI / steps;
    const float factor = 0.1f;
    
    var localData = new List<DatasetLine>();

    for (var i = 0; i < steps; i++)
    {
        var avail = minAvail + (float)Math.Sin(i * stepSize) * factor;
        localData.Add(new DatasetLine(
            i + 1,
            workload,
            workload,
            avail
        ));
    }

    return localData;
}

List<DatasetLine> GenerateOscillatingFullDataset()
{
    // const int workload = 20;
    const int minWorkload = 30;
    const float minAvail = 0.3f;
    
    const int steps = 400;
    const double stepSize = 2 * Math.PI / steps;
    const float availFactor = 0.1f;
    const float workloadFactor = 20;
    
    var localData = new List<DatasetLine>();

    for (var i = 0; i < steps; i++)
    {
        var avail = minAvail + (float)Math.Sin(i * stepSize) * availFactor;
        // var workload = minWorkload + (float)Math.Sin(Math.PI + i * stepSize) * workloadFactor; // Full_1
        var workload = minWorkload + (float)Math.Sin(2 * i * stepSize) * workloadFactor; // Full_2
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
