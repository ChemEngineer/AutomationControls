using AutomationControls.Attributes;
using AutomationControls.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Communication.Serial.DataClasses
{
    [Serializable]
    //[DataProfile(typeof(AutomationControls.Communication.Serial.UserControls.SerialPortReceiveControl))]
    [DataProfile(typeof(AutomationControls.Communication.Serial.UserControls.SerialPortControl))]
    public class SerialPortData : ISerializable, INotifyPropertyChanged
    {
        public SerialPortData()
        {
            Initialize();
        }

        private void Initialize()
        {
            lstPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            lstBaud = new ObservableCollection<int>(new[] { 100, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000, 0 });
            lstDataBits = new ObservableCollection<int>(new[] { 5, 6, 7, 8 });
            lstEncodings = new ObservableCollection<string>(System.Text.Encoding.GetEncodings().Select(x => x.Name).OrderBy(x => x));

            if (PortName.IsNull()) PortName = "COM1";
            if (DataBits < 5 || DataBits > 8) DataBits = 8;
            if (BaudRate <= 0) BaudRate = 115200;
            StopBits = System.IO.Ports.StopBits.One;
        }

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



        #region Properties

        private SerialPort _sp = new SerialPort();
        public SerialPort sp
        {
            get { return _sp; }
            set
            {
                _sp = value;
                OnPropertyChanged("sp");
            }
        }

        private string _PortName;
        public string PortName
        {
            get { return _PortName; }
            set
            {
                _PortName = value;
                _sp.PortName = value;
                OnPropertyChanged("PortName");
            }
        }

        private Int32 _BaudRate;
        public Int32 BaudRate
        {
            get { return _BaudRate; }
            set
            {
                _BaudRate = value;
                _sp.BaudRate = value;
                OnPropertyChanged("BaudRate");
            }
        }

        private ObservableCollection<string> _lstPorts;
        public ObservableCollection<string> lstPorts
        {
            get { return new ObservableCollection<string>(SerialPort.GetPortNames().OrderBy(x => x)); }
            set
            {
                _lstPorts = value;
                OnPropertyChanged("lstPorts");
            }
        }

        private ObservableCollection<int> _lstBaud;
        public ObservableCollection<int> lstBaud
        {
            get { return _lstBaud; }
            set
            {
                _lstBaud = value;
                OnPropertyChanged("lstBaud");
            }
        }

        private ObservableCollection<int> _lstDataBits;
        public ObservableCollection<int> lstDataBits
        {
            get { return _lstDataBits; }
            set
            {
                _lstDataBits = value;
                OnPropertyChanged("lstDataBits");
            }
        }

        private ObservableCollection<string> _lstEncodings;
        public ObservableCollection<string> lstEncodings
        {
            get { return _lstEncodings; }
            set
            {
                _lstEncodings = value;
                OnPropertyChanged("lstEncodings");
            }
        }

        private Boolean _BreakState;
        public Boolean BreakState
        {
            get { return _BreakState; }
            set
            {
                if (value != _BreakState)
                {
                    _BreakState = value;
                    _sp.BreakState = value;
                    OnPropertyChanged("BreakState");
                }
            }
        }

        #region Read only

        private Int32 _BytesToWrite;
        public Int32 BytesToWrite
        {
            get
            {
                return _BytesToWrite;
            }
            set
            {
                _BytesToWrite = value;
                OnPropertyChanged("BytesToWrite");
            }
        }

        private Int32 _BytesToRead;
        public Int32 BytesToRead
        {
            get
            {
                return _BytesToRead;
            }
            set
            {
                _BytesToRead = value;
                OnPropertyChanged("BytesToRead");
            }
        }

        private Boolean _CDHolding;
        public Boolean CDHolding
        {
            get
            {
                return _CDHolding;
            }
            set
            {
                _CDHolding = value;
                OnPropertyChanged("CDHolding");
            }
        }

        private Boolean _CtsHolding;
        public Boolean CtsHolding
        {
            get
            {
                return _CtsHolding;
            }
            set
            {
                _CtsHolding = value;
                OnPropertyChanged("CtsHolding");
            }
        }

        private Boolean _DsrHolding;
        public Boolean DsrHolding
        {
            get
            {
                return _DsrHolding;
            }
            set
            {
                _DsrHolding = value;
                OnPropertyChanged("DsrHolding");
            }
        }

        #endregion

        private Int32 _DataBits;
        public Int32 DataBits
        {
            get { return _DataBits; }
            set
            {
                if (value != _DataBits)
                {
                    _DataBits = value;
                    _sp.DataBits = value;
                    OnPropertyChanged("DataBits");
                }
            }
        }

        private Boolean _DiscardNull;
        public Boolean DiscardNull
        {
            get { return _DiscardNull; }
            set
            {
                if (value != _DiscardNull)
                {
                    _DiscardNull = value;
                    _sp.DiscardNull = value;
                    OnPropertyChanged("DiscardNull");
                }
            }
        }

        private Boolean _DtrEnable;
        public Boolean DtrEnable
        {
            get { return _DtrEnable; }
            set
            {
                if (value != _DtrEnable)
                {
                    _DtrEnable = value;
                    _sp.DtrEnable = value;
                    OnPropertyChanged("DtrEnable");
                }
            }
        }

        private String _Encoding;
        public String Encoding
        {
            get { return _Encoding; }
            set
            {
                if (value.IsNull()) return;
                var encoding = System.Text.Encoding.GetEncoding(value);
                if (encoding == null) return;
                _sp.Encoding = encoding;
                _Encoding = value;
                OnPropertyChanged("Encoding");
            }
        }

        private Handshake _Handshake;
        public Handshake Handshake
        {
            get { return _Handshake; }
            set
            {
                if (value != _Handshake)
                {
                    _Handshake = value;
                    _sp.Handshake = value;
                    OnPropertyChanged("Handshake");
                }
            }
        }

        #region Readonly

        private Boolean _IsOpen;
        public Boolean IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                _IsOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }

        #endregion

        private String _NewLine;
        public String NewLine
        {
            get { return _NewLine; }
            set
            {
                if (value != NewLine)
                {
                    _NewLine = value;
                    _sp.NewLine = value;
                    OnPropertyChanged("NewLine");
                }
            }
        }

        private Parity _Parity;
        public Parity Parity
        {
            get { return _Parity; }
            set
            {
                if (value != _Parity)
                {
                    _Parity = value;
                    _sp.Parity = value;
                    OnPropertyChanged("Parity");
                }
            }
        }

        private Byte _ParityReplace;
        public Byte ParityReplace
        {
            get { return _ParityReplace; }
            set
            {
                if (value != _ParityReplace)
                {
                    _ParityReplace = value;
                    _sp.ParityReplace = value;
                    OnPropertyChanged("ParityReplace");
                }
            }
        }

        private Int32 _ReadBufferSize;
        public Int32 ReadBufferSize
        {
            get { return _ReadBufferSize; }
            set
            {
                if (value != _ReadBufferSize)
                {
                    _ReadBufferSize = value;
                    _sp.ReadBufferSize = value;
                    OnPropertyChanged("ReadBufferSize");
                }
            }
        }

        private Int32 _ReadTimeout;
        public Int32 ReadTimeout
        {
            get { return _ReadTimeout; }
            set
            {
                if (value != _ReadTimeout)
                {
                    _ReadTimeout = value;
                    _sp.ReadTimeout = value;
                    OnPropertyChanged("ReadTimeout");
                }
            }
        }

        private Int32 _ReceivedBytesThreshold;
        public Int32 ReceivedBytesThreshold
        {
            get { return _ReceivedBytesThreshold; }
            set
            {
                if (value != _ReceivedBytesThreshold)
                {
                    _ReceivedBytesThreshold = value;
                    _sp.ReceivedBytesThreshold = value;
                    OnPropertyChanged("ReceivedBytesThreshold");
                }
            }
        }

        private Boolean _RtsEnable;
        public Boolean RtsEnable
        {
            get { return _RtsEnable; }
            set
            {
                if (value != _RtsEnable)
                {
                    _RtsEnable = value;
                    _sp.RtsEnable = value;
                    OnPropertyChanged("RtsEnable");
                }
            }
        }

        private StopBits _StopBits;
        public StopBits StopBits
        {
            get { return _StopBits; }
            set
            {
                if (value != StopBits)
                {
                    _StopBits = value;
                    _sp.StopBits = value;
                    OnPropertyChanged("StopBits");
                }
            }
        }

        private Int32 _WriteBufferSize;
        public Int32 WriteBufferSize
        {
            get { return _WriteBufferSize; }
            set
            {
                if (value != _WriteBufferSize)
                {
                    _WriteBufferSize = value;
                    _sp.WriteBufferSize = value;
                    OnPropertyChanged("WriteBufferSize");
                }
            }
        }

        private Int32 _WriteTimeout;
        public Int32 WriteTimeout
        {
            get { return _WriteTimeout; }
            set
            {
                if (value != _WriteTimeout)
                {
                    _WriteTimeout = value;
                    _sp.WriteTimeout = value;
                    OnPropertyChanged("WriteTimeout");
                }
            }
        }

        private String _LastResponse;
        public String LastResponse
        {
            get { return _LastResponse; }
            set
            {
                if (value != LastResponse)
                {
                    _LastResponse = value;
                    OnPropertyChanged("LastResponse");
                }
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

        //Used to buffer and recieve commands 
        #region DataReceived Event Arguments

        public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
        public virtual void OnRaiseDataReceivedEvent(Events.DataReceivedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior     
        {
            EventHandler<Events.DataReceivedEventArgs> handler = RaiseDataReceivedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.                   if (handler != null)     
            {
                if (handler != null)
                    handler(this, e);
            }

        }

        private string _ReadDelimiter;
        public string ReadDelimiter
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

        private bool _IsMonitoring;
        // Monitoring input buffer for delimiter
        public bool IsMonitoring
        {
            get { return _IsMonitoring; }
            set
            {
                if (value != _IsMonitoring)
                {
                    _IsMonitoring = value;
                    OnPropertyChanged("IsMonitoring");
                }
            }
        }

        #endregion

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("port", _port, typeof(string));   
            //info.AddValue("baud", _baud, typeof(string));
            //info.AddValue("lstPorts", lstPorts, typeof(ObservableCollection<string>));
            //info.AddValue("lstBaud", lstBaud, typeof(ObservableCollection<int>));
            //info.AddValue("status", status, typeof(string));
            //info.AddValue("sp", sp, typeof(SerialPort));
            info.AddValue("BaudRate", _BaudRate, typeof(Int32));
            info.AddValue("BreakState", _BreakState, typeof(Boolean));
            info.AddValue("BytesToWrite", _BytesToWrite, typeof(Int32));
            info.AddValue("BytesToRead", _BytesToRead, typeof(Int32));
            info.AddValue("CDHolding", _CDHolding, typeof(Boolean));
            info.AddValue("CtsHolding", _CtsHolding, typeof(Boolean));
            info.AddValue("DataBits", _DataBits, typeof(Int32));
            info.AddValue("DiscardNull", _DiscardNull, typeof(Boolean));
            info.AddValue("DsrHolding", _DsrHolding, typeof(Boolean));
            info.AddValue("DtrEnable", _DtrEnable, typeof(Boolean));
            info.AddValue("Encoding", _Encoding, typeof(String));
            info.AddValue("Handshake", _Handshake, typeof(Handshake));
            info.AddValue("IsOpen", _IsOpen, typeof(Boolean));
            info.AddValue("NewLine", _NewLine, typeof(String));
            info.AddValue("Parity", _Parity, typeof(Parity));
            info.AddValue("ParityReplace", _ParityReplace, typeof(Byte));
            info.AddValue("PortName", _PortName, typeof(String));
            info.AddValue("ReadBufferSize", _ReadBufferSize, typeof(Int32));
            info.AddValue("ReadTimeout", _ReadTimeout, typeof(Int32));
            info.AddValue("ReceivedBytesThreshold", _ReceivedBytesThreshold, typeof(Int32));
            info.AddValue("RtsEnable", _RtsEnable, typeof(Boolean));
            info.AddValue("StopBits", _StopBits, typeof(StopBits));
            info.AddValue("WriteBufferSize", _WriteBufferSize, typeof(Int32));
            info.AddValue("WriteTimeout", _WriteTimeout, typeof(Int32));
            info.AddValue("ReadDelimiter", _ReadDelimiter, typeof(string));
            info.AddValue("IsMonitoring", _IsMonitoring, typeof(Boolean));
            info.AddValue("keepOpen", _keepOpen, typeof(Boolean));
        }

        public SerialPortData(SerializationInfo info, StreamingContext context)
        {
            //_port = (string)info.GetValue("port", typeof(string));
            //baud = (string)info.GetValue("baud", typeof(string));
            //lstPorts = (ObservableCollection<string>)info.GetValue("lstPorts", typeof(ObservableCollection<string>));
            //lstBaud = (ObservableCollection<int>)info.GetValue("lstBaud", typeof(ObservableCollection<int>));
            //status = (string)info.GetValue("status", typeof(string));
            //sp = (SerialPort)info.GetValue("sp", typeof(SerialPort));

            BaudRate = (Int32)info.GetValue("BaudRate", typeof(Int32));
            BreakState = (Boolean)info.GetValue("BreakState", typeof(Boolean));
            BytesToWrite = (Int32)info.GetValue("BytesToWrite", typeof(Int32));
            BytesToRead = (Int32)info.GetValue("BytesToRead", typeof(Int32));
            CDHolding = (Boolean)info.GetValue("CDHolding", typeof(Boolean));
            CtsHolding = (Boolean)info.GetValue("CtsHolding", typeof(Boolean));
            DataBits = (Int32)info.GetValue("DataBits", typeof(Int32));
            DiscardNull = (Boolean)info.GetValue("DiscardNull", typeof(Boolean));
            DsrHolding = (Boolean)info.GetValue("DsrHolding", typeof(Boolean));
            DtrEnable = (Boolean)info.GetValue("DtrEnable", typeof(Boolean));
            Encoding = (String)info.GetValue("Encoding", typeof(String));
            Handshake = (Handshake)info.GetValue("Handshake", typeof(Handshake));
            IsOpen = (Boolean)info.GetValue("IsOpen", typeof(Boolean));
            NewLine = (String)info.GetValue("NewLine", typeof(String));
            Parity = (Parity)info.GetValue("Parity", typeof(Parity));
            ParityReplace = (Byte)info.GetValue("ParityReplace", typeof(Byte));
            PortName = (String)info.GetValue("PortName", typeof(String));
            ReadBufferSize = (Int32)info.GetValue("ReadBufferSize", typeof(Int32));
            ReadTimeout = (Int32)info.GetValue("ReadTimeout", typeof(Int32));
            ReceivedBytesThreshold = (Int32)info.GetValue("ReceivedBytesThreshold", typeof(Int32));
            RtsEnable = (Boolean)info.GetValue("RtsEnable", typeof(Boolean));
            StopBits = (StopBits)info.GetValue("StopBits", typeof(StopBits));
            WriteBufferSize = (Int32)info.GetValue("WriteBufferSize", typeof(Int32));
            WriteTimeout = (Int32)info.GetValue("WriteTimeout", typeof(Int32));
            ReadDelimiter = (string)info.GetValue("ReadDelimiter", typeof(string));
            keepOpen = (Boolean)info.GetValue("keepOpen", typeof(Boolean));
            // IsMonitoring = (Boolean)info.GetValue("IsMonitoring", typeof(Boolean));

            Initialize();
        }

        #region Extension Wrappers

        public CancellationTokenSource cts = new CancellationTokenSource();

        public Progress<String> progressSend = new Progress<string>();
        public Progress<String> progressReceive = new Progress<string>();

        public async Task SendAsync(String text)
        {
            if (!_sp.IsOpen) return;
            cts.Cancel(); cts = new CancellationTokenSource();
            await sp.WriteAsync(text, progressSend, cts.Token);
        }

        public async Task SendLineAsync(string s)
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            await _sp.SendLineAsync(s, "\n", progressSend);
        }

        public async Task CloseAsync()
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            await _sp.CloseAsync(progressReceive, cts.Token);
        }

        public async Task OpenAsync()
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            await _sp.OpenAsync(progressSend, cts.Token);

        }

        #region async recieving

        private static CancellationTokenSource ctsReceiveMonitor = new CancellationTokenSource();
        public void StartReceiveMonitor()
        {
            _sp.ReceiveMonitor(progressReceive, ctsReceiveMonitor.Token);
        }

        public void StopReceiveMonitor()
        {
            ctsReceiveMonitor.Cancel(); ctsReceiveMonitor = new CancellationTokenSource();
        }

        #endregion

        #endregion

        public void RefreshSerialPorts()
        {
            var v = SerialPort.GetPortNames().Distinct().OrderBy(x => x);
            lstPorts = new ObservableCollection<string>(v);
        }
    }

    [Serializable]
    public class SerialPortDataList : ObservableCollection<SerialPortData>
    {

    }
}
