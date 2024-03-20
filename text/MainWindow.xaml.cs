using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace text
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "Enter text here...")
            {
                tb.Text = "";
            }
            tb.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Enter text here...";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
