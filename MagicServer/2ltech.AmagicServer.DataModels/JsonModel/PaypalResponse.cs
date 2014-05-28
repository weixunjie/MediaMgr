using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmagicServer.DataModels
{

//    {
//    "response": {
//        "": "approved",
//        "": "PAY-2F934162FV500741LKNN2WHQ",
//        "": "2014-04-26T12:48:30Z",
//        "": "sale"
//    },
//    "": {
//        "": "Android",
//        "": "2.1.0",
//        "": "PayPal-Android-SDK",
//        "": "sandbox"
//    },
//    "": "payment"
//}

    public class client
    {

        public string platform
        {
            get;
            set;
        }

        public string paypal_sdk_version
        {
            get;
            set;
        }


        public string product_name
        {
            get;
            set;
        }

        public string environment
        {
            get;
            set;
        }
    }

    public class response
    {

        public string state
        {
            get;
            set;
        }

        public string id
        {
            get;
            set;
        }


        public string create_time
        {
            get;
            set;
        }

        public string intent
        {
            get;
            set;
        }
    }
    public class PaypalResponse
    {

        public response response
        {
            get;
            set;

        }

        public client client
        {
            get;
            set;

        }

    
        
    }
}
