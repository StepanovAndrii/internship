using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows;
using Newtonsoft.Json;

namespace Practice.API
{
    /// <summary>
    /// Клас, що забезпечує доступ до взаємодії з HTTP API.
    /// </summary>
    internal class HttpApiService
    {
        private string _baseUrl;
        private readonly Dictionary<string, string> _requestParameters;
        private readonly List<string> _requestSubdirecories;
        private readonly List<string> _suitableKeyValues = new List<string>
        {
            "primary",
            "trunk",
            "secondary",
            "bus_stop",
            "residential",
            "tertiary"
        };

        // Паттерн Singleton для HttpApiService
        private static readonly Lazy<HttpApiService> lazy = new Lazy<HttpApiService>(() => new HttpApiService());
        public static HttpApiService Instance => lazy.Value;

        private HttpApiService()
        {
            _requestParameters = new Dictionary<string, string>();
            _requestSubdirecories = new List<string>();
        }

        // Властивості
        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (!UrlValidator.ValidateURL(value))
                {
                    DialogManager.NotifyExit("Неправильний формат url.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
                _baseUrl = value;
            }
        }
        public string Country { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string Description { get; private set; }

        // Метод для встановлення даних
        /// <summary>
        /// Встановлює дані для відправки запиту HTTP.
        /// </summary>
        /// <param name="country">Країна.</param>
        /// <param name="city">Місто.</param>
        /// <param name="street">Вулиця.</param>
        /// <param name="description">Опис.</param>
        public void PutOrEditData(string country, string city, string street, string description)
        {
            Country = country;
            City = city;
            Street = street;
            Description = description;
        }

        // Метод для відправки HTTP-запиту
        /// <summary>
        /// Відправляє HTTP-запит.
        /// </summary>
        public async void SendRequest(string userAgentProduct, double userAgentProductVersion, string userAgentComment)
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                DialogManager.NotifyExit("Базовий url відсутній.", MessageBoxImage.Error, MessageBoxButton.OK);
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", $"{userAgentProduct}/{userAgentProductVersion} ({userAgentComment})");

                Uri finalUri = CreateRequestURI();

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(finalUri);
                    response.EnsureSuccessStatusCode(); // Викине HttpRequestException, якщо статус не успішний
                    await HandleSuccessfulResponse(await response.Content.ReadAsStringAsync());
                }
                catch (HttpRequestException)
                {
                    DialogManager.NotifyExit("Не вдалось зв'язатись з сервером.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
                catch (TaskCanceledException)
                {
                    DialogManager.NotifyExit("Таймаут на з'єднання з сервером.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
                catch (JsonReaderException)
                {
                    DialogManager.NotifyExit("Помилка при обробці JSON відповіді сервера.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
                catch (InvalidOperationException)
                {
                    DialogManager.NotifyExit("Помилка при зчитуванні відповіді сервера.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }


        // Метод для додавання параметра запиту
        /// <summary>
        /// Додає параметр запиту.
        /// </summary>
        /// <param name="key">Ключ параметра.</param>
        /// <param name="value">Значення параметра.</param>
        public void AddRequestParameter(string key, string value)
        {
            _requestParameters[key] = value;
        }

        public void AddRequestSubdirectory(string subdirectory)
        {
            _requestSubdirecories.Add(subdirectory);
        }

        public void AddSearchRequestParameter()
        {
            if (string.IsNullOrEmpty(Street))
            {
                DialogManager.Notify("Дані відсутні.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
            else
                _requestParameters.Add("q", $"{Country} {City} {Street}");
        }
        // Метод для створення URI запиту
        private Uri CreateRequestURI()
        {
            UriBuilder uriBuilder = new UriBuilder(BaseUrl);

            uriBuilder.Path = _requestSubdirecories.Any() ? string.Join("/", _requestSubdirecories) : uriBuilder.Path;
            uriBuilder.Query = _requestParameters.Any() ? string.Join("&", _requestParameters.Select(parameter => $"{parameter.Key}={parameter.Value}")) : uriBuilder.Query;

            return uriBuilder.Uri;
        }

        // Метод для обробки успішної відповіді
        private async Task HandleSuccessfulResponse(string jsonResponse)
        {
            JArray jsonList = JArray.Parse(jsonResponse);

            JObject priorityObject = FindHighestPriorityObjectByValues(jsonList, "type", _suitableKeyValues);
            if(priorityObject.HasValues)
            {
                await DbController.AddAccident(priorityObject, Description);
            }
            else
            {
                DialogManager.Notify("Вулиці не було знайдено.", MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }

        // Метод для пошуку об'єкта з найвищим пріоритетом за значеннями
        private JObject FindHighestPriorityObjectByValues(JArray jsonArray, string key, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                var jsonObject = FindSuitableJsonObject(jsonArray, key, value);
                if (jsonObject.HasValues)
                    return jsonObject;
            }
            return new JObject();
        }

        // Метод для пошуку відповідного об'єкта JSON
        private JObject FindSuitableJsonObject(JArray jsonArray, string key, string value)
        {
            return jsonArray.Children<JObject>().FirstOrDefault(obj => obj.SelectToken(key)?.ToString() == value) ?? new JObject();
        }
    }
}
