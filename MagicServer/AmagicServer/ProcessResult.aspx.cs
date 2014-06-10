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
                indexAmout = indexAmout + fieldName.Length + 1;
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

            NVPAPICaller test = new NVPAPICaller();

            string retMsg = "";
            string token = "";
            string finalPaymentAmount = "";
            string payerId = "";
            NVPCodec decoder = null;

            NVPCodec decoderDetail = null;

            token = Request["token"].ToString();
            payerId = Request["PayerID"].ToString();
            finalPaymentAmount = "0.1";



            bool ret = test.ConfirmPayment(finalPaymentAmount, token, payerId, ref decoder, ref retMsg);
            if (ret)
            {
                string tid = decoder["TRANSACTIONID"];
                bool retDetail = test.GetShippingDetails(token, ref decoderDetail);

                if (retDetail)
                {

                    string plainSN = decoderDetail["CUSTOM"];

                    //Response.Write(aa);

                    if (!deviceInfoBLL.CheckDeviceByPhoneSN(plainSN))
                    {
                        if (!deviceInfoBLL.CheckDeviceByPaymentId(tid))
                        {
                            DeviceInfo di = new DeviceInfo();
                            di.ActiveFlag = "1";
                            di.DeviceId = plainSN;
                            di.PaypalStatus = "P";
                            di.PhoneSN = plainSN;
                            di.PaypalInfo = "";
                            di.PaymentId = tid;
                            di.State = "";
                            di.PayPayDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                            di.ProductType = "live";
                            di.RecordDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
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

                    //// Indicates whether the payment is instant or delayed. Possible values: l  none l  echeck l  instant 
                    //string paymentType = decoder["PAYMENTTYPE"];

                    //// Time/date stamp of payment
                    //string orderTime = decoder["ORDERTIME"];

                    //// The final amount charged, including any shipping and taxes from your Merchant Profile.
                    //string amt = decoder["AMT"];

                    //// A three-character currency code for one of the currencies listed in PayPay-Supported Transactional Currencies. Default: USD.    
                    //string currencyCode = decoder["CURRENCYCODE"];

                    //// PayPal fee amount charged for the transaction    
                    //string feeAmt = decoder["FEEAMT"];

                    //// Amount deposited in your PayPal account after a currency conversion.    
                    //string settleAmt = decoder["SETTLEAMT"];

                    //// Tax charged on the transaction.    
                    //string taxAmt = decoder["TAXAMT"];

                    ////' Exchange rate if a currency conversion occurred. Relevant only if your are billing in their non-primary currency. If 
                    //string exchangeRate = decoder["EXCHANGERATE"];
                }
            }
            else
            {
                Response.Redirect("APIError.aspx?" + retMsg);
            }


            //    string strFormValues;
            //    string strResponse;
            //    string authToken;
            //    string txToken;
            //    string query;

            //    authToken = "DGFlMUaVIzRUcxGLezfWIzKpn-JRojNRaSogvpeEzYQ_n5UyLwVy1PGgemm";
            //   // authToken = "XKuewxkCAnj6V2ohVcwh7hjr3e5Y7RG-sLgJ50GWTLDwbGN75dTCAlHkAYO";

            //  //  txToken = "2PN098802P8374540";
            //    txToken =Request.QueryString["tx"];
            //    //txToken = Request.QueryString["tx"];

            //    string sn = Request.QueryString["cm"];


            //    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.paypal.com/cgi-bin/webscr");
            //   // HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");
            //    myHttpWebRequest.Method = "POST";
            //    myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

            //    query = "cmd=_notify-synch&tx=" + txToken + "&at=" + authToken;

            //    ASCIIEncoding encoding = new ASCIIEncoding();
            //    byte[] byte1 = encoding.GetBytes(query);
            //    strFormValues = Encoding.ASCII.GetString(byte1);
            //    myHttpWebRequest.ContentLength = strFormValues.Length;

            //    StreamWriter stOut = new StreamWriter(myHttpWebRequest.GetRequestStream(), System.Text.Encoding.ASCII);
            //    stOut.Write(strFormValues);
            //    stOut.Close();

            //    StreamReader stIn = new StreamReader(myHttpWebRequest.GetResponse().GetResponseStream());
            //    strResponse = stIn.ReadToEnd();
            //    stIn.Close();

            //    string isSuccess = strResponse.Substring(0, 7);
            //   // Response.Write(isSuccess);

            //  //  Response.Write(strResponse);

            //    if (isSuccess == "SUCCESS")
            //    {
            //        string receivedEmail = GetFieldValue(strResponse, "receiver_email").Trim();
            //        string totaoAmount=GetFieldValue(strResponse, "mc_gross").Trim() ;

            //        string plainSN = sn;

            //        if (!string.IsNullOrEmpty(plainSN) && totaoAmount == "15.00" && receivedEmail == "tran2l%402lltd.com")
            //        {

            //            if (!deviceInfoBLL.CheckDeviceByPhoneSN(plainSN))
            //            {
            //                if (!deviceInfoBLL.CheckDeviceByPaymentId(txToken))
            //                {
            //                    DeviceInfo di = new DeviceInfo();
            //                    di.ActiveFlag = "1";
            //                    di.DeviceId = plainSN;
            //                    di.PaypalStatus = "P";
            //                    di.PhoneSN = plainSN;
            //                    di.PaypalInfo = "";
            //                    di.PaymentId = txToken;
            //                    di.State = "";
            //                    di.PayPayDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            //                    di.ProductType = "live";
            //                    di.RecordDate = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            //                    if (deviceInfoBLL.AddDevice(di) > 0)
            //                    {
            //                        Response.Write("\n Paid Succefully, Please Validate The Result In Andy Magic App.");
            //                        Response.Write(System.Environment.NewLine + "<br/> 支付成功，请在安迪魔术应用中验证并使用完整版。");
            //                    }
            //                }
            //                else
            //                {
            //                    Response.Write("\n Paid Fail.");
            //                    Response.Write(System.Environment.NewLine + "<br/> 支付失败。 <br/>"); 
            //                }
            //            }
            //            else
            //            {
            //                Response.Write("\n Paid Succefully, Please Validate The Result In Andy Magic App.");
            //                Response.Write(System.Environment.NewLine + "<br/> 支付成功，请在安迪魔术应用中验证并使用完整版。");
            //            }                    
            //        }        
            //    }
            //    else
            //    {
            //        Response.Write("\n Paid Fail.");
            //        Response.Write(System.Environment.NewLine + "<br/> 支付失败。 <br/>"); 
            //    }
            //}
        }
    }
}