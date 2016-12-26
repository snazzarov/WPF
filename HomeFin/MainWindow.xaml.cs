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

namespace HomeFin
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
        private void SpecialAccounts_Click(object sender, RoutedEventArgs e)
        {
            var accountsWindow = new AccountsWindow('2');
            accountsWindow.ShowDialog();
        }
        private void Accounts_Click(object sender, RoutedEventArgs e)
        {
            var accountsWindow = new AccountsWindow('1');
            accountsWindow.ShowDialog();
        }

        private void Currencies_Click(object sender, RoutedEventArgs e)
        {
            var currenciesWindow = new CurrenciesWindow();
            currenciesWindow.ShowDialog();
        }

        private void Operations_Click(object sender, RoutedEventArgs e)
        {
            var operationsWindow = new OperationsWindow();
            operationsWindow.ShowDialog();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
