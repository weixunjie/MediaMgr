using AmagicServer.DataModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AmagicServer.BLL
{
    public class DeviceInfoBLL
    {
        private DbUitls _dbUitls = null;
        public DeviceInfoBLL()
        {
            _dbUitls = new DbUitls();
        }

        public List<DeviceInfo> GetAllDevices()
        {
            return GetAllDevicesBySql("select  * from DeviceInfo");
        }

        public List<DeviceInfo> GetAllDeviceByPhoneNoCloseMatch(string phoneNO)
        {
            return GetAllDevicesBySql("select  * from DeviceInfo where PhoneSN like '%" + phoneNO + "%'");
        }

        private List<DeviceInfo> GetAllDevicesBySql(string sqlStr)
        {
            List<DeviceInfo> allDeivice = new List<DeviceInfo>();
            using (SqlDataReader reader = _dbUitls.GetSqlRedaer(sqlStr))
            {
                // Loop over the results 
                while (reader.Read())
                {
                    DeviceInfo di = new DeviceInfo();
                    di.DeviceId = reader["deviceid"].ToString();
                    di.PhoneSN = reader["PhoneSN"].ToString();
                    di.PaypalStatus = reader["PaypalStatus"].ToString();
                    di.PaypalInfo = reader["PaypalInfo"].ToString();
                    di.PaymentId = reader["PaymentId"].ToString();
                    di.State = reader["State"].ToString();
                    di.PayPayDate = reader["PayPayDate"].ToString();
                    di.ProductType = reader["ProductType"].ToString();
                    di.RecordDate = reader["RecordDate"].ToString();
                    di.ActiveFlag = reader["ActiveFlag"].ToString();

                    allDeivice.Add(di);

                }

            }

            _dbUitls.ClosoConnection();


            return allDeivice;

            //CREATE table DeviceInfo(deviceid int identity(1,1),PhoneSN varchar(100),PaypalStatus varchar(20),RecordDate datetime, ActiveFlag int)
        }

        public int AddDevice(DeviceInfo di)
        {

            string cmdText = "insert into DeviceInfo(PhoneSN,PaypalStatus,RecordDate,ActiveFlag,PayPalInfo,PaymentId,State,ProductType,PayPayDate) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')";

            cmdText = String.Format(cmdText, di.PhoneSN, di.PaypalStatus, di.RecordDate, di.ActiveFlag,di.PaypalInfo,di.PaymentId,di.State,di.ProductType,di.PayPayDate);


            return _dbUitls.ExeSql(cmdText);

            //CREATE table DeviceInfo(deviceid int identity(1,1),PhoneSN varchar(100),PaypalStatus varchar(20),RecordDate datetime, ActiveFlag int)
        }

        public bool CheckDeviceByPhoneSN(string phoneSN)
        {

            using (SqlDataReader reader = _dbUitls.GetSqlRedaer("select  * from DeviceInfo where PhoneSN='" + phoneSN + "'"))
            {
                if (reader != null && reader.Read())
                {
                    _dbUitls.ClosoConnection();
                    return true;
                }
            }

            return false;
            //CREATE table DeviceInfo(deviceid int identity(1,1),PhoneSN varchar(100),PaypalStatus varchar(20),RecordDate datetime, ActiveFlag int)
        }





    }
}
