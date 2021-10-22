using AutomationControls.Attributes;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace AutomationControls.Communication.TCPIP
{
    [Guid("B595F855-0647-4BE2-87D1-044F20ABB3AF")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ITcpClientVM
    {
        //global::AutomationControls.Communication.Status.ConnectionStatus connectionStatus { get; set; }
        //global::System.Net.Sockets.Socket CreateSocket();
        string ipAddress { get; set; }
        string ipStatus { get; set; }
        string location { get; set; }
        string macAddress { get; set; }
        string model { get; set; }
        string notes { get; set; }
        string Ping();
        string PingWait();
        int port { get; set; }
        string receiveBuffer { get; set; }
        object tag { get; set; }
        void Connect();
        void Disconnect();
        string Receive();
        void Send(string p);
        void SendBytes(ref Byte[] b);
    }


    [GuidAttribute("1A585C4D-3371-48dd-AF8A-AFFECC1B0967")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface TcpClientVMEvents
    {
        void StatusDelegate(string status);
        void SendDelegate(string status);
        void ReceiveDelegate(string status);
    }

    [Serializable]
    [ComVisible(true)]
    [Guid("40BCD4C9-60EB-482D-9F87-9DA6D0DEBDF7")]
    [ComSourceInterfaces(typeof(TcpClientVMEvents))]
    [DataProfileAttribute(typeof(AutomationControls.Communication.TCPIP.UserControls.TCPClient))]
    // [DataProfileAttribute(typeof(AutomationControls.Communication.TCPIP.UserControls.TCPServer))]
    public class TcpClientVM : ITcpClientVM, ISerializable, IProvideUserControls, ISqlDbInfo
    {
        #region "INotifyPropertyChanged Pattern"

        public event PropertyChangedEventHandler PropertyChangedEvent = delegate { };
        public void OnPropertyChanged(string name)
        {
            if (PropertyChangedEvent != null)
            {
                PropertyChangedEvent(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
        #region COM Events

        public delegate void StatusUpdatedDelegate(string status);
        public event StatusUpdatedDelegate StatusDelegate;
        public void CauseStatusUpdated(string status)
        {
            try { if (StatusDelegate != null) StatusDelegate(status); } catch { }
        }


        public delegate void ReceiveUpdatedDelegate(string status);
        public event ReceiveUpdatedDelegate ReceiveDelegate;
        public void CauseReceiveUpdated(string status)
        {
            try { if (ReceiveDelegate != null) ReceiveDelegate(status); } catch { }
        }

        public delegate void SendUpdatedDelegate(string status);
        public event SendUpdatedDelegate SendDelegate;
        public void CauseSendUpdated(string status)
        {
            try { if (SendDelegate != null) SendDelegate(status); } catch { }
        }

        #endregion

        public CancellationTokenSource cts = new CancellationTokenSource();

        [XmlIgnore]
        public Progress<Status.ConnectionStatus> progressConnectionStatus = new Progress<Status.ConnectionStatus>();
        [XmlIgnore]
        public Progress<String> progressSend = new Progress<string>();
        [XmlIgnore]
        public Progress<String> progressReceive = new Progress<string>();

        public TcpClientVM()
        {
            try
            {
                IPAddress[] ips = Dns.GetHostAddresses("");
                if (ips == null) return;
                ipAddress = ips[ips.Length - 1].ToString();
                port = 80;
                IpAddress = IPAddress.Parse(_ipAddress);
                _ipEndPoint = new IPEndPoint(IpAddress, port);
                socket = CreateSocket();

                ((Progress<Status.ConnectionStatus>)progressConnectionStatus).ProgressChanged += (sender, e) =>
                {
                    connectionStatus = e;
                };
            }
            catch (Exception e)
            {

                e.Message.ToFile("C:\tcperr.txt");
            }

            ((Progress<String>)progressSend).ProgressChanged += (sender, e) => { CauseSendUpdated(e); };
            ((Progress<String>)progressReceive).ProgressChanged += (sender, e) => { CauseReceiveUpdated(e); };
        }



        public void Connect()
        {
            if (CheckConnectionStatus() != Status.ConnectionStatus.Connected
                || CheckConnectionStatus() != Status.ConnectionStatus.Connecting)
            {
                try
                {
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connecting);

                    // Establish the remote endpoint for the socket.
                    socket = CreateSocket();
                    IPAddress addr;
                    if (!IPAddress.TryParse(ipAddress, out addr) || !(port > 0))
                    {
                        ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.ConnectionFailed);
                        return;
                    }
                    ipAddress = addr.ToString();
                    ipEndPoint = new IPEndPoint(addr, port);
                    if (ipEndPoint == null)
                    {
                        if (progressSend != null) ((IProgress<String>)progressSend).Report("Unable to establish remote endpoint");
                        ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.ConnectionFailed);
                        return;
                    }

                    // Create a TCP/IP socket.
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // Connect to the remote endpoint.
                    socket.Connect(ipEndPoint);

                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connected);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.ConnectionFailed);
                }
            }
        }

        public Task ConnectAsync() { return Task.Run(() => { Connect(); }, cts.Token); }

        public Task ConnectAsync(String ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            return ConnectAsync();
        }

        public void Disconnect()
        {
            if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
            {
                try
                {
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Disconnecting);
                    socket.Disconnect(true);
                    cts.Cancel();
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Disconnected);
                    if (cts.IsCancellationRequested) cts = new CancellationTokenSource();
                }
                catch { ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.DisconnectFailed); }
            }
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() => { Disconnect(); }, CancellationToken.None);
        }

        public void Send(string p)
        {
            Connect();
            if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
            {
                if (socket != null)
                {
                    string s = p;
                    try
                    {
                        this.socket.Send(Encoding.ASCII.GetBytes(s));
                        ReportSend("Sent " + p.Length.ToString() + " Bytes --- " + s);
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        if (!keepOpen)
                            Disconnect();
                    }
                }
            }
        }

        public void SendBytes(ref Byte[] b)
        {
            if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
            {
                ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Sending);
                List<byte> lst = new List<byte>(b);

                this.socket.Send(lst.ToArray());
                ReportSend("Sent " + lst.Count.ToString() + " Bytes ");
                ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connected);
            }
            if (!keepOpen)
                Disconnect();
        }

        public void SendStream(ref Stream s)
        {
            if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
            {
                ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Sending);

                List<byte> bytes = new List<byte>();
                using (NetworkStream net = new NetworkStream(socket))
                {
                    int b = 0;
                    while (b != -1)
                    {
                        b = s.ReadByte();
                        if (b > -1) bytes.Add(Convert.ToByte(b));
                    }
                    net.WriteAsync(bytes.ToArray(), 0, bytes.Count);
                }


                ReportSend("Sent " + bytes.Count.ToString());
                ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connected);
            }
            if (!keepOpen)
                Disconnect();
        }

        public string Receive()
        {
            string s = "";
            if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
            {
                if (socket != null)
                {
                    if (socket.Available == 0) return "";
                    byte[] b = new byte[4096];
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Receving);
                    socket.Receive(b);
                    s = Encoding.ASCII.GetString(b);
                    ReportReceive(s);
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connected);
                }
            }
            if (!keepOpen)
                Disconnect();
            return s;
        }



        public Task SendAsync(string p, CancellationToken ct = default(CancellationToken))
        {
            return Task.Run(() => { Send(p); }, ct).ContinueWith((e) =>
            {
                if (!keepOpen)
                    Disconnect();
            });
        }

        public Task SendAsync(byte[] b, CancellationToken ct = default(CancellationToken))
        {
            // return Task.Run(() => { SendBytes(ref b); }, ct);
            return socket.SendAsync(b, null, cts.Token).ContinueWith((e) =>
            {
                if (!keepOpen)
                    Disconnect();
            });
            // return SendAsync(b, ct);
        }

        public Task<string> ReceiveAsync(int milliseconds)
        {
            CancellationTokenSource cts = new CancellationTokenSource(milliseconds);
            return Task<string>.Factory.StartNew(() =>
            {
                string ret = "";
                if (CheckConnectionStatus() == Status.ConnectionStatus.Connected)
                {
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Receving);
                    ret = Receive();
                    ReportConnectionStatus(AutomationControls.Communication.Status.ConnectionStatus.Connected);
                }
                ReportReceive("Received " + ret.Length.ToString() + " Bytes --- " + ret);
                return ret;
            }, cts.Token);
        }

        //public Progress<String> ReceiveAsyncTask()
        //{
        //    socket.ReceiveAsync(progressReceive);
        //    return (Progress<String>)progressReceive;
        //}

        public string PingWait()
        {
            string ret = "";
            Ping ping = new Ping();
            ping.PingCompleted += (sender, e) =>
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                //{
                if (e.Reply == null) return;
                ret = e.Reply.Status.ToString();
                ipStatus = ret;
                //}));

            };
            ping.SendPingAsync(_ipAddress, 6000);
            while (ret == "") Task.Delay(200);
            return ret;
        }

        public string Ping(String ipAddress)
        {
            string ret = "";
            Ping ping = new Ping();
            ping.PingCompleted += (sender, e) =>
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                //{
                if (e.Reply == null) return;
                ret = e.Reply.Status.ToString();
                ipStatus = ret;
                //}));

            };
            ping.SendPingAsync(ipAddress, 6000);
            return ret;
        }

        public string Ping()
        {
            string ret = "";
            Ping ping = new Ping();
            ping.PingCompleted += (sender, e) =>
            {
                if (e.Reply == null) return;
                ret = e.Reply.Status.ToString();
                ipStatus = ret;
            };
            ping.SendPingAsync(_ipAddress, 6000);
            return ret;
        }

        public Task<string> PingAsync() { return Task<string>.Factory.StartNew(() => Ping()); }

        public Task MonitorConnectionStatus(int refreshRate)
        {
            return new Task(new Action(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    connectionStatus = CheckConnectionStatus();
                    ReportConnectionStatus(connectionStatus);
                    Thread.Sleep(refreshRate);
                }
            }));
        }

        private Status.ConnectionStatus _connectionStatus;
        [XmlIgnore]
        public Status.ConnectionStatus connectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                CauseStatusUpdated(value.ToString());
                OnPropertyChanged("connectionStatus");

            }
        }

        private string _ipStatus = "";
        public string ipStatus
        {
            get { return _ipStatus; }
            set
            {
                _ipStatus = value;
                OnPropertyChanged("ipStatus");
            }
        }

        private string _macAddress;
        public string macAddress
        {
            get { return _macAddress; }
            set
            {
                _macAddress = value;
                OnPropertyChanged("macAddress");
            }
        }

        private string _location;
        public string location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged("location");
            }
        }

        private string _model;
        public string model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged("model");
            }
        }

        private String _ipAddress;
        public String ipAddress
        {
            get { return _ipAddress; }
            set
            {
                if (string.IsNullOrEmpty(value)) _ipAddress = "";
                else _ipAddress = value;
                IPAddress ip;
                if (IPAddress.TryParse(ipAddress, out ip))
                {
                    _IpAddress = ip;
                    _ipEndPoint = new IPEndPoint(ip, port);
                }
                else
                {
                    var ips = Dns.GetHostAddresses("");
                    if (ips == null) return;
                    _IpAddress = ips[0];
                    if (ip == null) return;
                    _ipEndPoint = new IPEndPoint(ip, port);
                }
                OnPropertyChanged("ipAddress");
            }
        }

        private int _port;
        public int port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged("port");
                _ipEndPoint = new IPEndPoint(IpAddress, port);


            }
        }

        private IPAddress _IpAddress;
        [XmlIgnore()]
        public IPAddress IpAddress
        {
            get { return _IpAddress; }
            set
            {
                _IpAddress = value;
                OnPropertyChanged("IpAddress");
                _ipEndPoint = new IPEndPoint(IpAddress, port);
            }
        }

        private IPEndPoint _ipEndPoint;
        [XmlIgnore()]
        public IPEndPoint ipEndPoint
        {
            get { return _ipEndPoint; }
            set
            {
                _ipEndPoint = value;
                OnPropertyChanged("ipEndPoint");
            }
        }

        private string _notes;
        public string notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged("notes");
            }
        }

        private Object _tag;
        [XmlIgnore]
        public Object tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                OnPropertyChanged("tag");
            }
        }

        private Socket _socket;
        [XmlIgnore()]
        public Socket socket
        {
            get
            {
                if (_socket == null) CreateSocket();
                return _socket;
            }
            set
            {
                _socket = value;
                OnPropertyChanged("socket");
            }
        }

        private bool _keepOpen;
        public bool keepOpen
        {
            get { return _keepOpen; }
            set
            {
                if (value != keepOpen)
                {
                    _keepOpen = value;
                    OnPropertyChanged("keepOpen");
                }
            }
        }

        private String _ReadDelimiter;
        public String ReadDelimiter
        {
            get { return _ReadDelimiter; }
            set
            {
                if (value != ReadDelimiter)
                {
                    _ReadDelimiter = value;
                    OnPropertyChanged("ReadDelimiter");
                }
            }
        }


        #region ISql

        private DateTime _lastUpdated;
        public DateTime lastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                OnPropertyChanged("lastUpdated");
            }
        }

        private System.String _key;
        public System.String key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("key");
            }
        }

        #endregion


        public string receiveBuffer { get; set; }
        //Used to buffer and recieve commands 
        #region DataReceived Event Arguments

        public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
        public virtual void OnRaiseDataReceivedEvent(Events.DataReceivedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior     
        {
            EventHandler<Events.DataReceivedEventArgs> handler = RaiseDataReceivedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.                   if (handler != null)     
            {
                handler(this, e);
            }

        }


        #endregion
       

        public Socket CreateSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = 1000;
            socket.ReceiveTimeout = 1000;
            return socket;
        }

        public Status.ConnectionStatus CheckConnectionStatus()
        {
            if (socket.Connected) connectionStatus = Status.ConnectionStatus.Connected;
            else connectionStatus = Status.ConnectionStatus.Disconnected;
            ReportConnectionStatus(connectionStatus);
            return connectionStatus;
        }

        private void ReportConnectionStatus(Status.ConnectionStatus status) { if (progressConnectionStatus != null) ((IProgress<Status.ConnectionStatus>)progressConnectionStatus).Report(status); }
        private void ReportSend(string s) { if (progressSend != null) ((IProgress<string>)progressSend).Report(s); }
        private void ReportReceive(string s) { if (progressReceive != null) ((IProgress<string>)progressReceive).Report(s); }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("macAddress", _macAddress, typeof(String));
            info.AddValue("location", _location, typeof(String));
            info.AddValue("model", _model, typeof(String));
            info.AddValue("ipAddress", _ipAddress, typeof(String));
            info.AddValue("port", _port, typeof(int));
            info.AddValue("keepOpen", _keepOpen, typeof(Boolean));
            #region ISql
            info.AddValue("lastUpdated", _lastUpdated, typeof(DateTime));
            info.AddValue("key", _key, typeof(string));
            #endregion
        }

        public TcpClientVM(SerializationInfo info, StreamingContext context)
        {
            macAddress = (String)info.GetValue("macAddress", typeof(String));
            location = (String)info.GetValue("location", typeof(String));
            model = (String)info.GetValue("model", typeof(String));
            ipAddress = (String)info.GetValue("ipAddress", typeof(String));
            port = (int)info.GetValue("port", typeof(int));
            // keepOpen = (Boolean)info.GetValue("keepOpen", typeof(Boolean));
            #region ISql
            //_lastUpdated = (DateTime)info.GetValue("lastUpdated", typeof(DateTime));
            //_key = (string)info.GetValue("key", typeof(string));
            #endregion
        }

        #region IProvideUserControl Members

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl uc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                uc = (UserControl)Activator.CreateInstance(type);
                if (uc == null) continue;

                uc.DataContext = this;
                lst.Add(uc);
            }
            var vvv = lst[0].DataContext;
            return lst.ToArray();
        }

        #endregion
    }

    public class TcpClientVMList : ObservableCollection<TcpClientVM>
    {

        public TcpClientVMList() : base() { }
        public TcpClientVMList(IEnumerable<TcpClientVM> lst)
        {
            this.Clear();
            AddRange(lst);
        }

        public void AddRange(IEnumerable<TcpClientVM> lst)
        {
            foreach (var v in lst) { Add(v); }
        }
        public void PingAll() { foreach (var v in this) { v.ipStatus = "-"; v.Ping(); } }
    }
}