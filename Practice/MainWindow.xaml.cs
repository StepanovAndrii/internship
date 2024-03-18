using Practice.API;
using Practice.Models;
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
            ApiService apiService = new ApiService("https://nominatim.openstreetmap.org");
        }
    }
}
