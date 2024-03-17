using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Practice.API
{
    internal class ApiService
    {
        private string _baseUrl;

        public ApiService(string baseUrl)                                                                      
        {
            _baseUrl = ValidateURL(baseUrl);                                    
        }

        public string BaseUrl
        {
            get => _baseUrl ?? "none";
            set => _baseUrl = ValidateURL(value);
        }

        public void SendRequest()
        {
            List<JObject> receivedJsonList = new List<JObject>();

            using (HttpClient _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Practice/1.0 (WPF)");
                _httpClient.BaseAddress = new Uri(_baseUrl);

                HttpResponseMessage response = _httpClient.GetAsync(CreateRequestURI("q=соломенская", "format=json", "addressdetails=0")).Result;

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JArray jsonArray = JArray.Parse(jsonResponse); 

                    foreach (JObject token in jsonArray)
                    {
                        receivedJsonList.Add(token); 
                    }

                    MessageBox.Show((string)(receivedJsonList[0]["osm_id"]), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private Uri CreateRequestURI(params string[] parameters)
        {
            UriBuilder uriBuilder = new UriBuilder(_baseUrl + "/search");

            string queryString = string.Join("&", parameters);
            uriBuilder.Query = queryString;

            return uriBuilder.Uri;
        }

        private string ValidateURL(string url)
        {
            if (!Regex.Match(url, "^http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?$").Success)
            {
                MessageBox.Show("You have provided a base URL in an incorrect format", "Provision of data", MessageBoxButton.OK, MessageBoxImage.Error);;
                return null;
            }
            return url;
        }
    }
}