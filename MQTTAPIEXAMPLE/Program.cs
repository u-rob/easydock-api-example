using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTAPIEXAMPLE
{
    class Program
    {
        private static string MQTT_HOST = "localhost";
        private static string MQTT_USER = "user";
        private static string MQTT_PASSWORD = "password";

        private static MqttClient client;

        private static void ClientReceive(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Received " + Encoding.UTF8.GetString(e.Message));
        }

        static void Main(string[] args)
        {
            string[] topics =
            {
                "easydock/api/reply"
            };

            byte[] QOS_Level = new byte[topics.Length];
            for (int i = 0; i < QOS_Level.Length; i++)
            {
                QOS_Level[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
            }

            client = new MqttClient(MQTT_HOST, 1883, false, null, null, MqttSslProtocols.None);
            client.MqttMsgPublishReceived += ClientReceive;

            client.Connect(Guid.NewGuid().ToString(), MQTT_USER, MQTT_PASSWORD);
            client.Subscribe(topics, QOS_Level);

            Console.WriteLine("Press Enter to start a mission");
            Console.ReadLine();

            var message = new
            {
                message_id = Guid.NewGuid().ToString(),
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                method = "start_mission",
                dock_sn = "7ABDE9081232190F",
                data = new
                {
                    mission_name = "Inspektionsflug"
                }
            };
            string json = JsonConvert.SerializeObject(message);
            client.Publish("easydock/api/requests", Encoding.UTF8.GetBytes(json));
            Console.WriteLine("Sent message!");
        }
    }
}
