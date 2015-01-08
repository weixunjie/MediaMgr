using MediaMgrSystem.DataModels;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication4
{


    public class ComunicationBase1
    {
        public string guidId
        {
            get;
            set;
        }

        public int commandType
        {
            get;
            set;
        }
    }

    public class ComuResponseBase
    {
        public string guidId
        {
            get;
            set;
        }

        public string errorCode
        {
            get;
            set;
        }
        public string message
        {
            get;
            set;
        }

    }



    public class ComunicationBase
    {
        public string guidId
        {
            get;
            set;
        }

        public int commandType
        {
            get;
            set;
        }

    }

    public class VideoServerRegModel : ComunicationBase
    {
        public string IpAddress
        {
            get;
            set;
        }

        public string ConnectionId
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        HubConnection hubConnection;
        DateTime currentTime;
        private string cdi;
        bool isSVR = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private int count;
        private IHubProxy hubProxy = null;
        private IDictionary<string, string> aa = new Dictionary<string, string>();

        private object lockObject = new object();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isSVR = true;
            DOConnetion("VIDEOSERVER", "192.168.1.54");
        }

        private void DOConnetion(string str, string ipa)
        {
            timer.Interval = TimeSpan.FromMilliseconds(40);

            timer.Tick += new EventHandler(timer_Tick);



            hubConnection = new HubConnection(tbAddress.Text, "clientIdentify=" + ipa + "&clientType=" + str);


            hubProxy = hubConnection.CreateHubProxy("MediaMgrHub");

            hubConnection.Start();


            hubProxy.On<string>("sendKeepAlive", (i) =>
            {



                Dispatcher.Invoke(
new Action(() =>
{
    lbLog.Items.Add(i + "  " + DateTime.Now.ToString("MM/dd HH:mm:ss"));

}));



            });


            hubProxy.On<string>("sendMessageToClient", (i) =>
            {

                ComunicationBase a = Newtonsoft.Json.JsonConvert.DeserializeObject<ComunicationBase>(i);

                ComuResponseBase cb = new ComuResponseBase();
                
                cb.errorCode = "0";
                cb.guidId = a.guidId;
                cdi = a.guidId;

                Dispatcher.Invoke(
new Action(() =>
{
    lbLog.Items.Add(a.guidId + "  " + DateTime.Now.ToString("MM/dd HH:mm:ss"));

    hubProxy.Invoke("SendMessageToMgrServer", Newtonsoft.Json.JsonConvert.SerializeObject(cb), hubConnection.ConnectionId);
}));



            });





            hubConnection.StateChanged += hubConnection_StateChanged;


        }

        private void messageReceived(string a)
        {


        }

        void timer_Tick(object sender, EventArgs e)
        {
            //lock (lockObject)
            //{

            //    count++;

            //    if (count <= 100)
            //    {
            //        TimeSpan TS = DateTime.Now.Subtract(currentTime);
            //        ss.Text =((int)TS.TotalMilliseconds).ToString();

            //    }
            //    else
            //    {
            //        timer.Stop();

            //        hubProxy.Invoke("SendTimeToServer", ss.Text);
            //        count = 0;
            //    }



            //}




        }

        void hubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
            {

                Dispatcher.Invoke(
new Action(() =>
{
    lbLog.Items.Add(" CONNECTED " + DateTime.Now.ToString("MM/dd HH:mm:ss"));


}));


            }
            if (obj.NewState == ConnectionState.Disconnected)
            {

                Dispatcher.Invoke(
new Action(() =>
{
    lbLog.Items.Add(" DISCONNECTED " + DateTime.Now.ToString("MM/dd HH:mm:ss"));



    new Thread(RunReConnection).Start();



}));

    
            }
        }


        private void RunReConnection()
        {

            System.Threading.Thread.Sleep(10000);


            Dispatcher.Invoke(
       new Action(() =>
       {
           lbLog.Items.Add(" DISCONNECTED " + DateTime.Now.ToString("MM/dd HH:mm:ss"));


           System.Threading.Thread.Sleep(10000);

           if (isSVR)
           {

               DOConnetion("VIDEOSERVER", "192.168.1.54");
           }
           else
           {
               DOConnetion("ANDROID", ss.Text);
           }


       }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isSVR = false;
            DOConnetion("ANDROID", ss.Text);
            //    hubConnection.Stop();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            ReceiveCommand rec = new ReceiveCommand();
            rec.guidId = Guid.NewGuid().ToString();
            rec.commandType = 114;
            rec.arg = new ReceiveCommandBaseArg();
            rec.arg.streamName = tbSstreamname.Text;

            hubProxy.Invoke("SendMessageToMgrServer", Newtonsoft.Json.JsonConvert.SerializeObject(rec), hubConnection.ConnectionId);


        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            DOConnetion("ENCODERFORAUDIO", ss.Text);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ReceiveCommand rc=new ReceiveCommand();
            
    
            rc.commandType = 114;
            rc.arg=new ReceiveCommandBaseArg();
            rc.arg.streamName = "12345678902";
            rc.arg.errorCode = "0";
            hubProxy.Invoke("SendMessageToMgrServer", Newtonsoft.Json.JsonConvert.SerializeObject(rc), hubConnection.ConnectionId);
        }
    }

   
}
