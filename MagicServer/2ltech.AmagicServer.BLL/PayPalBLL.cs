using AmagicServer.DataModels;
using PayPal;
using PayPal.Api.Payments;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AmagicServer.BLL
{
    public class PayPalBLL
    {

        public PayPalBLL()
        {

        }

        public bool ValidatePaidPay(string clientId,string clientSecret,string payNumber)
        {

            Dictionary<string, string> configMap = new Dictionary<string, string>();

            // Endpoints are varied depending on whether sandbox OR live is chosen for mode
            configMap.Add("mode", "live");


            //    string accessToken = new OAuthTokenCredential("An5ns1Kso7MWUdW4ErQKJJJ4qi4", "Aj5QjaSGghY5xPnU6YETPXPCNumC", configMap).GetAccessToken();






          //  string accessToken = new OAuthTokenCredential("AciSJxAbs-e6uM61F_Vv1wuxh8LfkQdP0zS2ZO5ZBqzPFNdt-5kI_dSI9fTc", "EIMcTRCFvMAfRdvUFiHD8UqxAtesoa2fcU1duVzidXgrfK0d-h55bcuXLH4m", configMap).GetAccessToken();

            string accessToken = new OAuthTokenCredential(clientId, clientSecret, configMap).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = configMap;



            // These values are defaulted in SDK. If you want to override default values, uncomment it and add your value
            // configMap.Add("connectionTimeout", "360000");
            // configMap.Add("requestRetries", "1");

            // Retrieve the payment object by calling the
            // static `Get` method
            // on the Payment class by passing a valid
            // APIContext and Payment ID
            Payment pymnt = Payment.Get(apiContext, "PAY-2N358499KN117023XKN11UNZJI");

            if (pymnt != null && !string.IsNullOrWhiteSpace(pymnt.state))
            {
                if (pymnt.id == payNumber && pymnt.state.ToUpper()=="APPROVED")
                {

                    return true;
                }
            }

            return false;
        
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, @"");


            //request.Method = HttpMethod.Get;

            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ////request.Headers.Add("Bearer-Type", "An5ns1Kso7MWUdW4ErQKJJJ4qi4-Aj5QjaSGghY5xPnU6YETPXPCNumC");        




            //string responseString = string.Empty;


            //string response = httpClient.SendAsync(request);


            //APIContext apiContext = Configuration.GetAPIContext();


            //// Retrieve the payment object by calling the
            //// static `Get` method
            //// on the Payment class by passing a valid
            //// APIContext and Payment ID
            //Payment pymnt = Payment.Get(apiContext, "PAY-0XL713371A312273YKE2GCNI");


            //HttpClient c = new HttpClient();
            //c.BaseAddress = new Uri(@"https://api.sandbox.paypal.com/v1/payments/payment/");

            //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "PAY-16C81432391208717KNLXLBY");
            //req.Headers.Add("Bearer-Type", "APP-80W284485P519543T");

            ////req.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
            //c.SendAsync(req).ContinueWith(respTask =>
            //{
            //    Console.WriteLine("Response: {0}", respTask.Result);
            //});
        }




    }
}
