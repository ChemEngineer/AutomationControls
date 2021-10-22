using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Servers
{
    [DataProfileAttribute(typeof(TcpServerControl))]
    public abstract class AsyncTcpServer<T> : TcpServerData, IAsyncTcpServer, IDisposable where T : new()
    {

        private string _serializePath = "Nope";
        public string serializePath
        {
            get { return _serializePath; }
            set
            {
                _serializePath = value;
                OnPropertyChanged("serializePath");
            }
        }

        public bool ServerRunning = true;
        public int payloadSize = 0;
        public string[] split;


        public CancellationTokenSource cts { get; set; }
        protected static int BufferSize = 4096;
        protected NetworkStream stream;
        private TcpListener tcpServer;
        public TcpClient tcpClient;

        T t = new T();

        public AsyncTcpServer()
        {
            Initialize();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "ipAddress" || e.PropertyName == "port")
                {
                    Initialize();
                }
            };
        }

        protected virtual void Initialize()
        {
            cts = new CancellationTokenSource();
            serializePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.GetType().Name);
            // DeserializeJSON(System.IO.Path.Combine(serializePath, t.GetType().Name + "json"));

            if (tcpServer == null && ipAddress != null && port != 0)
                tcpServer = new TcpListener(IPAddress.Parse(ipAddress), port);

            if (tcpServer != null)
                ServerRunning = true;
            else
                ServerRunning = false;


        }

        public bool Start()
        {
            try
            {
                if (tcpServer == null) Initialize();
                tcpServer.Start();
                isConnected = true;
                ListenForClients(tcpServer);
                return true;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return false; }
        }

        public void Stop()
        {
            // cts.Cancel();
            if (tcpServer != null) tcpServer.Stop();
            isConnected = false;
            SerializeJSON(System.IO.Path.Combine(serializePath, t.GetType().Name + "json"));
        }

        private async void ListenForClients(TcpListener tcpServer)
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    TcpClient tcpClient = await tcpServer.AcceptTcpClientAsync();
                    //clients.Add(tcpClient);
                    ProcessClient(tcpClient);
                }
                catch
                {
                }
            }
        }

        public virtual void ProcessClient(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.ReceiveTimeout = 2000;
                    tcpClient.SendTimeout = 2000;
                    stream = tcpClient.GetStream();
                    Task.Factory.StartNew(async () =>
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            if (!stream.DataAvailable)
                            {
                                await Task.Delay(100, cts.Token);
                                continue;
                            }
                            else
                            {
                                string command = "";
                                string ret = "";
                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    ret = await sr.ReadLineAsync();
                                    using (StreamWriter sw = new StreamWriter(stream))
                                    {
                                        split = ret.Split(new string[] { "_~_~" }, StringSplitOptions.RemoveEmptyEntries);
                                        command = split.First();

                                        if (command.Equals("1"))
                                        {
                                            string s = Identify() + "\r\n";
                                            sw.Write(s);
                                            sw.Flush();
                                            OnDataReadyEvent(new DataReadyEventArgs() { ipAddress = ipAddress, text = s });
                                        }
                                        else if (command.Equals("2"))
                                        {
                                            payloadSize = int.Parse(split[1]);
                                            ProcessCommand(tcpClient);
                                        }
                                    }
                                }
                            }
                        };
                    });
                }
            }
            catch (Exception ex)
            {

                string s = ex.ToString();
            }
        }

        #region DataReadyEventHandler

        public virtual string Identify() { return "AsyncTcpServerBase"; }
        public virtual void ProcessCommand(TcpClient tcpClient) { }

        public delegate void DataReadyEventHandler(object sender, DataReadyEventArgs a);
        public event EventHandler<DataReadyEventArgs> DataReadyEvent;
        protected virtual void OnDataReadyEvent(DataReadyEventArgs e)
        {
            EventHandler<DataReadyEventArgs> handler = DataReadyEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public class DataReadyEventArgs : EventArgs
        {
            public DataReadyEventArgs() { }
            private T _Message;
            public T Message { get { return _Message; } set { _Message = value; } }

            private string _ipAddress;
            public string ipAddress { get { return _ipAddress; } set { _ipAddress = value; } }

            private string _text;
            public string text { get { return _text; } set { _text = value; } }
        }

        #endregion

        public void Dispose() { Stop(); }

        public void SerializeJSON(string path)
        {
            Serialization.Serializer<TcpServerData> ser = new Serialization.Serializer<TcpServerData>(this);
            ser.ToJSON(path);
        }

        public void DeserializeJSON(string path)
        {
            Serialization.Serializer<TcpServerData> ser = new Serialization.Serializer<TcpServerData>(this);
            // this = ser.FromJSON(path);
        }
    }

    [Serializable]
    [DataProfileAttribute(typeof(TcpServerListControl))]
    public class AsyncTcpServerDataList<T> : ObservableCollection<IAsyncTcpServer>, ISerializable, IProvideUserControls where T : AsyncTcpServer<T>, new()
    {
        public AsyncTcpServerDataList() { }

        #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }
            return lst.ToArray();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lst", this, this.GetType());
        }

        public AsyncTcpServerDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
    [DataProfileAttribute(typeof(TcpServerControl))]
    public class TcpServerData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public TcpServerData()
        {
            Initialize();
        }

        private void Initialize() { }

        #region PropertyChanged Pattern

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion


        private string _ipAddress;
        public string ipAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
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
            }
        }

        private bool _isConnected;
        public bool isConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged("isConnected");
            }
        }

        private bool _openOnStart;
        public bool openOnStart
        {
            get { return _openOnStart; }
            set
            {
                _openOnStart = value;
                OnPropertyChanged("openOnStart");
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ipAddress", _ipAddress, typeof(string));
            info.AddValue("port", _port, typeof(int));
            info.AddValue("isConnected", _isConnected, typeof(bool));
            info.AddValue("openOnStart", _openOnStart, typeof(bool));
        }
        public TcpServerData(SerializationInfo info, StreamingContext context)
        {
            _ipAddress = (string)info.GetValue("ipAddress", typeof(string));
            _port = (int)info.GetValue("port", typeof(int));
            // _isConnected = (bool)info.GetValue("isConnected", typeof(bool));
            _openOnStart = (bool)info.GetValue("openOnStart", typeof(bool));
        }


        #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }
            return lst.ToArray();
        }

        #endregion

    }

    [Serializable]
    [DataProfileAttribute(typeof(TcpServerListControl))]
    public class TcpServerDataList : ObservableCollection<TcpServerData>, ISerializable, IProvideUserControls
    {
        public TcpServerDataList() { }

        #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }
            return lst.ToArray();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lst", this, this.GetType());
        }

        public TcpServerDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}