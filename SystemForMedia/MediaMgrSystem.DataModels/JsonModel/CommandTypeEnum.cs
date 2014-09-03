using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public enum CommandTypeEnum
    {
        PLAYVEDIO = 111,
        STOPVEDIO = 112,

        REPEATPLAY = 113,

        DEVICE_OPER_OPENS_SCREEN = 122,
        DEVICE_OPER_CLOSE_SCREEN = 123,

        DEVICE_OPER_RESTART = 124,

        DEVICE_OPER_SHUTDOWN = 125,


        DEVICE_OPER_SCHEDULE_TURNONANDSHUTDOWN = 127,


        DEVICE_OPER_CHANGE_IP_ADDRESS = 128,

        STREAMSFINISHED = 114,




        REMOTECONTRLMANUALOPEN = 211,



        REMOTECONTRLMANUALCLOSE = 212,

        REMOTECONTRLMANUALCHANGEPARAM = 213,


        REMOTECONTRLSCHEDULEOPEN = 221,



        REMOTECONTRLSCHEDULECLOSE = 222,

        REMOTECONTRLSCHEDULECHANGEPARAM = 223,


        REMOTECONTRLSENDSTATUS = 231,

        ENCODEROPEN = 402,


        ENCODERCLOSE = 403,

    }
}
