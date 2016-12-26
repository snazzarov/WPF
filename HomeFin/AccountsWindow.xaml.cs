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
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace HomeFin
{
    /// <summary>
    /// Логика взаимодействия для Accounts.xaml
    /// </summary>
    public partial class AccountsWindow : Window
    {
        private DataTable accountsTable;
        private char special;
        private string SQLSelect;
        private SqlConnection connection;
        private SqlDataAdapter adapter;
        private string prefix;
        private int lengthPrefix;
        const string PROC_UPDATE = "UpdAccount";
        const char USER_ACCOUNT = '1';
        const char SPECIAL_ACCOUNT = '2';

        public AccountsWindow(char special)
        {
            InitializeComponent();
            this.special = special;
            this.DataContext = new CurrencyViewModel();
            accountsTable = null;
            prefix = Db.GetPrefix();
            lengthPrefix = prefix.Length;
            SetSQLSelect();
        }
        private void SetSQLSelect()
        {

            if (special.Equals(USER_ACCOUNT))
                SQLSelect = "SELECT * FROM Account " +
                            "Where Upper(Title) not like Upper('" + prefix + "') + '%'" +
                            "  and Deleted is null";
            else            // '2'
                SQLSelect = "SELECT * FROM Account " +
                                "Where Upper(Title) like Upper('" + prefix + "') + '%'" +
                                "  and Deleted is null";
        }
        public void SetAdapterSelectUpdate()
        {
            connection = Db.DbConnection();
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.UpdateCommand = new SqlCommand(PROC_UPDATE, connection);
            adapter.UpdateCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter parameter = adapter.UpdateCommand.Parameters.Add("@Id",  SqlDbType.Int, 0, "Id");
            parameter.Direction = ParameterDirection.InputOutput;
        }
        public DataTable FillTable()
        {
            DataTable tempTable = new DataTable();
            try
            {
                connection = Db.DbConnection();
                connection.Open();
                adapter.Fill(tempTable);
                return tempTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
            return null;
        }
        private bool UpdateDB() 
        {
            try
            {
                adapter.Update(accountsTable);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }
        private void AccountsGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
        public void WindowRefresh()
        {
            SetAdapterSelectUpdate();
            accountsTable = new DataTable();
            accountsTable = FillTable();
            accGrid.ItemsSource = accountsTable.DefaultView;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowRefresh();
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            if (special.Equals(USER_ACCOUNT)) {
                NarrowNewWindow nw = new NarrowNewWindow(this);
                nw.ShowDialog();
                return;
            }
            WideNewWindow ww = new WideNewWindow(this);
            ww.ShowDialog();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (accGrid.SelectedItems != null)
            {
                DataRowView datarowView = accGrid.SelectedItems[0] as DataRowView;
                if (datarowView != null)
                {
                    DataRow dataRow = (DataRow)datarowView.Row;
                    dataRow["Deleted"] = "1";
                    if (((decimal)dataRow["Balance"] > 0.0m) && 
                         (special == USER_ACCOUNT))
                        MessageBox.Show("Balance of the account will be moved to Income account " + special.ToString());
                }
            }
            UpdateDB();
            WindowRefresh();
        }
    }
}