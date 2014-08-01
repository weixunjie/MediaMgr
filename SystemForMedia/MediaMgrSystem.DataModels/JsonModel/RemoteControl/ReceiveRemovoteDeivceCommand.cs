using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ReceiveRemovoteDeivceCommand : ComunicationBase
    {

        public List<ReceiveRemovoteDeivceStatus> status
        {
            get;
            set;
        }

     
    }
}
