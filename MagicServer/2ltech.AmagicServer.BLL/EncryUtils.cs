
using AmagicServer.DataModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AmagicServer.BLL
{
    public static class EncryUtils
    {
        public static string DES_Key = "tellme58";
        private static byte[] ivKey = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF }; 
        public static string DESEnCode(string pToEncrypt)
        {
            // string pToEncrypt1 = HttpContext.Current.Server.UrlEncode(pToEncrypt);   
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

            //建立加密对象的密钥和偏移量    
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法    
            //使得输入密码必须输入英文文本    
            des.Key = ASCIIEncoding.ASCII.GetBytes(DES_Key);
            des.IV = ivKey;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();


            return Convert.ToBase64String(ms.ToArray());
        }



        /// <summary>
        /// 对DES加密后的字符串进行解密
        /// </summary>
        /// <param name="encryptedString">待解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDeCode(string encryptedString)
        {
            byte[] btKey = Encoding.Default.GetBytes(DES_Key);
            byte[] btIV = ivKey;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }

    }
}
