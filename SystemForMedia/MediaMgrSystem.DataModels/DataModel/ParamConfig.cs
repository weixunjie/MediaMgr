using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class UserInfo
    {
        public String UserId { get; set; }

        public String UserCode { get; set; }

        public String UserName { get; set; }





        public String UserLevel { get; set; }

        public bool IsActive { get; set; }

        public string Password
        {
            get;
            set;
        }

    }

}
