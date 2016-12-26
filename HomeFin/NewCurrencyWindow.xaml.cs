using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeFin
{
    /// <summary>
    /// Логика взаимодействия для NewAccount.xaml
    /// </summary>
    public partial class NewCurrencyWindow : Window
    {
        private DataTable newCurrencies;
        private CurrenciesWindow parentWindow;
        private SqlDataAdapter adapter;
        private SqlConnection connection;
        private string SQLSelect;
        const string PROC_INSERT = "InsCurrency";

        public NewCurrencyWindow(CurrenciesWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
            connection = Db.DbConnection();
            SetAdapterInsert();
            newCurrGrid.RowEditEnding += narrowAccGrid_RowEditEnding;
            SQLSelect = "SELECT * FROM Currency Where Deleted is null";
        }
        private void SetAdapterInsert()
        {
            SqlParameter parameter;
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.InsertCommand = new SqlCommand(PROC_INSERT, connection);
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 30, "Name"));
            parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            parameter.Direction = ParameterDirection.Output;
        }
        private void updButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateDB())
                // error occurs while writing to database
                return;
            parentWindow.WindowRefresh();
            Close();
        }
        public bool UpdateDB()
        {
            try
            {
                adapter.Update(newCurrencies);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newCurrencies = new DataTable();
            newCurrencies.Columns.Add("Name", typeof(string));
            newCurrGrid.ItemsSource = newCurrencies.DefaultView;
        }
        private void narrowAccGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
    }
}
