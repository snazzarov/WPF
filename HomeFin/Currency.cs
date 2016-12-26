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
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; }

    }
    public class CurrencyCollection : ObservableCollection<Currency>
    {
    }

    public class CurrencyViewModel : NotifyUIBase
    {
        private const string PROC_GET_CURRENCIES = "GetCurrencies";
        public CurrencyViewModel()
        {
            CurrencyCollection cc = Application.Current.Resources["CurrencySource"] as CurrencyCollection;
            cc.Clear();
            List<Currency> lc = GetCurrencies();
            foreach (Currency c in lc)
                cc.Add(c);
            Application.Current.Resources["CurrencySource"] = cc;
        }
        private List<Currency> GetCurrencies()
        {
            try
            {
                SqlConnection connection = Db.DbConnection();
                List<Currency> CurList = new List<Currency>();
                SqlCommand com = new SqlCommand(PROC_GET_CURRENCIES, connection);
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