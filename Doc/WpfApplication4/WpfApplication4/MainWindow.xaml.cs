using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


    public class ComunicationBase
    {
        public string guidId
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
            Connetion("VIDEOSERVER", "192.168.1.54");
        }

        private void Connetion(string str, string ipa)
        {
            timer.Interval = TimeSpan.FromMilliseconds(40);

            timer.Tick += new EventHandler(timer_Tick);



            hubConnection = new HubConnection("http://localhost:19671/", "clientIdentify=" + ipa + "&clientType=" + str);


            hubProxy = hubConnection.CreateHubProxy("MediaMgrHub");

            hubConnection.Start();


            hubConnection.Received += hubConnection_Received;

            hubConnection.StateChanged += hubConnection_StateChanged;


            hubProxy.On<string>("sendMessageToClient", (i) =>
            {

                ComunicationBase a = Newtonsoft.Json.JsonConvert.DeserializeObject<ComunicationBase>(i);

                ComuResponseBase cb = new ComuResponseBase();
                cb.errorCode = "0";
                cb.guidId = a.guidId;


                Dispatcher.Invoke(
new Action(() =>
{
    lbLog.Items.Add(a.guidId+"  "+DateTime.Now.ToString("hh:mm:ss"));

    hubProxy.Invoke("SendMessageToMgrServer", Newtonsoft.Json.JsonConvert.SerializeObject(cb),hubConnection.ConnectionId);
}));



            });

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
                //VideoServerRegModel a = new VideoServerRegModel();
                //a.commandType = 130;
                //a.ConnectionId = hubConnection.ConnectionId;
                //a.IpAddress = "sss";
                //a.guidId = Guid.NewGuid().ToString();

                //hubProxy.Invoke("sendMessageToMgrServer", Newtonsoft.Json.JsonConvert.SerializeObject(a));

            }
            if (obj.NewState == ConnectionState.Disconnected)
            {
              
            }
        }

        void hubConnection_Received(string obj)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Connetion("ANDROID", ss.Text);
            //    hubConnection.Stop();

        }
    }
}
