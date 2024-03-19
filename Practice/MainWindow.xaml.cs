using Practice.API;
using System.Collections.Generic;
using System.Windows;

namespace Practice
{
    /// <summary>
    /// Логіка взаємодії для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
    }
}