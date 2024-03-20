using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using Practice.API;
using Practice.Models;

namespace Practice
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HttpApiService.Instance.BaseUrl = "https://nominatim.openstreetmap.org";
            HttpApiService.Instance.AddRequestParameter("format", "json");
            HttpApiService.Instance.AddRequestParameter("addressdetails", "0");
            HttpApiService.Instance.AddRequestParameter("q", "");
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpApiService.Instance.PutOrEditData(country.Text, town.Text, street.Text, Comment.Text);
            HttpApiService.Instance.AddSearchRequestParameter();
            HttpApiService.Instance.SendRequest("Practice", 1.0, "WPF");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IEnumerable<Accident> accidents = await DbController.GetAllAccidents();

            foreach (Accident accident in accidents)
            {
                GMapMarker marker = new GMapMarker(new PointLatLng(accident.Latitude, accident.Longitude))
                {
                    Shape = new Image
                    {
                        Source = new BitmapImage(new Uri("images/marker.png", UriKind.Relative)),
                        ToolTip = $"ДТП: {accident.Description}",
                        Visibility = Visibility.Visible
                    }
                };
                gmap.Markers.Add(marker);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gmap.Markers.Clear();
            DbController.ClearAllAccidents();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = "";
            tb.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Введіть дані";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}