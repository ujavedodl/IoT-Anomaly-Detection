using Microsoft.Azure.Devices.Client;
using System.Text.Json;


namespace IoTAnomalyDetector.Simulator
{
    public class TelemetryGenerator
    {
        private readonly DeviceClient _client;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public TelemetryGenerator(string connectionString)
        {
            _client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        }

        public async Task SendRandomAsync(Guid deviceId)
        {
            var payload = new IoTDeviceTelemetry
            {
                DeviceId = deviceId,
                Timestamp = DateTime.UtcNow,
                Telemetry = new Dictionary<string, double>
                {
                    ["temperature"] = Random.Shared.NextDouble() * 40,
                    ["humidity"] = Random.Shared.NextDouble() * 100
                }
            };

            string json = JsonSerializer.Serialize(payload, _jsonOptions);
            using var message = new Message(System.Text.Encoding.UTF8.GetBytes(json))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };

            // simple retry loop
            int retries = 3;
            while (true)
            {
                try
                {
                    await _client.SendEventAsync(message);
                    Console.WriteLine($"Sent: {json}");
                    break;
                }
                catch (Exception ex) when (retries-- > 0)
                {
                    Console.WriteLine($"Send failed ({ex.Message}), retrying...");
                    await Task.Delay(1000);
                }
            }
        }

        // Mirror your JSON schema as a C# record
        public class IoTDeviceTelemetry
        {
            public Guid DeviceId { get; set; }
            public DateTime Timestamp { get; set; }
            public IDictionary<string, double> Telemetry { get; set; } = new Dictionary<string, double>();
            public IDictionary<string, string>? Properties { get; set; }
        }

    }


}
