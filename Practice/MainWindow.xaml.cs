using Practice.API;
using System.Collections.Generic;
using System.Windows;

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HttpApiService apiService = HttpApiService.CreateInstance("https://nominatim.openstreetmap.org");
            apiService.PutOrEditData("Україна", "Київ", "Солом'янська", "Зе бест місце");
            List<string> subdirectories = new List<string>
            {
                "search"
            };
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                {"format", "json"},
                {"addressdetails", "0"}
            };
            apiService.SendRequest(subdirectories, keyValuePairs);
        }
    }
}
