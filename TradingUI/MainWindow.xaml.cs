using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TradingUI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel MainViewModel;
        public MainWindow()
        {
            InitializeComponent();
        }        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainViewModel.Close();
        }
    }
}
