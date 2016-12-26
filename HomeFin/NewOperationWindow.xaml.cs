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
    public partial class NewOperationWindow : Window
    {
        private DataTable newOperations;
        private OperationsWindow parentWindow;
        private SqlDataAdapter adapter;
        private SqlConnection connection;
        private string SQLSelect;
        const string PROC_INSERT = "InsOperation";

        public NewOperationWindow(OperationsWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
            connection = Db.DbConnection();
            SetAdapterInsert();
            newOperGrid.RowEditEnding += newOperGrid_RowEditEnding;
            SQLSelect = "SELECT distinct * FROM Operation";
        }
        private void SetAdapterInsert()
        {
            SqlParameter parameter;
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.InsertCommand = new SqlCommand(PROC_INSERT, connection);
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@DebetAccId", SqlDbType.Int, 0, "DebetAccId"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@CreditAccId", SqlDbType.Int, 0, "CreditAccId"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal, 0, "Amount"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar, 50, "Comment"));
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
                adapter.Update(newOperations);
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
            newOperations = new DataTable();
            newOperations.Columns.Add("DebetAccId", typeof(int));
            newOperations.Columns.Add("CreditAccId", typeof(int));
            newOperations.Columns.Add("Amount", typeof(decimal));
            newOperations.Columns.Add("Comment", typeof(string));
            newOperGrid.ItemsSource = newOperations.DefaultView;
        }
        private void newOperGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UpdateDB();
        }
    }
}
