using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S22.Imap;
using System.Net.Mail;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTAPIEXAMPLE
{
    class Program
    {
        // MQTT Daten
        static string MQTT_HOST = "192.168.2.200";
        static string MQTT_USER = "easydock";
        static string MQTT_PASSWORD = "2024";
        static int MQTT_PORT = 1883;

        // MAIL Daten
        static string MAIL_HOST = "imap.gmail.com";
        static string MAIL_USER = "urob.easydock.development@gmail.com";
        static string MAIL_PASS = "mumo mxti zpyb xyyr";
        static int MAIL_PORT = 993;

        // Mail Keywords
        static string[] REQUIRED_KEY_WORDS = { "Motion", "Alarm" };

        static MqttClient mqtt_client;
        static ImapClient mail_client;
        static readonly object client_lock = new object();

        public enum SignalType
        {
            REGULAR = 0,
            ALARM = 1,
        }

        static void Main(string[] args)
        {
            // Mqtt
            InitMqtt();

            // Mail
            InitMail();

            // Test
            //TriggerTestFlight();
            Thread.Sleep(999999999);
        }

        //--------------------------------------------------------------------------------
        //                                  InitMqtt
        //--------------------------------------------------------------------------------

        static void InitMqtt()
        {
            string[] topics =
            {
                "easydock/api/replys"
            };

            byte[] QOS_Level = new byte[topics.Length];
            for (int i = 0; i < QOS_Level.Length; i++)
            {
                QOS_Level[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
            }

            Console.Write("Connecting to Mqtt Server...");

            mqtt_client = new MqttClient(MQTT_HOST, MQTT_PORT, false, null, null, MqttSslProtocols.None);
            mqtt_client.MqttMsgPublishReceived += ReadMqtt;
            mqtt_client.Connect(Guid.NewGuid().ToString(), MQTT_USER, MQTT_PASSWORD);
            mqtt_client.Subscribe(topics, QOS_Level);
            Console.WriteLine("Connected!");
        }

        static void ReadMqtt(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Easydock Response: Received " + Encoding.UTF8.GetString(e.Message) + "\n");
        }

        static void InitMail()
        {
            try
            {
                Console.Write("Connecting to Mail Server...");
                mail_client = new ImapClient(MAIL_HOST, MAIL_PORT, MAIL_USER, MAIL_PASS, AuthMethod.Login, true);
                mail_client.NewMessage += new EventHandler<IdleMessageEventArgs>(ReadMail);
                Console.WriteLine("Connected!");
                Console.WriteLine("Waiting for Mails...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
            }
        }

        static void ReadMail(object sender, IdleMessageEventArgs e)
        {
            Console.WriteLine("A new message arrived with UID: " + e.MessageUID);
            MailMessage mail = mail_client.GetMessage(e.MessageUID);

            // JSON-String in ein JObject umwandeln
            JObject dataObject = JObject.Parse(mail.Body);

            var message = new
            {
                message_id = Guid.NewGuid().ToString(),
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                method = "takeoff_to_point",
                dock_sn = "7CTDM3100BZY12",
                data = dataObject
            };

            string jsonString = JsonConvert.SerializeObject(message);
            mqtt_client.Publish("easydock/api/requests", Encoding.UTF8.GetBytes(jsonString));
            Console.WriteLine("Takeoff Command send");

            //if (ContainsRequiredKeywords(message.Subject))
            //{
            //    Console.WriteLine("\n---------- ALARM ----------");
            //    Console.WriteLine("Topic: " + message.Subject);
            //    Console.WriteLine("From: " + message.From);
            //    Console.WriteLine("Date: " + message.Date());
            //    TriggerTestFlight();
            //}
        }

        static bool ContainsRequiredKeywords(string text)
        {
            return REQUIRED_KEY_WORDS.All(keyword => text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        static void TriggerTestFlight()
        {
            // Flug triggern
            //Console.WriteLine("Press Enter to start a mission");
            //Console.ReadLine();

            // Direct-Flight
            var message = new
            {
                message_id = Guid.NewGuid().ToString(),
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                method = "takeoff_to_point",
                dock_sn = "7CTDM3100BZY12",
                data = new
                {
                    lat = 52.08497782,
                    lng = 8.51040172,
                    camera_angle = 45,
                    signal_type = (int)SignalType.ALARM
                }
            };

            // Mission
            //var message = new
            //{
            //    message_id = Guid.NewGuid().ToString(),
            //    timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            //    method = "start_mission",
            //    dock_sn = "7ABDE9081232190F",
            //    data = new
            //    {
            //        mission_name = "Inspektionsflug"
            //    }
            //};

            string json = JsonConvert.SerializeObject(message);
            mqtt_client.Publish("easydock/api/requests", Encoding.UTF8.GetBytes(json));
            Console.WriteLine("Takeoff Command send");
        }
    }
}

//class Program
//{
//    private static string MQTT_HOST = "localhost";
//    private static string MQTT_USER = "user";
//    private static string MQTT_PASSWORD = "MAIL_PASS";

//    private static MqttClient client;

//    private static void ClientReceive(object sender, MqttMsgPublishEventArgs e)
//    {
//        Console.WriteLine("Received " + Encoding.UTF8.GetString(e.Message));
//    }

//    static void Main(string[] args)
//    {
//        string[] topics =
//        {
//            "easydock/api/replys"
//        };

//        byte[] QOS_Level = new byte[topics.Length];
//        for (int i = 0; i < QOS_Level.Length; i++)
//        {
//            QOS_Level[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
//        }

//        client = new MqttClient(MQTT_HOST, 1883, false, null, null, MqttSslProtocols.None);
//        client.MqttMsgPublishReceived += ClientReceive;

//        client.Connect(Guid.NewGuid().ToString(), MQTT_USER, MQTT_PASSWORD);
//        client.Subscribe(topics, QOS_Level);

//        Console.WriteLine("Press Enter to start a mission");
//        Console.ReadLine();

//        var message = new
//        {
//            message_id = Guid.NewGuid().ToString(),
//            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
//            method = "start_mission",
//            dock_sn = "7ABDE9081232190F",
//            data = new
//            {
//                mission_name = "Inspektionsflug"
//            }
//        };
//        string json = JsonConvert.SerializeObject(message);
//        client.Publish("easydock/api/requests", Encoding.UTF8.GetBytes(json));
//        Console.WriteLine("Sent message!");
//    }
//}