using Newtonsoft.Json;

namespace RESTAPIEXAMPLE
{
    class Program
    {

        public const string REST_API_HOST = "https://localhost:7187/";

        static async Task Main(string[] args)
        {
            var body = new
            {
                message_id = "abc",
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                dock = "Dock am Firmeneingang",
                data = new
                {
                    lat = 20.0,
                    lng = 13.0
                }
            };

            string json = JsonConvert.SerializeObject(body);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(REST_API_HOST);

            HttpContent httpContent = new StringContent(json);

            await client.PostAsync("api/takeoff_to_point", httpContent);
        }
    }
}
