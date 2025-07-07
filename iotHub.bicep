@description('Name of the IoT Hub')
param iotHubName string = 'iot-anom-hub'

@description('Azure location for resources')
param location string = resourceGroup().location

resource hub 'Microsoft.Devices/IotHubs@2021-07-02' = {
  name: iotHubName
  location: location
  sku: {
    name     : 'S1'
    capacity : 1
  }
  properties: {
    enableFileUploadNotifications: false
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount      : 2
      }
    }
  }
}

output hostName         string = hub.properties.hostName
output primaryConnString string = listKeys(hub.id, '2021-07-02').primaryConnectionString
