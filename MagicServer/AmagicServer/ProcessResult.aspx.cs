using AmagicServer.BLL;
using AmagicServer.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace AmagicServer
{
    public partial class ProcessResult : System.Web.UI.Page
    {
        private DeviceInfoBLL deviceInfoBLL = new DeviceInfoBLL();
        private string GetFieldValue(string strResponse, string fieldName)
        {
            int indexAmout = strResponse.IndexOf(fieldName + "=");
            StringBuilder sbValue = new StringBuilder();
            if (indexAmout >= 0)
            {
                indexAmout = indexAmout+fieldName.Length + 1;
                while (!string.IsNullOrWhiteSpace(strResponse.Substring(indexAmout, 1)))
                {
                    sbValue.Append(strResponse.Substring(indexAmout, 1));
                    indexAmout++;
                }

            }

            return sbValue.ToString();

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            string strFormValues;
            string strResponse;
            string authToken;
            string txToken;
            string query;
     
            // authToken = "DGFlMUaVIzRUcxGLezfWIzKpn-JRojNRaSogvpeEzYQ_n5UyLwVy1PGgemm";
            authToken = "XKuewxkCAnj6V2ohVcwh7hjr3e5Y7RG-sLgJ50GWTLDwbGN75dTCAlHkAYO";

           // txToken = "8UA550014D777742D";//;Request.QueryString["tx"];
            txToken = Request.QueryString["tx"];

            string sn = Request.QueryString["cm"];


            //   HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.paypal.com/cgi-bin/webscr");
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
      
            query = "cmd=_notify-synch&tx=" + txToken + "&at=" + authToken;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(query);
            strFormValues = Encoding.ASCII.GetString(byte1);
            myHttpWebRequest.ContentLength = strFormValues.Length;
 
            StreamWriter stOut = new StreamWriter(myHttpWebRequest.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(strFormValues);
            stOut.Close();
   
            StreamReader stIn = new StreamReader(myHttpWebRequest.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
  
            string isSuccess = strResponse.Substring(0, 7);
           // Response.Write(isSuccess);
      
          //  Response.Write(strResponse);

            if (isSuccess == "SUCCESS")
            {
                string receivedEmail = GetFieldValue(strResponse, "receiver_email").Trim();
                string totaoAmount=GetFieldValue(strResponse, "mc_gross").Trim() ;

                string plainSN = sn;// EncryUtils.DESDeCode(sn);

                if (!string.IsNullOrEmpty(plainSN) && totaoAmount == "0.10" && receivedEmail == "gm1tran2l%40gmail.com")
                {

                    if (!deviceInfoBLL.CheckDeviceByPhoneSN(plainSN))
                    {
                        if (!deviceInfoBLL.CheckDeviceByPaymentId(txToken))
                        {
                            DeviceInfo di = new DeviceInfo();
                            di.ActiveFlag = "1";
                            di.DeviceId = plainSN;
                            di.PaypalStatus = "P";
                            di.PhoneSN = plainSN;
                            di.PaypalInfo = "";
                            di.PaymentId = txToken;
                            di.State = "";
                            di.PayPayDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd hh:mm:ss");
                            di.ProductType = "Sandbox";
                            di.RecordDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd hh:mm:ss");
                            if (deviceInfoBLL.AddDevice(di) > 0)
                            {
                                Response.Write("\n Paid Succefully, Please Validate The Result In Andy Magic App.");
                                Response.Write(System.Environment.NewLine + "<br/> 支付成功，请在安迪魔术应用中验证并使用完整版。");
                            }
                        }
                        else
                        {
                            Response.Write("\n Paid Fail.");
                            Response.Write(System.Environment.NewLine + "<br/> 支付失败。 <br/>"); 
                        }
                    }
                    else
                    {
                        Response.Write("\n Paid Succefully, Please Validate The Result In Andy Magic App.");
                        Response.Write(System.Environment.NewLine + "<br/> 支付成功，请在安迪魔术应用中验证并使用完整版。");
                    }                    
                }        
            }
            else
            {
                Response.Write("\n Paid Fail.");
                Response.Write(System.Environment.NewLine + "<br/> 支付失败。 <br/>"); 
            }
        }
    }
}