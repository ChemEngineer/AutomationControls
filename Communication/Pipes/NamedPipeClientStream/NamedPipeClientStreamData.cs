using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Controls;

namespace AutomationControls.Communication.Pipes
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.Pipes.NamedPipeClientStreamDataControl))]
    public class NamedPipeClientStreamData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public NamedPipeClientStreamData()
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

        private NamedPipeClientStream _pipeServer;
        public NamedPipeClientStream pipeServer
        {
            get { return _pipeServer; }
            set { _pipeServer = value; }
        }

        private string _ServerName = ".";
        public string ServerName
        {
            get { return _ServerName; }
            set
            {
                _ServerName = value;
                OnPropertyChanged("ServerName");
            }
        }

        private string _PipeName;
        public string PipeName
        {
            get { return _PipeName; }
            set
            {
                _PipeName = value;
                OnPropertyChanged("PipeName");
            }
        }

        #region Wrapper Properties

        private Int32 _NumberOfServerInstances;
        public Int32 NumberOfServerInstances
        {
            get { return _NumberOfServerInstances; }
            set
            {
                _NumberOfServerInstances = value;
                OnPropertyChanged("NumberOfServerInstances");
            }
        }

        private Boolean _IsConnected;
        public Boolean IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        private Boolean _IsAsync;
        public Boolean IsAsync
        {
            get { return _IsAsync; }
            set
            {
                _IsAsync = value;
                OnPropertyChanged("IsAsync");
            }
        }

        private Boolean _IsMessageComplete;
        public Boolean IsMessageComplete
        {
            get { return _IsMessageComplete; }
            set
            {
                _IsMessageComplete = value;
                OnPropertyChanged("IsMessageComplete");
            }
        }

        private PipeTransmissionMode _TransmissionMode;
        public PipeTransmissionMode TransmissionMode
        {
            get { return _TransmissionMode; }
            set
            {
                _TransmissionMode = value;
                OnPropertyChanged("TransmissionMode");
            }
        }

        private Int32 _InBufferSize;
        public Int32 InBufferSize
        {
            get { return _InBufferSize; }
            set
            {
                _InBufferSize = value;
                OnPropertyChanged("InBufferSize");
            }
        }

        private Int32 _OutBufferSize;
        public Int32 OutBufferSize
        {
            get { return _OutBufferSize; }
            set
            {
                _OutBufferSize = value;
                OnPropertyChanged("OutBufferSize");
            }
        }

        private PipeTransmissionMode _ReadMode;
        public PipeTransmissionMode ReadMode
        {
            get { return _ReadMode; }
            set
            {
                _ReadMode = value;
                OnPropertyChanged("ReadMode");
            }
        }

        private SafePipeHandle _SafePipeHandle;
        public SafePipeHandle SafePipeHandle
        {
            get { return _SafePipeHandle; }
            set
            {
                _SafePipeHandle = value;
                OnPropertyChanged("SafePipeHandle");
            }
        }

        private Boolean _CanRead;
        public Boolean CanRead
        {
            get { return _CanRead; }
            set
            {
                _CanRead = value;
                OnPropertyChanged("CanRead");
            }
        }

        private Boolean _CanWrite;
        public Boolean CanWrite
        {
            get { return _CanWrite; }
            set
            {
                _CanWrite = value;
                OnPropertyChanged("CanWrite");
            }
        }

        private Boolean _CanSeek;
        public Boolean CanSeek
        {
            get { return _CanSeek; }
            set
            {
                _CanSeek = value;
                OnPropertyChanged("CanSeek");
            }
        }

        private Int64 _Length;
        public Int64 Length
        {
            get { return _Length; }
            set
            {
                _Length = value;
                OnPropertyChanged("Length");
            }
        }

        private Int64 _Position;
        public Int64 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged("Position");
            }
        }

        private Boolean _CanTimeout;
        public Boolean CanTimeout
        {
            get { return _CanTimeout; }
            set
            {
                _CanTimeout = value;
                OnPropertyChanged("CanTimeout");
            }
        }

        private Int32 _ReadTimeout;
        public Int32 ReadTimeout
        {
            get { return _ReadTimeout; }
            set
            {
                _ReadTimeout = value;
                OnPropertyChanged("ReadTimeout");
            }
        }

        private Int32 _WriteTimeout;
        public Int32 WriteTimeout
        {
            get { return _WriteTimeout; }
            set
            {
                _WriteTimeout = value;
                OnPropertyChanged("WriteTimeout");
            }
        }

        #endregion

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

        #endregion


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("NumberOfServerInstances", _NumberOfServerInstances, typeof(Int32));
            info.AddValue("IsConnected", _IsConnected, typeof(Boolean));
            info.AddValue("IsAsync", _IsAsync, typeof(Boolean));
            info.AddValue("IsMessageComplete", _IsMessageComplete, typeof(Boolean));
            info.AddValue("TransmissionMode", _TransmissionMode, typeof(PipeTransmissionMode));
            info.AddValue("InBufferSize", _InBufferSize, typeof(Int32));
            info.AddValue("OutBufferSize", _OutBufferSize, typeof(Int32));
            info.AddValue("ReadMode", _ReadMode, typeof(PipeTransmissionMode));
            info.AddValue("SafePipeHandle", _SafePipeHandle, typeof(SafePipeHandle));
            info.AddValue("CanRead", _CanRead, typeof(Boolean));
            info.AddValue("CanWrite", _CanWrite, typeof(Boolean));
            info.AddValue("CanSeek", _CanSeek, typeof(Boolean));
            info.AddValue("Length", _Length, typeof(Int64));
            info.AddValue("Position", _Position, typeof(Int64));
            info.AddValue("CanTimeout", _CanTimeout, typeof(Boolean));
            info.AddValue("ReadTimeout", _ReadTimeout, typeof(Int32));
            info.AddValue("WriteTimeout", _WriteTimeout, typeof(Int32));
            info.AddValue("PipeName", _PipeName, typeof(string));
            info.AddValue("ServerName", _ServerName, typeof(string));
        }

        public NamedPipeClientStreamData(SerializationInfo info, StreamingContext context)
        {
            _NumberOfServerInstances = (Int32)info.GetValue("NumberOfServerInstances", typeof(Int32));
            _IsConnected = (Boolean)info.GetValue("IsConnected", typeof(Boolean));
            _IsAsync = (Boolean)info.GetValue("IsAsync", typeof(Boolean));
            _IsMessageComplete = (Boolean)info.GetValue("IsMessageComplete", typeof(Boolean));
            _TransmissionMode = (PipeTransmissionMode)info.GetValue("TransmissionMode", typeof(PipeTransmissionMode));
            _InBufferSize = (Int32)info.GetValue("InBufferSize", typeof(Int32));
            _OutBufferSize = (Int32)info.GetValue("OutBufferSize", typeof(Int32));
            _ReadMode = (PipeTransmissionMode)info.GetValue("ReadMode", typeof(PipeTransmissionMode));
            _SafePipeHandle = (SafePipeHandle)info.GetValue("SafePipeHandle", typeof(SafePipeHandle));
            _CanRead = (Boolean)info.GetValue("CanRead", typeof(Boolean));
            _CanWrite = (Boolean)info.GetValue("CanWrite", typeof(Boolean));
            _CanSeek = (Boolean)info.GetValue("CanSeek", typeof(Boolean));
            _Length = (Int64)info.GetValue("Length", typeof(Int64));
            _Position = (Int64)info.GetValue("Position", typeof(Int64));
            _CanTimeout = (Boolean)info.GetValue("CanTimeout", typeof(Boolean));
            _ReadTimeout = (Int32)info.GetValue("ReadTimeout", typeof(Int32));
            _WriteTimeout = (Int32)info.GetValue("WriteTimeout", typeof(Int32));
            _PipeName = (string)info.GetValue("PipeName", typeof(string));
            _ServerName = (string)info.GetValue("ServerName", typeof(string));
        }


        #region IProvideUserControl

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

        #region functions

        public void Connect(String pipeName = null)
        {
            if (pipeName != null) PipeName = pipeName;
            pipeServer = new System.IO.Pipes.NamedPipeClientStream(ServerName, PipeName, System.IO.Pipes.PipeDirection.InOut, System.IO.Pipes.PipeOptions.Asynchronous);
            pipeServer.Connect();
            CanRead = pipeServer.CanRead;
            CanSeek = pipeServer.CanSeek;
            CanTimeout = pipeServer.CanTimeout;
            CanWrite = pipeServer.CanWrite;
            InBufferSize = pipeServer.InBufferSize;
            IsAsync = pipeServer.IsAsync;
            IsConnected = pipeServer.IsConnected;
            //IsMessageComplete = pipeServer.IsMessageComplete;
            //Length = pipeServer.Length;
            NumberOfServerInstances = pipeServer.NumberOfServerInstances;
            OutBufferSize = pipeServer.OutBufferSize;
            // Position = pipeServer.Position;
            ReadMode = pipeServer.ReadMode;
            //ReadTimeout = pipeServer.ReadTimeout;
            TransmissionMode = pipeServer.TransmissionMode;
            //WriteTimeout = pipeServer.WriteTimeout;
            // Length = pipeServer.Length;
            InBufferSize = pipeServer.InBufferSize;
        }

        public void Send(String s)
        {
            if (pipeServer.IsConnected)
            {
                byte[] b = Encoding.ASCII.GetBytes(s);
                pipeServer.Write(b, 0, b.Length);
            }
        }

        #endregion

    }

    [Serializable]
    public class NamedPipeClientStreamDataList : ObservableCollection<NamedPipeClientStreamData>
    {

    }
}