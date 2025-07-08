# IoTAnomalyDetector.Simulator

A .NET 8 console app that simulates IoT device telemetry and sends it to Azure IoT Hub.

## Configuration

Edit `appsettings.json` with your device connection string and desired batch settings:

```
{
  "DeviceConnectionString": "<your-iot-hub-device-connection-string>",
  "BatchSize": 10,
  "BatchDelayMs": 1000,
  "MaxRetries": 3
}
```

## Running

```
dotnet run --project src/IoTAnomalyDetector.Simulator
```

## Example Payload

```
{
  "deviceId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": "2025-07-06T15:30:00Z",
  "telemetry": {
    "temperature": 23.5,
    "humidity": 68.2
  },
  "properties": { "location": "Factory-Floor-1" }
}
``` 