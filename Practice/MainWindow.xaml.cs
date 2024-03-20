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
using Practice.API;
using System.Data.Common;

namespace Practice
{
    public partial class MainWindow : Window
    {
        List<CPoint> List = new List<CPoint>();

        public MainWindow()
        {
            InitializeComponent();
            HttpApiService.Instance.BaseUrl = "https://nominatim.openstreetmap.org";
            HttpApiService.Instance.AddRequestParameter("format", "json");
            HttpApiService.Instance.AddRequestParameter("addressdetails", "0");
            HttpApiService.Instance.PutOrEditData("Україна", "Київ", "Миколи Міхновського", "ДТП на ній");
            HttpApiService.Instance.AddSearchRequestParameter();
            HttpApiService.Instance.SendRequest("Practice", 1.0, "WPF");
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
        }

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

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    // Создаем маркер и добавляем его на карту
        //    GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new PointLatLng(lat, lon));
        //    // Здесь вы можете настроить маркер, добавить его на карту и т.д.
        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.UriSource = new Uri(@"C:\Users\anrub\Desktop\учеба 2.0\Практика\Задание\Practice\images\marker.png");
        //    bitmap.EndInit();

        //    Image image = new Image();
        //    image.Source = bitmap;
        //    marker.Shape = image;
        //    gmap.Markers.Add(marker);
        //}

        private void Button_Click3(object sender, RoutedEventArgs e)
        {

        }
    }

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
}

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