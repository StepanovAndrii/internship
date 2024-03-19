﻿using Practice.API;
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
            HttpApiService apiService = HttpApiService.CreateInstance("https://nominatim.openstreetmap.org");
            apiService.SendRequest("Україна", "Київ", "Солом'янська", "Круте місце");
        }
    }
}
