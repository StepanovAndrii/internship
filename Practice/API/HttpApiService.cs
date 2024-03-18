using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Practice.Models;
using System.Windows;
using System.Net;

namespace Practice.API
{
    internal class HttpApiService
    {
        private string _baseUrl;
        private static readonly Lazy<HttpApiService> lazy = new Lazy<HttpApiService>(() => new HttpApiService());

        public static HttpApiService Instance { get { return lazy.Value; } }
        private HttpApiService() { }

        public static HttpApiService CreateInstance(string baseUrl)
        {
            var instance = new HttpApiService();
            if (UrlValidator.ValidateURL(baseUrl))
            {
                instance._baseUrl = baseUrl;
            }
            else
            {
                DialogManager.DisplayQuestion("Неправильний формат базового URL.");
            }
            return instance;
        }

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (UrlValidator.ValidateURL(value))
                {
                    _baseUrl = value;
                }
                else
                {
                    DialogManager.ExitWithError("Неправильний формат базового URL.");
                }
            }
        }

        public void SendRequest()
        {
            List<JObject> receivedJsonList = new List<JObject>();

            using (HttpClient _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Practice/1.0 (WPF)");
                _httpClient.BaseAddress = new Uri(_baseUrl);

                HttpResponseMessage response = _httpClient.GetAsync(CreateRequestURI("q=берестейський проспект", "format=json", "addressdetails=0")).Result;

                if (response.IsSuccessStatusCode)
                {
                    ProcessResponse(response, receivedJsonList);
                }
                else
                {
                    HandleErrorResponse(response);
                }
            }
        }

        private void ProcessResponse(HttpResponseMessage response, List<JObject> receivedJsonList)
        {
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            JArray jsonArray = JArray.Parse(jsonResponse);

            foreach (JObject token in jsonArray)
            {
                receivedJsonList.Add(token);
            }

            DbController.AddAccident(DeterminePriority(jsonArray));
        }

        private void HandleErrorResponse(HttpResponseMessage response)
        {
            MessageBox.Show($"Error: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private Uri CreateRequestURI(params string[] parameters)
        {
            UriBuilder uriBuilder = new UriBuilder(_baseUrl + "/search");
            uriBuilder.Query = string.Join("&", parameters);
            return uriBuilder.Uri;
        }

        private JObject DeterminePriority(JArray jsonDataArray)
        {
            List<int> init = new List<int>();
            int priority = -1;
            int position = -1;
            int temporaryPriority = -1;

            foreach (JObject json in jsonDataArray)
            {
                if (json["addresstype"].ToString() == "road" || json["addresstype"].ToString() == "highway")
                {
                    init.Add(jsonDataArray.IndexOf(json));
                }
            }
            for (int i = 0; i < init.Count; i++)
            {
                switch (jsonDataArray[init[i]]["type"].ToString())
                {
                    case "primary":
                        return (JObject)jsonDataArray[i];
                    case "trunk":
                        temporaryPriority = 5;
                        break;
                    case "secondary":
                        temporaryPriority = 4;
                        break;
                    case "bus_stop":
                        temporaryPriority = 3;
                        break;
                    case "residential":
                        temporaryPriority = 2;
                        break;
                    case "tertiary":
                        temporaryPriority = 1;
                        break;
                }
                if (temporaryPriority > priority)
                {
                    priority = temporaryPriority;
                    position = i;
                }
            }
            if (position == -1)
            {
                MessageBox.Show("Не було знайдено підходячого варіанту", "Запит", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return (JObject)jsonDataArray[position];
        }
    }
}
