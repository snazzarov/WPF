using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Windows;

namespace HomeFin
{
    public class Db
    {
        static string connectionString;
        private SqlDataAdapter adapter;
        static SqlConnection connection;
        private string SQLSelect;
        const string PROC_INSERT = "InsAccount";
        const string PROC_UPDATE = "UpdAccount";
        private bool wide;
        private string prefix;
        private int lengthPrefix;
        private char special;

        public static SqlConnection DbConnection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            return new SqlConnection(connectionString);
        }
        public static string GetPrefix()
        {
            return Properties.Settings.Default.PREFIX_SPECIAL.ToString();
        }
        public Db(bool wide)
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            connection = new SqlConnection(connectionString);
            prefix = Properties.Settings.Default.PREFIX_SPECIAL.ToString();
            lengthPrefix = prefix.Length;
            this.wide = wide;
            this.special = '0';
            SetSQLSelect();
            //MessageBox.Show(SQLSelect);
        }
        public Db(char special)
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            connection = new SqlConnection(connectionString);
            prefix = Properties.Settings.Default.PREFIX_SPECIAL.ToString();
            lengthPrefix = prefix.Length;
            this.special = special;
            SetSQLSelect();
        }
        private void SetSQLSelect()
        {
            if (!wide)
                SQLSelect = "SELECT * FROM Account " +
                            "Where Upper(Title) not like Upper('" + prefix + "') + '%'" +
                            "  and Deleted is null";
            else
                SQLSelect = "SELECT * FROM Account " +
                                "Where Upper(Title) like Upper('" + prefix + "') + '%'" +
                                "  and Deleted is null";
        }
        public void SetAdapterInsert()
        {            
            SqlParameter parameter;
            decimal balance = 0.0m;
            SqlCommand command = new SqlCommand(SQLSelect, connection);
            adapter = new SqlDataAdapter(command);
            adapter.InsertCommand = new SqlCommand(PROC_INSERT, connection);
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 30, "Title"));
            if (!wide)
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Balance", SqlDbType.Decimal, 0, balance.ToString()));
            else
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Balance", SqlDbType.Decimal, 0, "Balance"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@CurrencyId", SqlDbType.Int, 0, "CurrencyId"));
            adapter.InsertCommand.Parameters.Add(new SqlParameter("@Prefix", SqlDbType.NVarChar, lengthPrefix, "prefix"));
            parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            parameter.Direction = ParameterDirection.Output;
        }
        public void SetAdapterSelectUpdate()
        {
            connection = new SqlConnection(connectionString);
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
        public bool UpdateDB(DataTable accountsTable)
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
        private void ExecSP(string titleAccount)
        {   
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AddAccount";
                command.Parameters.AddWithValue("@Title", titleAccount);
                command.Parameters.AddWithValue("@Balance", 0);
                command.Parameters.AddWithValue("@CurrencyId", 0);

                command.ExecuteNonQuery();
            }
        }
        public List<Currency> GetCurrencies()
        {
            try
            {
                List<Currency> CurList = new List<Currency>();
                SqlCommand com = new SqlCommand("dbo.GetCurrencies", connection);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                using (com.Connection = connection)
                {
                    com.Connection.Open();
                    da.Fill(dt);
                }
                //Bind Currency Model generic list using LINQ 
                CurList = (from DataRow dr in dt.Rows
                           select new Currency()
                           {
                               CurrencyId = Convert.ToInt32(dr["Id"]),
                               Name = Convert.ToString(dr["Name"])
                           }).ToList();
                return CurList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            return null;
        }
    }
}

