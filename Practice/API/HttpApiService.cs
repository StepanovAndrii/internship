using System;
using System.Net.Http;
using Practice.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
                DialogManager.DisplayError("Неправильний формат базового URL.");
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

        public void SendRequest(string country, string city, string street, string description)
        {
            List<JObject> receivedJSON;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "format", "json" },
                { "addressdetails", "0" },
                { "q", $"{country} {city} {street}" }
            };

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Practice/1.0 (WPF)");

                Uri finalUri = CreateRequestURI(parameters, "search");
                HttpResponseMessage response = httpClient.GetAsync(finalUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    receivedJSON = JsonConvert.DeserializeObject<List<JObject>>(jsonResponse);
                    ProcessResponse(response, receivedJSON, "my desc");
                }
                else
                {
                    
                }
            }
        }

        private void ProcessResponse(HttpResponseMessage response, List<JObject> receivedJsonList, string description)
        {
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            JArray jsonArray = JArray.Parse(jsonResponse);

            foreach (JObject json in jsonArray)
            {
                receivedJsonList.Add(json);
            }

            // DbController.AddAccident(FindHighestPriorityObjectByValues(jsonArray, key: "type", "primary", "trunk", "secondary", "bus_stop", "residential", "tetriary", "pedestrian"), description);
        }

        /// <summary>
        /// Створює повний запит на сервер.
        /// </summary>
        /// <param name="parameterKeyValues">Структура даних виду "ключ-значення" в яку вноситься ключі і відповідні їм значення (параметри запиту).</param>
        /// <param name="subdirectories">Шлях, по якому треба пройти додатково від базового. Відокремлюються символом "/".</param>
        /// <returns>Повністю готовий остаточний варіант url.</returns>
        private Uri CreateRequestURI(IDictionary<string, string> parameterKeyValues, params string[] subdirectories)
        {
            UriBuilder uri = new UriBuilder(BaseUrl); // Ініціалізація uri з базовим шляхом в основі

            uri.Path = string.Join("/", subdirectories); // Об'єднує піддиректорії символом "/"
            uri.Query = string.Join("&", parameterKeyValues.Select(kv => $"{kv.Key}={kv.Value}")); // Об'єднує параметри ключ-значення символом "&"

            return uri.Uri;
        }

        /// <summary>
        /// Знаходить перший JSON який підійшов по умові (має значення ключа яке є в наданому перечисленні <paramref name="priorityList"/>
        /// </summary>
        /// <remarks>Є обгорткою-перегрузом навколо оригінального методу. Порядок наданих значень має значення.</remarks>
        /// <param name="jsonArray">Список json об'єктів черед яких буде шукатись підходящий.</param>
        /// <param name="key">Ключ, по якому будуть перебиратись підходящі значення.</param>
        /// <param name="priorityList">Не обов'язкові параметри (массив) усіх підходящих умові значень.</param>
        /// <returns>JSON об'єкт який перший підійде по значенням з параметру <paramref name="priorityList"/>.</returns>
        private JObject FindHighestPriorityObjectByValues(JArray jsonArray, string key, params string[] priorityList)
        {
            return FindHighestPriorityObjectByValues(jsonArray, key, priorityList.ToList<string>());
        }

        /// <summary>
        /// Знаходить перший JSON який підійшов по умові (має значення ключа яке є в наданому перечисленні <paramref name="priorityList"/>). 
        /// </summary>
        /// <remarks>В більшості випадків враховується порядок в <paramref name="priorityList"/>. Значення яке буде найперше - буде найпріорітетнішим.</remarks>
        /// <param name="jsonArray">Список json об'єктів черед яких буде шукатись підходящий.</param>
        /// <param name="key">Ключ, по якому будуть перебиратись підходящі значення.</param>
        /// <param name="priorityList">Список усіх підходящих умові значень.</param>
        /// <returns>JSON об'єкт який перший підійде по значенням з параметру <paramref name="priorityList"/>.</returns>
        private JObject FindHighestPriorityObjectByValues(JArray jsonArray, string key, IEnumerable<string> priorityList)
        {
            JObject answer;
            foreach (string value in priorityList) // Проходимось по кожному значенню
            {
                answer = FindSuitableJsonObject(jsonArray, key, value);
                if (answer.HasValues) // Перевіряємо чи знайшли ми щось (чи не пустий). Якщо знайшли - виходимо з методу і повертаємо цей об'єкт
                {
                    return answer; 
                }
            }
            return new JObject(); // Якщо все таки не вийшло знайти потрібного - повертаємо пустий
        }

        /// <summary>
        /// Обирає об'єкт JSON серед купи який підходить під умови (певний ключ має певне значення).
        /// </summary>
        /// <param name="jsonArray">Список json об'єктів черед яких буде шукатись підходящий.</param>
        /// <param name="key">Ключ в json об'єктах у якому буде шукатись значення.</param>
        /// <param name="value">Потрібне значення ключа.</param>
        /// <returns>Перший JSON об'єкт який має значення <paramref name="value"/> в ключі <paramref name="key"/>.</returns>
        private JObject FindSuitableJsonObject(JArray jsonArray, string key, string value)
        {
            JObject searchResult = jsonArray.Children<JObject>().FirstOrDefault(obj => obj.SelectToken(key)?.ToString() == value); // За допомогою linq запиту шукається перший підходящий об'єкт json, а якщо не знаходиться повертається null

            if (searchResult != null) // Якщо об'єкт знайшовся, метод припиняє роботу і повертає цей об'єкт
            {
                return searchResult;
            }
            return new JObject(); // Якщо об'єкт не знаходиться, повертається пустий об'єкт JSON ({})
        }
    }
}
