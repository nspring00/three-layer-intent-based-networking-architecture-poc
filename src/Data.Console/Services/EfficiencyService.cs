﻿using Data.Console.Models;

namespace Data.Console.Services;

public class EfficiencyService : IEfficiencyService
{
    private const float CpuUtilizationWeight = 0.7f;
    private const float MemoryUtilizationWeight = 0.3f;

    public float ComputeAvgEfficiency(Utilization utilization)
    {
        return CpuUtilizationWeight * utilization.CpuUtilization + MemoryUtilizationWeight * utilization.MemoryUtilization;
    }
}
