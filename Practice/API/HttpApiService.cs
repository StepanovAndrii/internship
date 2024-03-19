using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Practice.Models;

namespace Practice.API
{
    internal class HttpApiService
    {
        private string _baseUrl;
        private readonly Dictionary<string, string> _requestParameters;

        private static readonly Lazy<HttpApiService> lazy = new Lazy<HttpApiService>(() => new HttpApiService());
        public static HttpApiService Instance => lazy.Value;

        private HttpApiService()
        {
            _requestParameters = new Dictionary<string, string>();
        }

        public void Initialize(string baseUrl)
        {
            if (!UrlValidator.ValidateURL(baseUrl))
                DialogManager.ExitWithError("Invalid Url format.");

            _baseUrl = baseUrl;
        }

        public string BaseUrl => _baseUrl;

        public string Country { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string Description { get; private set; }

        public void PutOrEditData(string country, string city, string street, string description)
        {
            Country = country;
            City = city;
            Street = street;
            Description = description;
        }

        public async void SendRequest(IEnumerable<string> subdirectories)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Practice/1.0 (WPF)");
                AddRequestParameter("q", $"{Country} {City} {Street}");

                var finalUri = CreateRequestURI(subdirectories);
                var response = await httpClient.GetAsync(finalUri);

                if (response.IsSuccessStatusCode)
                    await HandleSuccessfulResponse(await response.Content.ReadAsStringAsync());
                else
                    DialogManager.DisplayError("Failed to connect to the server.");
            }
        }

        public void AddRequestParameter(string key, string value)
        {
            if (_requestParameters.ContainsKey(key))
                _requestParameters[key] = value;
            else
                _requestParameters.Add(key, value);
        }

        private Uri CreateRequestURI(IEnumerable<string> subdirectories)
        {
            var uriBuilder = new UriBuilder(BaseUrl)
            {
                Path = string.Join("/", subdirectories),
                Query = string.Join("&", _requestParameters.Select(kv => $"{kv.Key}={kv.Value}"))
            };
            return uriBuilder.Uri;
        }

        private async Task HandleSuccessfulResponse(string jsonResponse)
        {
            var jsonList = JArray.Parse(jsonResponse);
            var highestPriorityObject = FindHighestPriorityObjectByValues(jsonList, "type", "trunk", "secondary", "bus_stop");
            await DbController.AddAccident(highestPriorityObject, Description);
        }

        private JObject FindHighestPriorityObjectByValues(JArray jsonArray, string key, params string[] priorityList)
        {
            foreach (var value in priorityList)
            {
                var jsonObject = FindSuitableJsonObject(jsonArray, key, value);
                if (jsonObject.HasValues)
                    return jsonObject;
            }
            return new JObject();
        }

        private JObject FindSuitableJsonObject(JArray jsonArray, string key, string value)
        {
            return jsonArray.Children<JObject>().FirstOrDefault(obj => obj.SelectToken(key)?.ToString() == value) ?? new JObject();
        }
    }
}
