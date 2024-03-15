using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

//ruban 
namespace Practice.API
{
    internal class ApiService
    {
        // Fields
        private string _baseUrl;

        // Constructors
        public ApiService(string baseUrl = "https://nominatim.openstreetmap.org/")
        {
            _baseUrl = baseUrl;
        }

        // Methods
        public void ChangeBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public async Task GetData(string adress)
        {
            try
            {
                string userAgentInfo = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36";
                string jsonDataInStringFormat;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", userAgentInfo);
                    Uri endpoint = new Uri($"{_baseUrl}search?q={adress}&format=json&addressdetails=0");

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                    HttpResponseMessage response = await client.GetAsync(endpoint, cts.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonDataInStringFormat = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                ConvertStringJsonToObject(jsonDataInStringFormat);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void ConvertStringJsonToObject(string json)
        {
            try
            {
                var jsonArray = JsonSerializer.Deserialize<object[]>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
