using System;
using System.Collections.Generic;

public class TelemetryGenerator
{
    private readonly Random _rand = new();
    private readonly string[] _sensors = new[] { "temperature", "humidity", "pressure" };
    public object GenerateTelemetry()
    {
        var telemetry = new Dictionary<string, double>();
        foreach (var sensor in _sensors)
        {
            telemetry[sensor] = Math.Round(_rand.NextDouble() * 100, 2);
        }
        return new
        {
            deviceId = Guid.NewGuid().ToString(),
            timestamp = DateTime.UtcNow.ToString("o"),
            telemetry,
            properties = new Dictionary<string, string> { { "location", "Factory-Floor-1" } }
        };
    }
    public List<object> GenerateBatch(int count)
    {
        var batch = new List<object>();
        for (int i = 0; i < count; i++) batch.Add(GenerateTelemetry());
        return batch;
    }
} 