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
            InitializeHttpApiService(); // Ініціалізуємо HttpApiService
            SendHttpRequest(); // Відправляємо запит
        }

        private void InitializeHttpApiService()
        {
            string baseUrl = "https://nominatim.openstreetmap.org";
            HttpApiService.Instance.Initialize(baseUrl);
            HttpApiService.Instance.PutOrEditData("Україна", "Київ", "Солом'янська", "Опис");
            HttpApiService.Instance.AddRequestParameter("format", "json");
            HttpApiService.Instance.AddRequestParameter("addressdetails", "0");
        }

        private void SendHttpRequest()
        {
            IEnumerable<string> subdirectories = new List<string> { "search" };
            HttpApiService.Instance.SendRequest(subdirectories);
        }
    }
}
