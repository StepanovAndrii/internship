using Practice.API;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net;

//using System.Device.Location

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Practice
{
    /// <summary>
    /// Логіка взаємодії для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class CPoint
        {
            public double x { get; set; }
            public double y { get; set; }

            public CPoint() { }

            public CPoint(double _x, double _y)
            {
                x = _x;
                y = _y;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //HttpApiService.Instance.BaseUrl = "https://nominatim.openstreetmap.org";
            //HttpApiService.Instance.AddRequestParameter("format", "json");
            //HttpApiService.Instance.AddRequestParameter("addressdetails", "0");
            //HttpApiService.Instance.PutOrEditData("Україна", "Київ", "Миколи Міхновського", "ДТП на ній");
            //HttpApiService.Instance.AddSearchRequestParameter();
            //HttpApiService.Instance.SendRequest("Practice", 1.0, "WPF");
        }

        private void map_load(object sender, EventArgs e)
        {
            gmap.Bearing = 0;
            gmap.CanDragMap = true;
            gmap.DragButton = MouseButton.Left;

            gmap.MaxZoom = 18;
            gmap.MinZoom = 2;
            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;

            gmap.ShowTileGridLines = false;
            gmap.Zoom = 10;
            gmap.ShowCenter = false;

            gmap.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(50.4501, 30.5234);

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new PointLatLng(50.4015971, 30.6237091));

            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(@"C:\Users\anrub\Desktop\учеба 2.0\Практика\Задание\Practice\images\marker.png");
            //bitmap.EndInit();

            //Image image = new Image();
            //image.Source = bitmap;
            //marker.Shape = image;
            ////marker.Offset = new Point(image.Width / 2, -image.Height);

            //gmap.Markers.Add(marker);

            //marker.Shape = new Image
            //{
            //    Source = new BitmapImage(new Uri(@"C:\Users\anrub\Desktop\учеба 2.0\Практика\Задание\Practice\images\marker.png")),
            //    ToolTip = "#1 ДТП",
            //    Visibility = Visibility.Visible
            //};
            //gmap.Markers.Add(marker);
        }

        List<CPoint> List = new List<CPoint>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string[] ArrayOfStringWithCoor = File.ReadAllLines(@"places.txt", Encoding.GetEncoding(1251));
            for (int i = 0; i < ArrayOfStringWithCoor.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringWithCoor[i].Split(new char[] { ';' });
                List.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            for (int i = 0; i < List.Count; i++)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(List[i].x, List[i].y));
                marker.Shape = new Image
                {
                    Source = new BitmapImage(new Uri("images/marker.png", UriKind.Relative)),
                    ToolTip = $"#{i + 1} ДТП",
                    Visibility = Visibility.Visible
                };
                gmap.Markers.Add(marker);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //gmap.Markers.Clear();
            //List.Clear();
            string url = "https://nominatim.openstreetmap.org/search?q=%D0%9A%D0%B8%D0%B5%D0%B2&format=json&addressdetails=0";
            HttpRequestExample requestExample = new HttpRequestExample();
            List<string> coordinatesList = await requestExample.GetCoordinatesAsync(url);

            // Объединяем список координат в одну строку с помощью метода Join
            string coordinatesString = string.Join("\n", coordinatesList);

            // Обновляем содержимое Label
            //CoorOfStreet.Content = coordinatesString;

            foreach (string coordinates in coordinatesList)
            {
                // Разбиваем строку на широту и долготу
                string[] parts = coordinates.Split(',');
                string latString = parts[0].Split(':')[1].Trim().Replace('.', ',');
                string lonString = parts[1].Split(':')[1].Trim().Replace('.', ',');

                // Преобразуем строковые значения широты и долготы в числовой формат
                if (double.TryParse(latString, out double lat) && double.TryParse(lonString, out double lon))
                {
                    // Создаем маркер и добавляем его на карту
                    GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new PointLatLng(lat, lon));
                    // Здесь вы можете настроить маркер, добавить его на карту и т.д.
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(@"C:\Users\anrub\Desktop\учеба 2.0\Практика\Задание\Practice\images\marker.png");
                    bitmap.EndInit();

                    Image image = new Image();
                    image.Source = bitmap;
                    marker.Shape = image;
                    gmap.Markers.Add(marker);
                }
                else
                {
                    // Обработка ошибок преобразования строки в числовой формат
                    Console.WriteLine($"Ошибка преобразования координат: {coordinates}");
                }
            }
        }

        public class HttpRequestExample
        {
            public async Task<string> MakeRequestAsync(string url)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "C# Nominatim API Client");
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode(); // Проверяем успешность запроса

                        // Читаем содержимое ответа в виде строки
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody;
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Обработка ошибок запроса
                    return $"HttpRequestException: {ex.Message}";
                }
                catch (Exception ex)
                {
                    // Обработка других исключений
                    return $"Exception: {ex.Message}";
                }
            }

            public async Task<List<string>> GetCoordinatesAsync(string url)
            {
                List<string> coordinatesList = new List<string>(); // Создаем список для хранения координат

                try
                {
                    // Получаем JSON-ответ с помощью HttpClient
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "C# Nominatim API Client");
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode(); // Проверяем успешность запроса
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Преобразуем JSON-ответ в массив объектов
                        JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                        // Проверяем, что корневой элемент - массив
                        if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            // Проходим по всем элементам массива
                            foreach (JsonElement element in jsonDocument.RootElement.EnumerateArray())
                            {
                                // Извлекаем координаты из объекта и добавляем их в список
                                string lat = element.GetProperty("lat").GetString();
                                string lon = element.GetProperty("lon").GetString();
                                coordinatesList.Add($"Latitude: {lat}, Longitude: {lon}");
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Обработка ошибок запроса
                    coordinatesList.Add($"HttpRequestException: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    // Обработка ошибок разбора JSON
                    coordinatesList.Add($"JsonException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Обработка других исключений
                    coordinatesList.Add($"Exception: {ex.Message}");
                }

                return coordinatesList; // Возвращаем список координат
            }


        }
    }
}