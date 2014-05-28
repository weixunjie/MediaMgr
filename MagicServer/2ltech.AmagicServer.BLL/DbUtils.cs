using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AmagicServer.BLL
{
    public class DbUitls
    {
        private SqlConnectionStringBuilder connStringBuilder;
        private SqlConnection conn = null;
        public DbUitls()
        {
            connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = "lne4i5ecro.database.windows.net,1433";
            connStringBuilder.InitialCatalog = "2lltdamagic1";
            connStringBuilder.Encrypt = true;
            connStringBuilder.TrustServerCertificate = false;
            connStringBuilder.UserID = "amagic";
            connStringBuilder.Password = "Cigama123";
            conn = new SqlConnection(connStringBuilder.ToString());



        }

        ~DbUitls()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public int ExeSql(string sqlStr)
        {

            // Connect to the master database and create the sample database 


            using (SqlCommand command = conn.CreateCommand())
            {

                conn.Open();
                command.CommandText = sqlStr;
                int result = command.ExecuteNonQuery();

                conn.Close();
                return result;
            }

        }



        public void ClosoConnection()
        {
            conn.Close();
        }



        public SqlDataReader GetSqlRedaer(string sqlStr)
        {            using (SqlCommand command = conn.CreateCommand())
            {


                conn.Open();
                command.CommandText = sqlStr;

                SqlDataReader reader = command.ExecuteReader();


                return reader;

            }

        }




    }

}
