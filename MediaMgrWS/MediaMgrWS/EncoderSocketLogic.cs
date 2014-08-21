using MediaMgrSystem.DataAccessLayer;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrWS
{
    public static class EncoderSocketLogic
    {
        public static SocketPermission permission;
        public static  Socket sListener;
        public static IPEndPoint ipEndPoint;
        public static Socket handler;

        public static object lockClients = new object();


        public static object lockEncoderCommand = new object();
        public static List<SocketClients> allEncoderClient = new List<SocketClients>();

        public static List<ToEncoderCommand> allQueueCommandToEncoder = new List<ToEncoderCommand>();

        public static EncoderBLL EncoderBLLInstance = null;


        public static IHubProxy hubProxy;   
        public static void OpenSocketServer()
        {
            try
            {
                // Creates one SocketPermission object for access restrictions
                permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections 
                TransportType.Tcp,        // Defines transport types 
                "",                       // The IP addresses of local host 
                SocketPermission.AllPorts // Specifies all ports 
                );

                // Listening Socket object 
                sListener = null;

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance 
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint 
                ipEndPoint = new IPEndPoint(ipAddr, 4510);

                // Create one Socket object to listen the incoming connection 
                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint 
                sListener.Bind(ipEndPoint);


                sListener.Listen(10);

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);



            }
            catch (Exception ex) { }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication 
            Socket handler = null;
            try
            {
                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                listener = (Socket)ar.AsyncState;
                // Create a new socket 
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm 
                handler.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data 
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception exc) { }
        }


        public static SocketClients GetConnectedEcoderSocket(string clientIdentify)
        {
            SocketClients sc = null;
            lock (lockClients)
            {
                sc = null;
                foreach (var c in allEncoderClient)
                {
                    if (c.ClientIdentify == clientIdentify)
                    {
                        sc = c;
                        break;
                    }
                }
            }

            return sc;
        }
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information 
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication. 
                handler = (Socket)obj[1];

                // Received message 
                string content = string.Empty;


                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);

                    if (!string.IsNullOrWhiteSpace(content))
                    {

                        EncoderOperationCommand epc = Newtonsoft.Json.JsonConvert.DeserializeObject<EncoderOperationCommand>(content);


                        if (epc.commandType == CommandTypeEnum.ENCODEROPEN)
                        {

                            if (epc != null && epc.groupIds != null && epc.groupIds.Count > 0)
                            {
                                string groupIds = string.Empty;

                                foreach (var g in epc.groupIds)
                                {
                                    groupIds += g + ",";
                                }


                                EncoderInfo ei = EncoderBLLInstance.GetEncoderList(epc.clientIdentify);

                                if (ei != null)
                                {
                                    RunningEncoder re = EncoderBLLInstance.CheckIfEncoderRunning(epc.clientIdentify);
                                    List<RunningEncoder> reAllRunning =  EncoderBLLInstance.GetAllEncoderRunning();

                                    bool isHighPriorityRunning = false;
                                    if (reAllRunning != null && reAllRunning.Count > 0)
                                    {
                                        foreach (var r in reAllRunning)
                                        {
                                            if (int.Parse(r.Priority) > int.Parse(ei.Priority))
                                            {

                                                isHighPriorityRunning = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (re != null)
                                    {
                                        ComuResponse cr = new ComuResponse();
                                        cr.errorCode = "110";
                                        cr.message = "已经打开";
                                        cr.guidId = epc.guidId;
                                        SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cr), handler);

                                    }
                                    else if (isHighPriorityRunning)
                                    {
                                        ComuResponse cr = new ComuResponse();
                                        cr.errorCode = "111";
                                        cr.message = "优先级低，不能打开";
                                        cr.guidId = epc.guidId;
                                        SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cr), handler);
                                    }
                                    else
                                    {

                                        foreach (var r in reAllRunning)
                                        {
                                            StopEncoder(epc.guidId, epc.clientIdentify, r.GroupIds, true);

                                        }

                                        EncoderOpenReponse eor = new EncoderOpenReponse();

                                        eor.arg = new EncoderOpenReponseArg();
                                        eor.arg.baudRate = ei.BaudRate;
                                        eor.arg.streamName = "1234567890" + ei.EncoderId;
                                        eor.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;

                                        eor.guidId = epc.guidId;
                                        eor.errorCode = "0";

                                         EncoderBLLInstance.UpdateRunningEncoder(new RunningEncoder() { ClientIdentify = re.ClientIdentify, GroupIds = groupIds, Priority = re.Priority });
                                        SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(eor), handler);

                                        hubProxy.Invoke("sendEncoderTaskControlOpen", groupIds.TrimEnd(','), epc.clientIdentify);
                                    }
                                }


                            }
                        }
                        else if (epc.commandType == CommandTypeEnum.ENCODERQUIT)
                        {

                            RunningEncoder re = EncoderBLLInstance.CheckIfEncoderRunning(epc.clientIdentify);


                            if (re != null)
                            {
                                StopEncoder(epc.guidId, epc.clientIdentify, re.GroupIds, false);
                            }
                            else
                            {
                                ComuResponse cr = new ComuResponse();
                                cr.errorCode = "123";
                                cr.message = "未打开，不能关闭";
                                cr.guidId = epc.guidId;
                                SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cr), handler);
                            }


                        }

                        else
                        {

                            CommandResponse commandResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandResponse>(content);

                            if (commandResponse != null)
                            {


                                lock (lockEncoderCommand)
                                {
                                    ToEncoderCommand tcToRemoved = null;
                                    if (allQueueCommandToEncoder != null && allQueueCommandToEncoder.Count > 0)
                                    {
                                        foreach (var cmd in allQueueCommandToEncoder)
                                        {
                                            if (cmd.GuidId == commandResponse.guidId)
                                            {
                                                tcToRemoved = cmd;
                                                break;
                                            }
                                        }
                                    }

                                    if (tcToRemoved != null)
                                    {
                                        allQueueCommandToEncoder.Remove(tcToRemoved);
                                    }

                                    hubProxy.Invoke("sendEncoderTaskControlCommandBack", Newtonsoft.Json.JsonConvert.SerializeObject(commandResponse));

                                }
                            }
                            else
                            {

                                EncoderCommandBase ecb = Newtonsoft.Json.JsonConvert.DeserializeObject<EncoderCommandBase>(content);
                                if (ecb != null && ecb.commandType == CommandTypeEnum.ENCODERREG)
                                {

                                    lock (lockClients)
                                    {
                                        SocketClients objToRemoved = null;
                                        foreach (var c in allEncoderClient)
                                        {
                                            if (c.ClientIdentify == ecb.clientIdentify)
                                            {
                                                objToRemoved = c;
                                                break;
                                            }
                                        }

                                        if (objToRemoved != null)
                                        {
                                            allEncoderClient.Remove(objToRemoved);
                                        }

                                        allEncoderClient.Add(new SocketClients() { ClientIdentify = ecb.clientIdentify, SokcetInstance = handler });
                                    }

                                    EncoderSendGroupsInfoCommand cmd = new EncoderSendGroupsInfoCommand();
                                    cmd.commandType = CommandTypeEnum.ENCODERSENDGROUPSINFO;
                                    cmd.guidId = Guid.NewGuid().ToString();
                                    cmd.groups = new List<GroupInfo>();
                                    cmd.groups.Add(new GroupInfo { groupId = "1", groupName = "f" });
                                    cmd.groups.Add(new GroupInfo { groupId = "3", groupName = "bf" });

                                    foreach (var c in allEncoderClient)
                                    {
                                        if (c.ClientIdentify != ecb.clientIdentify)
                                            SendSocketDataToClient(content, c.SokcetInstance);
                                    }
                                }

                                    // SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));


                                else if (ecb != null && ecb.commandType == CommandTypeEnum.ENCODERQUIT)
                                {
                                    lock (lockClients)
                                    {
                                        SocketClients objToRemoved = null;
                                        foreach (var c in allEncoderClient)
                                        {
                                            if (c.ClientIdentify == ecb.clientIdentify)
                                            {
                                                objToRemoved = c;
                                                break;
                                            }
                                        }

                                        if (objToRemoved != null)
                                        {
                                            allEncoderClient.Remove(objToRemoved);
                                        }

                                    }
                                }
                            }

                        }
                    }


                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {

                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }



                }
            }
            catch (Exception exc) { }
        }

        private static void StopEncoder(string guiId, string clientIdentify, string groupIds, bool isStopOther)
        {

            //            {"guidId":"2847f884-a55b-4375-aca4-a7f2f2df08b9","commandType":"403"} }


            //guidId 唯一的指令编号，由服务器生成。
            //commandType  指令编号
            //             403： 表示通知知呼叫台停止          

            string strToSend = string.Empty;
            if (isStopOther)
            {
                EncoderCommandBase ec = new EncoderCommandBase();
                ec.commandType = CommandTypeEnum.ENCODERCLOSE;
                ec.guidId = Guid.NewGuid().ToString();
                strToSend = Newtonsoft.Json.JsonConvert.SerializeObject(ec);

            }
            else
            {
                ComuResponse cr = new ComuResponse();
                cr.errorCode = "0";
                cr.message = "";
                cr.guidId = guiId;
                strToSend = Newtonsoft.Json.JsonConvert.SerializeObject(cr);
            }

             EncoderBLLInstance.RemoveRunningEncoder(clientIdentify);
            SendSocketDataToClient(strToSend, GetConnectedEcoderSocket(clientIdentify).SokcetInstance);
            hubProxy.Invoke("sendEncoderTaskControlClose", clientIdentify, groupIds);
        }


        public static void SendSocketDataToClient(string str, Socket socketToSend)
        {
            try
            {

                // Prepare the reply message 
                byte[] byteData =
                    Encoding.Unicode.GetBytes(str);

                // Sends data asynchronously to a connected Socket 
                socketToSend.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), socketToSend);


            }
            catch (Exception exc) { }
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // A Socket which has sent the data to remote host 
                Socket handler = (Socket)ar.AsyncState;

                // The number of bytes sent to the Socket 
                int bytesSend = handler.EndSend(ar);

            }
            catch (Exception exc) { }
        }

        public static void CloseSocketServer()
        {
            try
            {
                if (sListener != null)
                {
                    sListener.Shutdown(SocketShutdown.Both);
                    sListener.Close();
                }


            }
            catch (Exception exc) { }
        }


    }
}
