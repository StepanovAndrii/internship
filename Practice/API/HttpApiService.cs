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

        /// <summary>
        /// Базовий URL для HTTP-запитів.
        /// </summary>
        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (!UrlValidator.ValidateURL(value))
                {
                    DialogManager.NotifyExit("Неправильний формат URL.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
                _baseUrl = value;
            }
        }

        /// <summary>
        /// Країна, що використовується для HTTP-запитів.
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Місто, що використовується для HTTP-запитів.
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// Вулиця, що використовується для HTTP-запитів.
        /// </summary>
        public string Street { get; private set; }

        /// <summary>
        /// Опис, що використовується для HTTP-запитів.
        /// </summary>
        public string Description { get; private set; }

        // Методи

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

        /// <summary>
        /// Відправляє HTTP-запит з використанням вказаного агента користувача.
        /// </summary>
        public async void SendRequest(string userAgentProduct, double userAgentProductVersion, string userAgentComment)
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                DialogManager.NotifyExit("Базовий URL відсутній.", MessageBoxImage.Error, MessageBoxButton.OK);
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

        /// <summary>
        /// Додає параметр до HTTP-запиту.
        /// </summary>
        /// <param name="key">Ключ параметра.</param>
        /// <param name="value">Значення параметра.</param>
        public void AddRequestParameter(string key, string value)
        {
            _requestParameters[key] = value;
        }

        /// <summary>
        /// Додає піддиректорію до HTTP-запиту.
        /// </summary>
        /// <param name="subdirectory">Піддиректорія.</param>
        public void AddRequestSubdirectory(string subdirectory)
        {
            _requestSubdirecories.Add(subdirectory);
        }

        /// <summary>
        /// Додає параметр пошуку до HTTP-запиту на основі встановлених даних про вулицю.
        /// </summary>
        public void AddSearchRequestParameter()
        {
            if (string.IsNullOrEmpty(Street))
            {
                DialogManager.Notify("Дані відсутні.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
            else
                _requestParameters["q"] = $"{Country} {City} {Street}";
        }

        // Приватні методи

        /// <summary>
        /// Створює URI для HTTP-запиту на основі встановлених параметрів.
        /// </summary>
        private Uri CreateRequestURI()
        {
            UriBuilder uriBuilder = new UriBuilder(BaseUrl);

            uriBuilder.Path = _requestSubdirecories.Any() ? string.Join("/", _requestSubdirecories) : uriBuilder.Path;
            uriBuilder.Query = _requestParameters.Any() ? string.Join("&", _requestParameters.Select(parameter => $"{parameter.Key}={parameter.Value}")) : uriBuilder.Query;

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Обробляє успішну відповідь на HTTP-запит.
        /// </summary>
        /// <param name="jsonResponse">JSON-відповідь.</param>
        private async Task HandleSuccessfulResponse(string jsonResponse)
        {
            JArray jsonList = JArray.Parse(jsonResponse);

            JObject priorityObject = FindHighestPriorityObjectByValues(jsonList, "type", _suitableKeyValues);
            if (priorityObject.HasValues)
            {
                await DbController.AddAccident(priorityObject, Description);
            }
            else
            {
                DialogManager.Notify("Вулиці не було знайдено.", MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Знаходить об'єкт з найвищим пріоритетом за заданими значеннями.
        /// </summary>
        /// <param name="jsonArray">Масив JSON-об'єктів.</param>
        /// <param name="key">Ключ, за яким шукати значення.</param>
        /// <param name="values">Список значень, за якими шукати.</param>
        /// <returns>Об'єкт JSON з найвищим пріоритетом.</returns>
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

        /// <summary>
        /// Знаходить відповідний JSON-об'єкт за значенням ключа.
        /// </summary>
        /// <param name="jsonArray">Масив JSON-об'єктів.</param>
        /// <param name="key">Ключ для пошуку.</param>
        /// <param name="value">Значення ключа для пошуку.</param>
        /// <returns>Відповідний JSON-об'єкт або порожній об'єкт, якщо не знайдено.</returns>
        private JObject FindSuitableJsonObject(JArray jsonArray, string key, string value)
        {
            return jsonArray.Children<JObject>().FirstOrDefault(obj => obj.SelectToken(key)?.ToString() == value) ?? new JObject();
        }
    }
}
