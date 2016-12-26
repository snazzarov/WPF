using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace HomeFin
{
/*
    public class MainViewModel : NotifyUIBase
    {
        //       public ObservableCollection<Media> Medias { get; set; }
        public MainViewModel()
        {
            CurrencyCollection cc = Application.Current.Resources["CurrencySource"] as CurrencyCollection;
            List<Currency> lc = GetCurrencies();
            foreach (Currency c in lc)
                cc.Add(c);
            Application.Current.Resources["CurrencySource"] = cc;
        }
        public List<Currency> GetCurrencies()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
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
                if (connection != null)
                    connection.Close();
            }
            return null;
        }
    }
*/
}