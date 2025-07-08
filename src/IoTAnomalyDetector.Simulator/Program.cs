using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IoTAnomalyDetector.Simulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // load connection string from appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var cs = config["IoTHub:ConnectionString"];
            if (string.IsNullOrEmpty(cs))
            {
                Console.Error.WriteLine("ERROR: IoTHub:ConnectionString missing in appsettings.json");
                return;
            }

            var generator = new TelemetryGenerator(cs);
            var deviceId = Guid.Parse(config["IoTHub:DeviceId"] ?? Guid.NewGuid().ToString());

            Console.WriteLine($"Simulating device {deviceId}...");
            while (true)
            {
                await generator.SendRandomAsync(deviceId);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
