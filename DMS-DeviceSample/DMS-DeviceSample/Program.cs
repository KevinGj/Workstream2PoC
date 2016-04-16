using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace DMS_DeviceSample
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "agder-workstream2.azure-devices.net";
        static string deviceKey = "+dtUr0luEA2eezq4FIlrDmS68vjK36m6+w1+vWad6Wk=";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("DMSDevice", deviceKey));

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        // sends a new device-to-cloud message every second.
        // The message contains a JSON-serialized object with the deviceId 
        // and a randomly generated number to simulate a killowatt output
        private static async void SendDeviceToCloudMessagesAsync()
        {
            int kWh = 1; // m/s
            Random rand = new Random();
            int maxValue = 10;

            while (true)
            {
                int currentKW = kWh + rand.Next(maxValue) * 2 - 2;

                if (currentKW > 0)
                { 
                    var telemetryDataPoint = new
                    {
                        deviceId = "DMSDevice",
                        killowatts = currentKW
                    };

                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    await deviceClient.SendEventAsync(message);
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                    Task.Delay(1000).Wait();
                }
            }
        }
    }
}
