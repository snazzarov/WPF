using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Windows;
using System.Data.SqlClient;

namespace HomeFin
{
    public class Operation
    {
        public int DebetAccId { get; set; }
        public int CreditAccId { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
    public class DebetAccount
    {
        public int DebetAccId { get; set; }
        public string Title { get; set; }
        //        public decimal Balance { get; set; }
//        public int CurrencyId { get; set; }
    }
    public class CreditAccount
    {
        public int CreditAccId { get; set; }
        public string Title { get; set; }
        //        public decimal Balance { get; set; }
//        public int CurrencyId { get; set; }
    }
    public class DebetAccountCollection : ObservableCollection<DebetAccount>
    {
    }
    public class CreditAccountCollection : ObservableCollection<CreditAccount>
    {
    }
    public class AccountViewModel : NotifyUIBase
    {
        private const string PROC_GET_USER_ACCOUNTS= "GetUserAccounts";
        private const string PROC_GET_SPECIAL_ACCOUNTS = "GetSpecialAccounts";
        public AccountViewModel()
        {
            DebetAccountCollection acDebet = Application.Current.Resources ["DebetAccountSource"] as DebetAccountCollection;
            acDebet.Clear();
            List<DebetAccount> lda = GetDebetAccounts();
            foreach (DebetAccount a in lda)
                acDebet.Add(a);
            Application.Current.Resources["DebetAccountSource"] = acDebet;
            //
            CreditAccountCollection acCredit = Application.Current.Resources["CreditAccountSource"] as CreditAccountCollection;
            acCredit.Clear();
            List<CreditAccount> lca = GetCreditAccounts();
            foreach (CreditAccount op in lca)
                acCredit.Add(op);
            Application.Current.Resources["CreditAccountSource"] = acCredit;
        }
        private List<CreditAccount> GetCreditAccounts()
        {
            try
            {
                SqlConnection connection = Db.DbConnection();
                List<CreditAccount> AccList = new List<CreditAccount>();
                SqlCommand com = new SqlCommand(PROC_GET_USER_ACCOUNTS, connection);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                using (com.Connection = connection)
                {
                    com.Connection.Open();
                    da.Fill(dt);
                }
                //Bind Account Model generic list using LINQ 
                AccList = (from DataRow dr in dt.Rows
                           select new CreditAccount()
                           {
                               CreditAccId = Convert.ToInt32(dr["Id"]),
                               Title = Convert.ToString(dr["Title"])
                           }).ToList();
                return AccList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private List<DebetAccount> GetDebetAccounts()
        {
            try
            {
                SqlConnection connection = Db.DbConnection();
                List<DebetAccount> AccList = new List<DebetAccount>();
                SqlCommand com = new SqlCommand(PROC_GET_SPECIAL_ACCOUNTS, connection);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                using (com.Connection = connection)
                {
                    com.Connection.Open();
                    da.Fill(dt);
                }
                //Bind Account Model generic list using LINQ 
                AccList = (from DataRow dr in dt.Rows
                           select new DebetAccount()
                           {
                               DebetAccId = Convert.ToInt32(dr["Id"]),
                               Title = Convert.ToString(dr["Title"])
                           }).ToList();
                return AccList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}

