using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Practice.Models;
using System.Windows;
using System.Net;
using System;

namespace Practice.API
{
    internal class ApiService
    {
        private string _baseUrl;

        public ApiService(string baseUrl)
        {
            if (!ValidateURL(baseUrl))
            {
                MessageBox.Show("Неправильний формат базового url. Для виходу з додатку натисність кнопку \"OK\".", "Practice.exe - Помилка додатку", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.None, MessageBoxOptions.None);
                Application.Current.Shutdown();
            }
            _baseUrl = baseUrl;
        }

        //public string BaseUrl
        //{
        //    get => _baseUrl ?? "none";
        //    set => _baseUrl = ValidateURL(value);
        //}

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
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JArray jsonArray = JArray.Parse(jsonResponse); 

                    foreach (JObject token in jsonArray)
                    {
                        receivedJsonList.Add(token); 
                    }

                    DbController.AddAccident(DeterminePriority(jsonArray));
                }
                else
                {
                    MessageBox.Show($"Error: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Uri CreateRequestURI(Dictionary<string, string> parameters)
        {
            UriBuilder uriBuilder = new UriBuilder(_baseUrl + "/search");

            List<string> parameterList = new List<string>();
            foreach (var parameter in parameters)
            {
                string encodedValue = WebUtility.UrlEncode(parameter.Value);
                string encodedParameter = $"{parameter.Key}={encodedValue}";
                parameterList.Add(encodedParameter);
            }

            string queryString = string.Join("&", parameterList);
            uriBuilder.Query = queryString;

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
            if(position == -1)
            {
                MessageBox.Show("Не було знайдено підходячого варіанту", "Запит", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return (JObject)jsonDataArray[position];
        }

        private Uri CreateRequestURI(params string[] parameters)
        {
            UriBuilder uriBuilder = new UriBuilder(_baseUrl + "/search");

            string queryString = string.Join("&", parameters);
            uriBuilder.Query = queryString;

            return uriBuilder.Uri;
        }

        private bool ValidateURL(string url)
        {
            return Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
        }
    }
}