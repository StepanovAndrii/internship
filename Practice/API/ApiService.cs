
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


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
        public async Task<string> GetData(string adress)
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
                return jsonDataInStringFormat;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /*
                    JsonDocument jsonDocument = JsonDocument.Parse(json);
                    JsonElement root = jsonDocument.RootElement;

                    List<Dictionary<string, object>> jsonArray = new List<Dictionary<string, object>>();

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in root.EnumerateArray())
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();

                            foreach (JsonProperty prop in element.EnumerateObject())
                            {
                                dict.Add(prop.Name, GetValue(prop.Value));
                            }

                            jsonArray.Add(dict);
                        }
                    }
                    foreach (var item in jsonArray)
                    {
                        foreach (var kvp in item)
                        {
                            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                        }
                        Console.WriteLine();
                    }
        */
        static object GetValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue))
                        return intValue;
                    if (element.TryGetDouble(out double doubleValue))
                        return doubleValue;
                    return element.GetDecimal();
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    throw new NotSupportedException($"Unsupported JSON value kind: {element.ValueKind}");
            }
        }
    }
}
