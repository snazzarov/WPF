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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeFin
{
    /// <summary>
    /// Логика взаимодействия для OperationsWindow.xaml
    /// </summary>
    public partial class OperationsWindow : Window
    {
        private DataTable accountsTable;
        private string SQLSelect;
        private SqlConnection connection;
        private SqlDataAdapter adapter;
        const string PROC_UPDATE = "UpdOperations";

        public OperationsWindow()
        {
            InitializeComponent();
            this.DataContext = new AccountViewModel();
            accountsTable = null;
            SetSQLSelect();
            operGrid.RowEditEnding += OperGrid_RowEditEnding;            
        }
        private void SetSQLSelect()
        {

            SQLSelect = "SELECT distinct * FROM Operation";
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
                adapter.Update(accountsTable);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }
        private void OperGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
        public void WindowRefresh()
        {
            SetAdapterSelectUpdate();
            accountsTable = new DataTable();
            accountsTable = FillTable();
            operGrid.ItemsSource = accountsTable.DefaultView;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowRefresh();
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            NewOperationWindow ww = new NewOperationWindow(this);
            ww.ShowDialog();
        }

    }
}
