using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Communication.TCPIP
{
    [Serializable]
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



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ipAddress", _ipAddress, typeof(string));
            info.AddValue("port", _port, typeof(int));
            info.AddValue("isConnected", _isConnected, typeof(bool));
        }
        public TcpServerData(SerializationInfo info, StreamingContext context)
        {
            _ipAddress = (string)info.GetValue("ipAddress", typeof(string));
            _port = (int)info.GetValue("port", typeof(int));
            _isConnected = (bool)info.GetValue("isConnected", typeof(bool));
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
