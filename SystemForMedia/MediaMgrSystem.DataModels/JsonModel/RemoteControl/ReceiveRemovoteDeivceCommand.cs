using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ReceiveRemovoteDeivceCommand : ComunicationBase
    {

        public string deviceType{get;set;}

        public string port{get;set;}

           public string state{get;set;}
 

     
    }
}
