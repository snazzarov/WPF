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
    /// Логика взаимодействия для currencies.xaml
    /// </summary>
    public partial class CurrenciesWindow : Window
    {
        private DataTable currenciesTable;
        private string SQLSelect;
        private SqlConnection connection;
        private SqlDataAdapter adapter;
        const string PROC_UPDATE = "UpdCurrency";

        public CurrenciesWindow()
        {
            InitializeComponent();
            currenciesTable = null;
            SetSQLSelect();
        }
        private void SetSQLSelect()
        {
            SQLSelect = "SELECT * FROM Currency Where Deleted is null";
        }
        public void SetAdapterSelectUpdate()
        {
            connection = Db.DbConnection();
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.UpdateCommand = new SqlCommand(PROC_UPDATE, connection);
            adapter.UpdateCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter parameter = adapter.UpdateCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
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
                adapter.Update(currenciesTable);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }
        private void currenciesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
        public void WindowRefresh()
        {
            SetAdapterSelectUpdate();
            currenciesTable = new DataTable();
            currenciesTable = FillTable();
            currencyGrid.ItemsSource = currenciesTable.DefaultView;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowRefresh();
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
                NewCurrencyWindow nw = new NewCurrencyWindow(this);
                nw.ShowDialog();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (currencyGrid.SelectedItems != null)
            {
                DataRowView datarowView = currencyGrid.SelectedItems[0] as DataRowView;
                if (datarowView != null)
                {
                    DataRow dataRow = (DataRow)datarowView.Row;
                    dataRow["Deleted"] = "1";
                }
            }
            UpdateDB();
            WindowRefresh();
        }
    }
}