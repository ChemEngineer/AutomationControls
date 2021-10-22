using AutomationControls.Extensions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication.TCPIP
{
    // Implementation
    //class TestServer : AutomationControls.Communication.TCPIP.AsyncTcpServer<Object>
    //{
    //    protected override void Initialize()
    //    {
    //        data.ipAddress = "192.168.1.136";
    //        data.port = 4000;
    //        base.Initialize();
    //    }

    //    public override void ProcessClient(TcpClient tcpClient)
    //    {
    //        if(tcpClient != null)
    //        {
    //            // stream = tcpClient.GetStream();
    //            Task.Factory.StartNew(async () =>
    //            {
    //                while(!cts.IsCancellationRequested)
    //                {
    //                    if(!stream.DataAvailable)
    //                    {
    //                        await Task.Delay(100, cts.Token);
    //                        continue;
    //                    }
    //                    else
    //                    {
    //                        using(StreamReader sr = new StreamReader(tcpClient.GetStream()))
    //                        {
    //                            string ret = await sr.ReadLineAsync();
    //                            var split = ret.Split('_');
    //                            string command = split.First();
    //                            if(command.Equals("1"))
    //                            {
    //                                //Logic
    //                            }
    //                            else if(command.Equals("2"))
    //                            {
    //                                //Logic
    //                            }
    //                        }
    //                        stream.Close();
    //                    }
    //                };
    //            });
    //        }
    //    }
    //}

    public abstract class AsyncTcpServer<T> : UserControl, AutomationControls.Communication.TCPIP.Interfaces.IAsyncTcpServer, IDisposable where T : new()
    {
        public Progress<string> progress = new Progress<string>();

        private string _serializePath;
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
        public string delimiter = "_~_~";
        public string[] split;

        #region "INotifyPropertyChanged Pattern"

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion


        private TcpServerData _data = new TcpServerData();
        public TcpServerData data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("data");
            }
        }

        public CancellationTokenSource cts = new CancellationTokenSource();
        protected static int BufferSize = 4096;
        protected NetworkStream stream;
        private TcpListener tcpServer;
        //  private TcpClient tcpClient;

        T t = new T();

        public AsyncTcpServer()
        {

            Initialize();
        }

        protected virtual void Initialize()
        {
            serializePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.GetType().Name);
            // DeserializeJSON(System.IO.Path.Combine(serializePath, t.GetType().Name + "json"));

            if (tcpServer == null)
                tcpServer = new TcpListener(IPAddress.Parse(data.ipAddress), data.port);

            tcpServer.Server.ReceiveTimeout = 10000;
            // Start();
        }

        public AsyncTcpServer(string ip, int port) { data.ipAddress = ip; data.port = port; Initialize(); }

        public bool Start()
        {
            try
            {
                tcpServer.Start();
                ServerRunning = true;
                ListenForClients(tcpServer);
                return true;
            }
            catch (Exception e)
            {
                ServerRunning = false;
                Console.WriteLine(e.ToString()); return false;
            }
        }

        public void Stop()
        {
            cts.Cancel();
            if (tcpServer != null) tcpServer.Stop();
            ServerRunning = false;
            SerializeJSON(System.IO.Path.Combine(serializePath, t.GetType().Name + "json"));
        }

        private async void ListenForClients(TcpListener tcpServer)
        {
            cts = new CancellationTokenSource();
            while (!cts.IsCancellationRequested)
            {
                TcpClient tcpClient = await tcpServer.AcceptTcpClientAsync(cts.Token);
                try
                {
                    bool processed = false;
                    bool connected = tcpClient.Connected;
                    while (!connected) { await Task.Delay(500); };
                    //clients.Add(tcpClient);
                    if (!processed)
                    {
                        processed = true;
                        ProcessClient(tcpClient);
                    }
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
                    tcpClient.ReceiveTimeout = 10000;
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
                                        split = ret.Split(new string[] { delimiter }, StringSplitOptions.None);
                                        command = split.First();
                                        Report("Command count: " + split.Count());
                                        if (command.Equals("1"))
                                        {
                                            sw.Write(Identify() + "\r\n");
                                            sw.Flush();
                                        }
                                        else if (command.Equals("2"))
                                        {
                                            payloadSize = int.Parse(split[1]);
                                            ProcessCommand(tcpClient);
                                            //if (IsDataOk())
                                            //    sw.Write("1" + "\r\n");
                                            //else
                                            //    sw.Write("0" + "\r\n");

                                            //sw.Flush();

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

        public virtual bool IsDataOk()
        {
            return true;
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

            private string _imagePath;
            public string imagePath { get { return _imagePath; } set { _imagePath = value; } }
        }

        #endregion

        public void Dispose() { Stop(); }

        public void SerializeJSON(string path)
        {
            AutomationControls.Serialization.Serializer<TcpServerData> ser = new Serialization.Serializer<TcpServerData>(data);
            ser.ToJSON(path);
        }

        public void DeserializeJSON(string path)
        {
            AutomationControls.Serialization.Serializer<TcpServerData> ser = new Serialization.Serializer<TcpServerData>(data);
            data = ser.FromJSON(path);
        }

        protected void Report(string s) { if (progress != null) ((IProgress<string>)progress).Report(s); }
    }
}
