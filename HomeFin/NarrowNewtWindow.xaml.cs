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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeFin
{
    /// <summary>
    /// Логика взаимодействия для NewAccount.xaml
    /// </summary>
    public partial class NarrowNewWindow : Window
    {
        private DataTable newAccounts;
        private AccountsWindow parentWindow;
        private SqlDataAdapter adapter;
        private SqlConnection connection;
        private string prefix;
        private string SQLSelect;
        const string PROC_2INSERT = "Ins2Account";

        public NarrowNewWindow(AccountsWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
            connection = Db.DbConnection();
            prefix = Db.GetPrefix();
            SetAdapterInsert();
            narrowAccGrid.RowEditEnding += narrowAccGrid_RowEditEnding;
            prefix = Db.GetPrefix();
            SQLSelect = "SELECT * FROM Account " +
                        "Where Upper(Title) not like Upper('" + prefix + "') +  '%'"   +
                       "  and Deleted is null";
        }
        private void SetAdapterInsert()
        {
            SqlParameter parameter;
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.InsertCommand = new SqlCommand(PROC_2INSERT, connection);
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 30, "Title"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@CurrencyId", SqlDbType.Int, 0, "CurrencyId"));
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
                adapter.Update(newAccounts);
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
            newAccounts = new DataTable();
            newAccounts.Columns.Add("Title", typeof(string));
            newAccounts.Columns.Add("CurrencyId", typeof(int));
            narrowAccGrid.ItemsSource = newAccounts.DefaultView;
        }
        private void narrowAccGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
    }
}
