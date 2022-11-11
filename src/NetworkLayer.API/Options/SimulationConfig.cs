namespace NetworkLayer.API.Options;

public class SimulationConfig
{
    public string DataSetPath { get; set; } = "dataset.csv";
    public int InitialCount { get; set; }
    public float Jitter { get; set; } = 0.05f;
    public int JitterSeed { get; set; } = 1234;
}
