using AutomationControls.Attributes;
using AutomationControls.Communication;
using AutomationControls.Communication.Pipes;
using AutomationControls.Communication.Serial.DataClasses;
using AutomationControls.Communication.TCPIP;
using AutomationControls.Enums;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.BaseClasses
{
    [Serializable]
    [DataProfileAttribute(typeof(DeviceBaseControl))]
    public abstract class DeviceBase : INotifyPropertyChanged, IProvideUserControls, ISerializable, IProfileName
    {

        public DeviceBase()
        {
            this.bus = DeviceCommunications.None;
            Name = GetType().Name;
            this.PropertyChanged += (sender3, e3) =>
            {
                if (e3.PropertyName == "bus")
                {
                    switch (this.bus)
                    {
                        case AutomationControls.Enums.DeviceCommunications.None:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.RS232:
                            this.comm = new AutomationControls.Communication.SerialCommunication();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.USB:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.TCPIP:
                            this.comm = new AutomationControls.Communication.TCPIPCommunication();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.Pipe:
                            this.comm = new AutomationControls.Communication.PipeClientCommunicator();
                            break;
                        case DeviceCommunications.MQTTClient:
                            this.comm = new MQTTClientCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.Undefined:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        default:
                            break;
                    }
                }
            };
        }

        #region "INotifyPropertyChanged Pattern"

        //public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        private ICommunications _comm = new AutomationControls.Communication.NullCommunicator();
        public ICommunications comm
        {
            get { return _comm; }
            set
            {
                _comm = value;
                OnPropertyChanged("comm");
            }
        }

        private DeviceCommunications _bus = DeviceCommunications.RS232;
        public DeviceCommunications bus
        {
            get
            {
                return _bus;
            }
            set
            {
                if (value != bus)
                {
                    _bus = value;
                    switch (value)
                    {
                        case DeviceCommunications.None:
                            comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        case DeviceCommunications.RS232:
                            if (_comm is AutomationControls.Communication.SerialCommunication)
                                break;
                            else
                                comm = new AutomationControls.Communication.SerialCommunication();
                            break;
                        case DeviceCommunications.TCPIP:
                            if (_comm is AutomationControls.Communication.TCPIPCommunication)
                                break;
                            else
                                comm = new AutomationControls.Communication.TCPIPCommunication();
                            break;
                        case DeviceCommunications.Pipe:
                            if (_comm is AutomationControls.Communication.PipeClientCommunicator)
                                break;
                            else
                                comm = new AutomationControls.Communication.PipeClientCommunicator();
                            break;
                        case DeviceCommunications.USB:
                            comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        //case DeviceCommunications.MQTTBroker:
                        //    if (_comm is AutomationControls.Communication.MQTTBrokerCommunicator) 
                        //         break;
                        //    else
                        //        comm = new AutomationControls.Communication.MQTTBrokerCommunicator();
                        //    break;
                        case DeviceCommunications.Undefined:
                            comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        default:
                            comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                    }
                    OnPropertyChanged("bus");

                }
            }
        }

        private string _profileName;
        public string profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;

                OnPropertyChanged("profileName");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("keepOpen", _keepOpen, typeof(bool));
            info.AddValue("connectAtStartup", _connectAtStartup, typeof(bool));
            info.AddValue("Name", _Name, typeof(string));
            info.AddValue("profileName", _profileName, typeof(string));
            info.AddValue("bus", _bus, typeof(DeviceCommunications));

            var v = _comm.GetType().BaseType;
            if (_comm is SerialPortData)
                info.AddValue("spdata", _comm, typeof(AutomationControls.Communication.SerialCommunication));
            if (_comm is TcpClientVM)
                info.AddValue("tcpdata", _comm, typeof(AutomationControls.Communication.TCPIPCommunication));
            if (_comm is NamedPipeClientStreamData)
                info.AddValue("pipedata", _comm, typeof(AutomationControls.Communication.PipeClientCommunicator));
            //if (_comm is MQTTBrokerCommunicator)
            //    info.AddValue("mqttdata", _comm, typeof(AutomationControls.Communication.MQTTBrokerCommunicator));
            if (_comm is MQTTClientCommunicator)
                info.AddValue("mqttclientdata", _comm, typeof(AutomationControls.Communication.MQTTClientCommunicator));
        }

        public DeviceBase(SerializationInfo info, StreamingContext context)
        {

            _connectAtStartup = (bool)info.GetValue("connectAtStartup", typeof(bool));
            _keepOpen = (bool)info.GetValue("keepOpen", typeof(bool));

            Name = (string)info.GetValue("Name", typeof(string));
            _profileName = (string)info.GetValue("profileName", typeof(string));
            _bus = (DeviceCommunications)info.GetValue("bus", typeof(DeviceCommunications));

            if (_bus == DeviceCommunications.RS232)
                comm = (ICommunications)info.GetValue("spdata", typeof(SerialCommunication));
            else if (_bus == DeviceCommunications.TCPIP)
                comm = (ICommunications)info.GetValue("tcpdata", typeof(TCPIPCommunication));
            else if (_bus == DeviceCommunications.Pipe)
                comm = (ICommunications)info.GetValue("pipedata", typeof(PipeClientCommunicator));
            //else if (_bus == DeviceCommunications.MQTTBroker)
            //    comm = (ICommunications)info.GetValue("mqttdata", typeof(MQTTBrokerCommunicator));
            else if (_bus == DeviceCommunications.MQTTClient)
                comm = (ICommunications)info.GetValue("mqttclientdata", typeof(MQTTClientCommunicator));

            this.PropertyChanged += (sender3, e3) =>
            {
                if (e3.PropertyName == "bus")
                {
                    switch (this.bus)
                    {
                        case AutomationControls.Enums.DeviceCommunications.None:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.RS232:
                            this.comm = new AutomationControls.Communication.SerialCommunication();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.USB:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.TCPIP:
                            this.comm = new AutomationControls.Communication.TCPIPCommunication();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.Pipe:
                            this.comm = new AutomationControls.Communication.PipeClientCommunicator();
                            break;
                        //case AutomationControls.Enums.DeviceCommunications.MQTTBroker:
                        //    this.comm = new AutomationControls.Communication.MQTTBrokerCommunicator();
                        //    break;
                        case AutomationControls.Enums.DeviceCommunications.MQTTClient:
                            this.comm = new AutomationControls.Communication.MQTTClientCommunicator();
                            break;
                        case AutomationControls.Enums.DeviceCommunications.Undefined:
                            this.comm = new AutomationControls.Communication.NullCommunicator();
                            break;
                        default:
                            break;
                    }
                }
            };
        }


        public virtual UserControl[] GetUserControls()
        {
            db.lstUserControls.Clear();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), true);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                //Check to see if we already have the control created - Avoid binding errors due to recreation
                if (db.lstUserControls.Where(x => x.GetType().Name == type.Name).Count() == 0)
                {
                    cc = (UserControl)Activator.CreateInstance(type);
                    if (cc == null) continue;
                    db.lstUserControls.Add(cc);
                    cc.DataContext = this;
                }
            }

            //foreach(var pi in this.GetType().GetProperties())
            //{
            //    foreach(var v in pi.PropertyType.GetCustomAttributes(true))
            //    {
            //        DataProfileAttribute attr = v as DataProfileAttribute;
            //        if(attr != null)
            //        {
            //            var type = attr.ClassName;
            //            cc = (UserControl)Activator.CreateInstance(type);
            //            if(cc == null) continue;
            //            lst.Add(cc);
            //            cc.DataContext = pi.GetValue(this);
            //        }
            //    }
            //}
            return db.lstUserControls.ToArray();
        }

        private bool _connectAtStartup;
        public bool connectAtStartup
        {
            get { return _connectAtStartup; }
            set
            {
                _connectAtStartup = value;
                OnPropertyChanged("connectAtStartup");
            }
        }

        private bool _keepOpen;
        public bool keepOpen
        {
            get { return _keepOpen; }
            set
            {
                _keepOpen = value;
                OnPropertyChanged("keepOpen");
            }
        }

    }


    [Serializable]
    [DataProfile(typeof(AutomationControls.BaseClasses.DeviceBaseListControl))]
    public class DeviceBaseList : ObservableCollection<DeviceBase>, ISerializable, IProvideUserControls
    {
        public DeviceBaseList()
        {
        }

        public DeviceBaseList(IEnumerable<DeviceBase> lst) { lst.ForEach(x => this.Add(x)); }

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
            foreach (DeviceBase data in this)
            {
                var v = data.comm.GetType().BaseType;
                if (data.comm is SerialPortData)
                    info.AddValue("spdata", data.comm, typeof(AutomationControls.Communication.SerialCommunication));
                if (data.comm is TcpClientVM)
                    info.AddValue("tcpdata", data.comm, typeof(AutomationControls.Communication.TCPIPCommunication));
                if (data.comm is NamedPipeClientStreamData)
                    info.AddValue("pipedata", data.comm, typeof(AutomationControls.Communication.PipeClientCommunicator));

            }
            info.AddValue("lst", this, this.GetType());
        }

        public DeviceBaseList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<DeviceBaseList> ser = new AutomationControls.Serialization.Serializer<DeviceBaseList>(this);
            ser.ToJSON(s);
        }

        public static DeviceBaseList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<DeviceBaseList> ser = new AutomationControls.Serialization.Serializer<DeviceBaseList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}