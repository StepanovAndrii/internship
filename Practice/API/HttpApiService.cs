using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Practice.Models;

namespace Practice.API
{
    internal class HttpApiService
    {
        private string _baseUrl; 
        private string _country; 
        private string _city;
        private string _street;
        private string _description;

        private static readonly Lazy<HttpApiService> lazy = new Lazy<HttpApiService>(() => new HttpApiService()); // Створення сінглота

        public static HttpApiService Instance => lazy.Value;

        private HttpApiService() { }

        public static HttpApiService CreateInstance(string baseUrl) // Конструктор для ініціювання сінглтона
        {
            var instance = new HttpApiService();
            if (UrlValidator.ValidateURL(baseUrl))
            {
                instance._baseUrl = baseUrl;
            }
            else
            {
                DialogManager.ExitWithError("Неправильний формат базового URL.");
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
        public string Country
        {
            get
            {
                if(string.IsNullOrEmpty(_country))
                {
                    DialogManager.DisplayError("Дані про країну відсутні.");
                    return "Дані відсутні";
                }
                return _country;
            }
            set => _country = value;
        }
        public string City
        {
            get
            {
                if (string.IsNullOrEmpty(_city))
                {
                    DialogManager.DisplayError("Дані про місто відсутні.");
                    return "Дані відсутні";
                }
                return _city;
            }
            set => _city = value;
        }
        public string Street
        {
            get
            {
                if (string.IsNullOrEmpty(_street))
                {
                    DialogManager.DisplayError("Дані про вулицю відсутні.");
                    return "Дані відсутні";
                }
                return _street;
            }
            set => _street = value;
        }
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    DialogManager.DisplayError("Дані про опис відсутні.");
                    return "Дані відсутні";
                }
                return _description;
            }
            set => _description = value;
        }

        public async Task SendRequest(IEnumerable<string> subdirectories, IDictionary<string, string> requestParameters)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Practice/1.0 (WPF)");
                requestParameters.Add("q", $"{Country} {City} {Street}");

                Uri finalUri = CreateRequestURI(requestParameters, subdirectories);
                HttpResponseMessage response = httpClient.GetAsync(finalUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    JArray jsonList = new JArray();

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    jsonList = JsonConvert.DeserializeObject<JArray>(jsonResponse);

                    DbController.AddAccident(FindHighestPriorityObjectByValues(jsonList, "type", "trunk", "secondary", "bus_stop"), Description);
                }
                else
                {
                    DialogManager.DisplayError("Не вдалось з'єднатись із сервером."); 
                }
            }
        }

        /// <summary>
        /// Ініціалізує закриті поля даними.
        /// </summary>
        /// <param name="country">Назва країни де відбулась аварія.</param>
        /// <param name="city">Місто де відбулась аварія.</param>
        /// <param name="street">Вулиця на якій відбулась аварія.</param>
        /// <param name="description">Опис аварії.</param>
        public void PutOrEditData(string country, string city, string street, string description)
        {
            Country = country;
            City = city;
            Street = street;
            Description = description;
        }

        /// <summary>
        /// Створює повний запит на сервер.
        /// </summary>
        /// <param name="parameterKeyValues">Структура даних виду "ключ-значення" в яку вноситься ключі і відповідні їм значення (параметри запиту).</param>
        /// <param name="subdirectories">Шлях, по якому треба пройти додатково від базового. Відокремлюються символом "/".</param>
        /// <returns>Повністю готовий остаточний варіант url.</returns>
        private Uri CreateRequestURI(IDictionary<string, string> parameterKeyValues, params string[] subdirectories)
        {
            return CreateRequestURI(parameterKeyValues, subdirectories.ToList());
        }

        private Uri CreateRequestURI(IDictionary<string, string> parameterKeyValues, IEnumerable<string> subdirectories)
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
        /// <param name="jsonArray">Список json об'єктів серед яких буде шукатись підходящий.</param>
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
        /// <param name="jsonArray">Список json об'єктів серед яких буде шукатись підходящий.</param>
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
        /// <param name="jsonArray">Список json об'єктів серед яких буде шукатись підходящий.</param>
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
