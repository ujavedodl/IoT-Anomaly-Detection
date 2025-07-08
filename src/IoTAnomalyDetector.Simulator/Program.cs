using Microsoft.Azure.Devices.Client;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string deviceConnectionString = config["DeviceConnectionString"] ?? throw new Exception("DeviceConnectionString missing");
        int batchSize = int.TryParse(config["BatchSize"], out var b) ? b : 10;
        int batchDelayMs = int.TryParse(config["BatchDelayMs"], out var d) ? d : 1000;
        int maxRetries = int.TryParse(config["MaxRetries"], out var r) ? r : 3;

        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        var generator = new TelemetryGenerator();

        while (true)
        {
            var batch = generator.GenerateBatch(batchSize);
            foreach (var telemetry in batch)
            {
                string json = JsonSerializer.Serialize(telemetry);
                int attempt = 0;
                bool sent = false;
                while (!sent && attempt < maxRetries)
                {
                    try
                    {
                        using var msg = new Message(System.Text.Encoding.UTF8.GetBytes(json));
                        await deviceClient.SendEventAsync(msg);
                        sent = true;
                    }
                    catch (Exception ex)
                    {
                        attempt++;
                        if (attempt >= maxRetries) Console.WriteLine($"Failed to send: {ex.Message}");
                        else await Task.Delay(500 * attempt);
                    }
                }
            }
            await Task.Delay(batchDelayMs);
        }
    }
}
