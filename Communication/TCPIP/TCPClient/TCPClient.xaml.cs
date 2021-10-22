using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//////////////// USE //////////////////
//// Send test data to the remote device.
//Send("This is a test<EOF>");
//sendDone.WaitOne();

//// Receive the response from the remote device.
//Receive(client);
//receiveDone.WaitOne();

namespace AutomationControls.Communication.TCPIP.UserControls
{
    public partial class TCPClient : UserControl
    {

        #region PropertyChanged Pattern

        public static event PropertyChangedEventHandler PropertyChanged;

        protected static void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(name));
            }
        }

        #endregion


        //private TcpClientVM _data = new TcpClientVM();
        //public TcpClientVM data
        //{
        //    get { return _data; }
        //    set
        //    {
        //        _data = value;
        //        this.DataContext = data;
        //        OnPropertyChanged("data");
        //    }
        //}



        //public CancellationTokenSource cts = new CancellationTokenSource();

        //public IProgress<Status.ConnectionStatus> progressConnectionStatus = new Progress<Status.ConnectionStatus>();
        //public IProgress<String> progressSend = new Progress<string>();
        //public IProgress<String> progressReceive = new Progress<string>();

        public TCPClient()
        {
            InitializeComponent();
        }

        //public TCPClient(string ip, int port)
        //{
        //    InitializeComponent();
        //   // data.ipAddress = ip; //tbip.Text = ip;
        //   // data.port = port; //tbport.Text = port.ToString();
        //   // this.DataContext = this;
        //}


        //public async Task ConnectAsync(String ipAddress, int port, int timeout = 2000)
        //{ 
        //    if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connecting);
        //    CancellationTokenSource ctsConnect = new CancellationTokenSource(); cts.CancelAfter(timeout);
        //    Task task = Task.Run(() =>
        //    {
        //            try
        //            {
        //                // Establish the remote endpoint for the socket.
        //                data.socket = data.CreateSocket();
        //                IPAddress addr;
        //                if(!IPAddress.TryParse(ipAddress, out  addr) || !(port > 0))
        //                {
        //                    if(progressSend != null) progressSend.Report("Invalid IP Address supplied: " + ipAddress);
        //                    if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //                    return;
        //                }
        //                data.ipAddress = addr.ToString();
        //                data.ipEndPoint = new IPEndPoint(addr, port);
        //                if(data.ipEndPoint == null)
        //                {
        //                    if(progressSend != null) progressSend.Report("Unable to establish remote endpoint");
        //                    if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //                    return;
        //                }

        //                if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connecting);
        //                if(progressSend != null) progressSend.Report("Connecting to " + data.ipEndPoint.ToString());

        //                // Create a TCP/IP socket.
        //                data.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //                // Connect to the remote endpoint.
        //                data.socket.Connect(data.ipEndPoint);

        //                if(progressSend != null) progressSend.Report("Connection Successful");
        //                if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connected);

        //            } catch(Exception e)
        //            {
        //                Console.WriteLine(e.ToString());
        //                if(progressSend != null) progressSend.Report("Connection Failed!");
        //                if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //        }

        //    }, ctsConnect.Token);
        //    await task;
        //}

        //public async Task ConnectAsync()
        //{
        //    await Task.Run(() =>
        //    {
        //        try
        //        {
        //            if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connecting);
        //            // Establish the remote endpoint for the socket.
        //            data.socket = data.CreateSocket();
        //            IPAddress addr;
        //            if(!IPAddress.TryParse(data.ipAddress, out  addr) || !(data.port > 0))
        //            {
        //                if(progressSend != null) progressSend.Report("Invalid IP Address supplied: " + data.ipAddress);
        //                if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //                return;
        //            }
        //            data.ipAddress = addr.ToString();
        //            data.ipEndPoint = new IPEndPoint(addr, data.port);
        //            if(data.ipEndPoint == null)
        //            {
        //                if(progressSend != null) progressSend.Report("Unable to establish remote endpoint");
        //                if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //                return;
        //            }

        //            if(progressSend != null) progressSend.Report("Connecting to " + data.ipEndPoint.ToString());

        //            // Create a TCP/IP socket.
        //            data.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //            // Connect to the remote endpoint.
        //            data.socket.Connect(data.ipEndPoint);

        //            if(progressSend != null) progressSend.Report("Connection Successful");
        //            if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connected);

        //        } catch(Exception e)
        //        {
        //            Console.WriteLine(e.ToString());
        //            if(progressSend != null) progressSend.Report("Connection Failed!");
        //            if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.ConnectionFailed);
        //        }
        //    }, cts.Token);
        //}

        //public async Task DisconnectAsync()
        //{
        //    try
        //    {
        //        if(await data.CheckConnectionStatus() != Status.ConnectionStatus.Disconnected)
        //        {
        //            if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Disconnecting);
        //            await data.socket.DisconnectAsync(true, progressSend);
        //            if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Disconnected);
        //        }

        //        else { if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Disconnected); }
        //    } catch
        //    { if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.DisconnectFailed);  }
        //}

        //public async Task SendAsync(string p)
        //{
        //    if(await data.CheckConnectionStatus() == Status.ConnectionStatus.Connected)
        //    {
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Sending);
        //        await this.data.socket.SendAsync(p, progressSend);
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connected);
        //    }
        //}

        //public async Task SendAsync(byte[] b)
        //{
        //    if(await data.CheckConnectionStatus() == Status.ConnectionStatus.Connected)
        //    {
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Sending);
        //        await this.data.socket.SendAsync(b, progressSend);
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connected);

        //    }
        //}

        //public async Task<string> ReceiveAsync(TimeSpan receiveTime)
        //{
        //    string ret = string.Empty;
        //    if(await data.CheckConnectionStatus() == Status.ConnectionStatus.Connected)
        //    {
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Receving);
        //        ret = await data.socket.ReceiveAsync(receiveTime, progressSend);
        //        if(progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(Status.ConnectionStatus.Connected);

        //    }
        //    return ret;
        //}

        //public async Task MonitorConnectionStatus(int refreshRate)
        //{
        //    if(connectionMonitorTask == null) return;
        //    while(!connectionMonitorTask.IsCanceled)
        //    {
        //        connectionStatus = await data.CheckConnectionStatus();
        //        if(progressConnectionStatus != null) progressConnectionStatus.Report(connectionStatus); ;
        //        await Task.Delay(refreshRate);
        //    }
        //}


        //private Task connectionMonitorTask;
        private void cbMonitorConnectionStatus_Checked(object sender, RoutedEventArgs e)
        {
            //switch(cbMonitorConnectionStatus.IsChecked)
            //{
            //case true:
            //    ((Progress<Status.ConnectionStatus>)data.progressConnectionStatus).ProgressChanged += (sender2, e2) =>
            //    {
            //        lblConnectionToggle.Content = e2.ToString();
            //    };
            //    connectionMonitorTask = MonitorConnectionStatus(2000);
            //    await connectionMonitorTask;
            //    break;
            //case false:
            //    connectionMonitorTask.Dispose();
            //    break;
            //}
        }

        #region Control Handlers

        private async void miConnectClient_Click(object sender, RoutedEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            await data.ConnectAsync(data.ipAddress, data.port);
            StartMonitor();
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            await data.SendAsync(tbSend.Text);
        }


        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            data.cts.Cancel();
            await data.DisconnectAsync();
            data.socket = null;
            data.cts = new CancellationTokenSource();
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            CheckBox cb = sender as CheckBox;
            switch (cb.IsChecked)
            {
                case true:
                    StartMonitor();
                    //  data.socket.ReceiveAsync(data.progressReceive, data.progressSend, data.cts.Token);
                    break;
                case false:
                    data.cts.Cancel();
                    await data.DisconnectAsync();
                    data.socket = null;
                    break;
            }
        }

        private void StartMonitor()
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            data.cts.Cancel();
            data.cts = new CancellationTokenSource();

            data.RaiseDataReceivedEvent += (sender, e) =>
            {
                String s = e.buffer;
            };
            data.progressReceive.ProgressChanged += (sender, e) =>
            {
                string s = e;
            };
        }


        private async void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            if (e.Key == Key.Enter) { await data.SendAsync(tbSend.Text); }
        }


        private async void TextBox_KeyDown_ConnectAsync(object sender, KeyEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            if (e.Key == Key.Enter)
            {
                await data.ConnectAsync();
            }
        }

        private async void lblConnectionToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;
            switch (data.CheckConnectionStatus())
            {
                case Status.ConnectionStatus.Disconnected:
                    await data.ConnectAsync();
                    await Task.Delay(200);
                    StartMonitor();
                    break;
                case Status.ConnectionStatus.Connected:
                    await data.DisconnectAsync();
                    break;
                case Status.ConnectionStatus.Connecting:
                    break;
                case Status.ConnectionStatus.Disconnecting:
                    await data.ConnectAsync();
                    await Task.Delay(200);
                    StartMonitor();
                    break;
                case Status.ConnectionStatus.Sending:
                    break;
                case Status.ConnectionStatus.Receving:
                    break;
                case Status.ConnectionStatus.ConnectionFailed:
                    await data.ConnectAsync();
                    await Task.Delay(200);
                    StartMonitor();
                    break;
                case Status.ConnectionStatus.DisconnectFailed:
                    await data.ConnectAsync();
                    await Task.Delay(200);
                    StartMonitor();
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TcpClientVM data = DataContext as TcpClientVM;
            if (data == null) return;

            ((Progress<Status.ConnectionStatus>)data.progressConnectionStatus).ProgressChanged += (sender2, e2) =>
            {
                lblConnectionToggle.Dispatcher.Invoke((Action)(() => { lblConnectionToggle.Content = e2; }));
            };

            commMonitor.Load(data.progressSend, data.progressReceive);
            cbMonitorConnectionStatus.Dispatcher.Invoke((Action)(() =>
            {
                if (!(bool)cbMonitorConnectionStatus.IsChecked)
                    cbMonitorConnectionStatus.IsChecked = true;
            }));
        }
    }
}

