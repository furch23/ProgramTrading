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

namespace TradeingUI2
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string symbol = "";
            string side = "";
            double price = 0;
            int qty = 0;
            string ordertype = "";
            string timeinforce = "";

            if (double.TryParse(tb_Price.Text, out double p))
            {
                price = p;
            }
            else
            {
                MessageBox.Show("輸入價格錯誤", "告警式窗!!", System.Windows.MessageBoxButton.OK);
            }

            side = cb_Side.SelectionBoxItem as string;
            symbol = tb_Symbol.Text.Trim();
            qty = Convert.ToInt32(tb_Qty.Text);
            ordertype = cb_OrderType.SelectionBoxItem as string;
            timeinforce = cb_TimeInForce.SelectionBoxItem as string;

            string result = $"{side} {qty} {symbol} @ {price} {ordertype} {timeinforce}";

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(result, "是否送單?", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainViewModel.SendOrder(symbol, side, qty, price, ordertype, timeinforce);
            }
        }
    }
}
