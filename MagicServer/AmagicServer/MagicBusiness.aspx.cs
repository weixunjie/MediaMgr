using AmagicServer.BLL;
using AmagicServer.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AmagicServer
{
    public partial class MagicBusiness : System.Web.UI.Page
    {
        private DeviceInfoBLL deviceInfoBLL = new DeviceInfoBLL();

        private PayPalBLL payPalBLL = new PayPalBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
          
            if (Request["SendPayStatus"] != null)
            {
                string reqSendPaySatus = Request["SendPayStatus"].ToString();
                string reqDeviceId = Request["DeviceId"].ToString();

                if (!string.IsNullOrWhiteSpace(reqSendPaySatus) && !string.IsNullOrWhiteSpace(reqDeviceId))
                {

                    string plainReqSendPaySatus = EncryUtils.DESDeCode(reqSendPaySatus);
                    string plainReqDeviceId = EncryUtils.DESDeCode(reqDeviceId);


                    //string deviceInf = EncryUtils.DESDeCode("yvHzuQv+klakmsnyQMKLGA=="); ;
                    if (!string.IsNullOrWhiteSpace(plainReqSendPaySatus))
                    {
                        //                       <add key="" value="AciSJxAbs-e6uM61F_Vv1wuxh8LfkQdP0zS2ZO5ZBqzPFNdt-5kI_dSI9fTc"/>

                        //<add key="" value="EIMcTRCFvMAfRdvUFiHD8UqxAtesoa2fcU1duVzidXgrfK0d-h55bcuXLH4"/>
                        PaypalResponse paypalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PaypalResponse>(plainReqSendPaySatus);
                        if (paypalResponse != null && paypalResponse.response != null &&
                            !string.IsNullOrWhiteSpace(paypalResponse.response.state) &&
                            !string.IsNullOrWhiteSpace(paypalResponse.response.id))
                        {
                            if (paypalResponse.response.state.ToString().ToUpper() == "APPROVED")
                            {
                                if (payPalBLL.ValidatePaidPay(ConfigurationManager.AppSettings["PayPalClientId"].ToString().TrimEnd(),
                                    ConfigurationManager.AppSettings["PayPalClientSecret"].ToString().TrimEnd(), paypalResponse.response.id))
                                {
                                    DeviceInfo di = new DeviceInfo();
                                    di.ActiveFlag = "1";
                                    di.DeviceId = plainReqDeviceId;
                                    di.PaypalStatus = "P";
                                    di.PhoneSN = plainReqDeviceId;
                                    di.PaypalInfo = plainReqSendPaySatus;
                                    di.PaymentId = paypalResponse.response.id;
                                    di.State = paypalResponse.response.state;
                                    di.PayPayDate = paypalResponse.response.create_time;
                                    di.ProductType = paypalResponse.client.environment;
                                    di.RecordDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    if (deviceInfoBLL.AddDevice(di) > 0)
                                    {
                                        Response.Write(EncryUtils.DESEnCode("1"));
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    Response.Write(EncryUtils.DESEnCode("0"));
                }
            }

            else if (Request["CheckDevice"] != null)
            {
                string phoneSN = EncryUtils.DESDeCode(Request["CheckDevice"].ToString());
                Response.Write(EncryUtils.DESEnCode(deviceInfoBLL.CheckDeviceByPhoneSN(phoneSN) == true ? "1" : "0"));
            }

            else if (Request["GetClientId"] != null)
            {                
                Response.Write(EncryUtils.DESEnCode(ConfigurationManager.AppSettings["PayPalClientId"].ToString().TrimEnd()));

            }
        }
    }
}