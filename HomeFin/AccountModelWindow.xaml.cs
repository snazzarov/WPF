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
using System.Windows.Shapes;

namespace HomeFin
{
    /// <summary>
    /// Логика взаимодействия для AccountModelWindow.xaml
    /// </summary>
    public partial class AccountModelWindow : Window
    {
        public AccountModelWindow()
        {
            InitializeComponent();
            this.DataContext = new AccountModel();
        }

        private void addModelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteModelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
