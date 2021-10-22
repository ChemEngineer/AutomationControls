using AutomationControls.Attributes;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication.Pipes
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.Pipes.NamedPipeServerStreamDataControl))]
    public class NamedPipeServerStreamData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {

        public NamedPipeServerStreamData()
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



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsConnected", _IsConnected, typeof(Boolean));
            info.AddValue("IsAsync", _IsAsync, typeof(Boolean));
            info.AddValue("IsMessageComplete", _IsMessageComplete, typeof(Boolean));
            info.AddValue("TransmissionMode", _TransmissionMode, typeof(PipeTransmissionMode));
            info.AddValue("InBufferSize", _InBufferSize, typeof(Int32));
            info.AddValue("OutBufferSize", _OutBufferSize, typeof(Int32));
            info.AddValue("ReadMode", _ReadMode, typeof(PipeTransmissionMode));
            //info.AddValue("SafePipeHandle",_SafePipeHandle, typeof(SafePipeHandle));
            info.AddValue("CanRead", _CanRead, typeof(Boolean));
            info.AddValue("CanWrite", _CanWrite, typeof(Boolean));
            info.AddValue("CanSeek", _CanSeek, typeof(Boolean));
            info.AddValue("Length", _Length, typeof(Int64));
            info.AddValue("Position", _Position, typeof(Int64));
            info.AddValue("CanTimeout", _CanTimeout, typeof(Boolean));
            info.AddValue("ReadTimeout", _ReadTimeout, typeof(Int32));
            info.AddValue("WriteTimeout", _WriteTimeout, typeof(Int32));
        }
        public NamedPipeServerStreamData(SerializationInfo info, StreamingContext context)
        {
            _IsConnected = (Boolean)info.GetValue("IsConnected", typeof(Boolean));
            _IsAsync = (Boolean)info.GetValue("IsAsync", typeof(Boolean));
            _IsMessageComplete = (Boolean)info.GetValue("IsMessageComplete", typeof(Boolean));
            _TransmissionMode = (PipeTransmissionMode)info.GetValue("TransmissionMode", typeof(PipeTransmissionMode));
            _InBufferSize = (Int32)info.GetValue("InBufferSize", typeof(Int32));
            _OutBufferSize = (Int32)info.GetValue("OutBufferSize", typeof(Int32));
            _ReadMode = (PipeTransmissionMode)info.GetValue("ReadMode", typeof(PipeTransmissionMode));
            //_SafePipeHandle= (SafePipeHandle) info.GetValue("SafePipeHandle", typeof(SafePipeHandle));
            _CanRead = (Boolean)info.GetValue("CanRead", typeof(Boolean));
            _CanWrite = (Boolean)info.GetValue("CanWrite", typeof(Boolean));
            _CanSeek = (Boolean)info.GetValue("CanSeek", typeof(Boolean));
            _Length = (Int64)info.GetValue("Length", typeof(Int64));
            _Position = (Int64)info.GetValue("Position", typeof(Int64));
            _CanTimeout = (Boolean)info.GetValue("CanTimeout", typeof(Boolean));
            _ReadTimeout = (Int32)info.GetValue("ReadTimeout", typeof(Int32));
            _WriteTimeout = (Int32)info.GetValue("WriteTimeout", typeof(Int32));
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

    }

    [Serializable]
    public class NamedPipeServerStreamDataList : ObservableCollection<NamedPipeServerStreamData>
    {

        private static int numThreads = 4;
        List<Task> lst = new List<Task>();

        public async Task AddServer(string serverName, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
        {
            //NamedPipeServerStreamData data = new NamedPipeServerStreamData(); 
            NamedPipeServerStream pipeServer = new NamedPipeServerStream(serverName, PipeDirection.InOut, numThreads, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            await pipeServer.WaitForConnectionAsync(status, ct);
            var res = await pipeServer.GetStringAsync(status, ct);

            pipeServer.Close();
        }

        public Task MonitorPipes(string serverName)
        {
            return Task.Run(() =>
           {
               while (true)
               {
                   if (lst.Count < 4)
                   {
                       Task task = AddServer(serverName);
                       task.ContinueWith(x =>
                       {
                           lst.Remove(x);
                       });
                       lst.Add(task);
                   }
               }
           }, CancellationToken.None);
        }

    }
}

