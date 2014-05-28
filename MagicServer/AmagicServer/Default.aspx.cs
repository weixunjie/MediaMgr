using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AmagicServer
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || string.IsNullOrEmpty(Session["UserId"].ToString()))
            {
                Response.Redirect("Account/Login.aspx");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //    SqlConnectionStringBuilder connString1Builder;
            //    connString1Builder = new SqlConnectionStringBuilder();
            //    connString1Builder.DataSource = "e8z7xervet.database.windows.net,1433";
            //    connString1Builder.InitialCatalog = "2lltdamagic";
            //    connString1Builder.Encrypt = true;
            //    connString1Builder.TrustServerCertificate = false;
            //    connString1Builder.UserID = "amagic";
            //    connString1Builder.Password = "Cigama123";


            //    // Connect to the master database and create the sample database 
            //    using (SqlConnection conn = new SqlConnection(connString1Builder.ToString()))
            //    {


            //        using (SqlCommand command = conn.CreateCommand())
            //        {

            //            conn.Open();

            //            string cmdText = "drop table  IF EXISTS DeviceInfo";
            //            ////command.CommandText = cmdText;
            //            ////command.ExecuteNonQuery();

            //            ////cmdText = "drop table  IF EXISTS UserInfo";
            //            ////command.CommandText = cmdText;
            //            ////command.ExecuteNonQuery();

            //            //// Create the sample database 
            //            ////cmdText = "CREATE table DeviceInfo(deviceid int identity(1,1),PhoneSN varchar(100),PaypalStatus varchar(20),RecordDate datetime, ActiveFlag int)";
            //            ////command.CommandText = cmdText;
            //            ////command.ExecuteNonQuery();


            //            ////cmdText = "CREATE table UserInfo(UserId int identity(1,1),UserName varchar(100),Password varchar(20))";
            //            ////command.CommandText = cmdText;
            //            ////command.ExecuteNonQuery();

            //            ////   cmdText = "alter table DeviceInfo add primary key (deviceid)";
            //            ////command.CommandText = cmdText;
            //            ////command.ExecuteNonQuery();

            //            //cmdText = "  create  index DeviceInfoKeyIndex on DeviceInfo(deviceid) ";
            //            //command.CommandText = cmdText;
            //            //command.ExecuteNonQuery();


            //            //cmdText = "alter table UserInfo add primary key (UserId)";
            //            //command.CommandText = cmdText;
            //            //command.ExecuteNonQuery();

            //            //cmdText = " create   index UserInfoKeyIndex on UserInfo(UserId) ";
            //            //command.CommandText = cmdText;
            //            //command.ExecuteNonQuery();





            //            //cmdText = "insert into UserInfo(UserName,Password) values ('Admin','amagicadmin')";
            //            //command.CommandText = cmdText;
            //            //command.ExecuteNonQuery();


            //            cmdText = "insert into DeviceInfo(PhoneSN,PaypalStatus,RecordDate,ActiveFlag) values ('12547444','1','" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',1)";
            //            command.CommandText = cmdText;
            //            command.ExecuteNonQuery();


            //            command.CommandText = "SELECT * FROM DeviceInfo";

            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                // Loop over the results 
            //                while (reader.Read())
            //                {
            //                    Response.Write(reader["PhoneSN"].ToString().Trim() + "-" + reader["RecordDate"].ToString().Trim());

            //                }
            //            }

            //            conn.Close();




            //        }
            //    }
        }

    }
}