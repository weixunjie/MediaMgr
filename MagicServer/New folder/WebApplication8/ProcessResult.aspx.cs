using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication8
{
    public partial class ProcessResult : System.Web.UI.Page
    {

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
            //定义您的身份标记,这里改成您的身份标记
            // authToken = "DGFlMUaVIzRUcxGLezfWIzKpn-JRojNRaSogvpeEzYQ_n5UyLwVy1PGgemm";
            authToken = "XKuewxkCAnj6V2ohVcwh7hjr3e5Y7RG-sLgJ50GWTLDwbGN75dTCAlHkAYO";
            //获取PayPal 交易流水号
            txToken = Request.QueryString["tx"];
            string sn = Request.QueryString["cm"];
            string jj = Request.QueryString["wei"];
            // Set the 'Method' property of the 'Webrequest' to 'POST'.
            //   HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.paypal.com/cgi-bin/webscr");
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            //设置请求参数
            query = "cmd=_notify-synch&tx=" + txToken + "&at=" + authToken;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(query);
            strFormValues = Encoding.ASCII.GetString(byte1);
            myHttpWebRequest.ContentLength = strFormValues.Length;
            //发送请求
            StreamWriter stOut = new StreamWriter(myHttpWebRequest.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(strFormValues);
            stOut.Close();
            //接受返回信息
            StreamReader stIn = new StreamReader(myHttpWebRequest.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            //取前面七个字符
            string isSuccess = strResponse.Substring(0, 7);
            Response.Write(isSuccess);
            //显示返回的字符串，
            Response.Write(strResponse);

            if (isSuccess == "SUCCESS")
            {
                Response.Write("RESPONSE SUCESS\n ");//此处需要判断网站订单是否已经处理

                int indexAmout = strResponse.IndexOf("mc_gross=");

                StringBuilder sbAmount = new StringBuilder();
                if (indexAmout >= 0)
                {
                    indexAmout = "mc_gross=".Length;
                    while (!string.IsNullOrWhiteSpace(strResponse.Substring(indexAmout, 1)))
                    {
                        sbAmount.Append(strResponse.Substring(indexAmout, 1));

                        indexAmout++;
                    }
                }

                string receivedEmail = GetFieldValue(strResponse, "receiver_email").Trim();
                string totaoAmount=GetFieldValue(strResponse, "mc_gross").Trim() ;


                if (totaoAmount == "0.10" && receivedEmail == "gm1tran2l%40gmail.com")
                {
                    Response.Write("Good Check SUCESS\n ");//此处需要判断网站订单是否已经处理
                }

                //Response.Write("item_number：" + Request.QueryString["receiver_email"].ToString() + "\n");
            }
            else
            {
                Response.Write("\n response fail");
            }


            //string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            //string strLive = "https://www.paypal.com/cgi-bin/webscr";
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);
            ////Set values for the request back
            //req.Method = "POST";

            // string authToken = "6jwYL2NY-y0npRcq6Vz4gbmWmFO7VXHRZtaTyCGYnIcoYa121YYXuDx0izi";  

            //req.ContentType = "application/x-www-form-urlencoded";
            //byte[] param = Request.BinaryRead(Request.ContentLength);
            //string strRequest = Encoding.ASCII.GetString(param);
            //string strResponse_copy = strRequest;  //Save a copy of the initial info sent by PayPal

            //    =2SC1208200241730P
            //strRequest += "&cmd=_notify-validate&at=" +authToken+";
            //req.ContentLength = strRequest.Length;

            ////for proxy
            ////WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            ////req.Proxy = proxy;
            ////Send the request to PayPal and get the response
            //StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            //streamOut.Write(strRequest);
            //streamOut.Close();
            //StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            //string strResponse = streamIn.ReadToEnd();
            //streamIn.Close();

            //if (strResponse == "VERIFIED")
            //{
            //    //check the payment_status is Completed
            //    //check that txn_id has not been previously processed
            //    //check that receiver_email is your Primary PayPal email
            //    //check that payment_amount/payment_currency are correct
            //    //process payment

            //    // pull the values passed on the initial message from PayPal

            //    NameValueCollection these_argies = HttpUtility.ParseQueryString(strResponse_copy);
            //    string user_email = these_argies["payer_email"];
            //    string pay_stat = these_argies["payment_status"];
            //    //.
            //    //.  more args as needed look at the list from paypal IPN doc
            //    //.


            //    if (pay_stat.Equals("Completed"))
            //    {
            //       // Send_download_link("yours_truly@mycompany.com", user_email, "Your order", "Thanks for your order this the downnload link ... blah blah blah");
            //    }


            //    // more checks needed here specially your account number and related stuff
            //}
            //else if (strResponse == "INVALID")
            //{
            //    //log for manual investigation
            //}
            //else
            //{
            //    //log response/ipn data for manual investigation
            //}
            //    }

            //Post back to either sandbox or live
            //string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            //string strLive = "https://www.paypal.com/cgi-bin/webscr";
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);

            //// Set values for the request back
            //req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            //byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            //string strRequest = Encoding.UTF8.GetString(param);
            //strRequest += "&cmd=_notify-validate";
            //req.ContentLength = strRequest.Length;

            ////for proxy
            ////WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            ////req.Proxy = proxy;

            ////Send the request to PayPal and get the response
            //StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.UTF8);
            //streamOut.Write(strRequest);
            //streamOut.Close();
            //StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            //string strResponse = streamIn.ReadToEnd();
            //streamIn.Close();

            //if (strResponse == "VERIFIED")
            //{

            //    // check the payment_status is Completed
            //    // check that txn_id has not been previously processed
            //    // check that receiver_email is your Primary PayPal email
            //    // check that payment_amount/payment_currency are correct
            //    // process payment
            //}
            //else if (strResponse == "INVALID")
            //{
            //    //do somethings
            //}
            //else
            //{
            //    //log response/ipn data for manual investigation
            //}

        }
    }
}