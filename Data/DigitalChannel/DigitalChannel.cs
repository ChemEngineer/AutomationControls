using AutomationControls.Attributes;
using AutomationControls.Enums;
using AutomationControls.EventArguments;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Controllers.DataClasses
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Controllers.DataClasses.DigitalChannelControl))]
    public class DigitalChannel : INotifyPropertyChanged, ISerializable, IProvideUserControls, IDigitalChannel
    {
        public DigitalChannel()
        {
            Initialize();
            var type = this.GetType();
            DeviceName = this.GetType().Name;
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



        private string _DeviceName;
        public string DeviceName
        {
            get { return _DeviceName; }
            set
            {
                if (_DeviceName != value)
                {
                    _DeviceName = value;
                    OnPropertyChanged("DeviceName");
                }
            }
        }


        private SensorTypes _SensorType;
        public SensorTypes SensorType
        {
            get { return _SensorType; }
            set
            {
                if (_SensorType != value)
                {
                    _SensorType = value;
                    OnPropertyChanged("SensorType");
                }
            }
        }

        private string _PinDesignation;
        public string PinDesignation
        {
            get { return _PinDesignation; }
            set
            {
                _PinDesignation = value;
                OnPropertyChanged("PinDesignation");
            }
        }

        private DigitalState _State;
        public DigitalState State
        {
            get { return _State; }
            set
            {
                _State = value;
                OnPropertyChanged("State");
            }
        }

        private CommunicationDirection _Direction;
        public CommunicationDirection Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value;
                OnPropertyChanged("Direction");
            }
        }

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        public EventHandler<DigitalStateChangedEventArgs> RaiseDigitalStateChangedEvent { get; set; }
        protected virtual void OnRaiseDigitalStateChangedEvent(DigitalStateChangedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior
        {
            EventHandler<DigitalStateChangedEventArgs> handler = RaiseDigitalStateChangedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            if (handler != null) { handler(this, e); }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PinDesignation", _PinDesignation, typeof(string));
            info.AddValue("State", _State, typeof(DigitalState));
            info.AddValue("Direction", _Direction, typeof(CommunicationDirection));
            info.AddValue("Value", _Value, typeof(string));
            info.AddValue("DeviceName", _DeviceName, typeof(string));
            info.AddValue("SensorType", _SensorType, typeof(SensorTypes));
        }
        public DigitalChannel(SerializationInfo info, StreamingContext context)
        {
            PinDesignation = (string)info.GetValue("PinDesignation", typeof(string));
            State = (DigitalState)info.GetValue("State", typeof(DigitalState));
            Direction = (CommunicationDirection)info.GetValue("Direction", typeof(CommunicationDirection));
            Value = (string)info.GetValue("Value", typeof(string));
            DeviceName = (string)info.GetValue("DeviceName", typeof(string));
            SensorType = (SensorTypes)info.GetValue("SensorType", typeof(SensorTypes));
        }


        #region IProvideUserControls
        List<UserControl> lst = new List<UserControl>();
        public UserControl[] GetUserControls()
        {
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;


                if (lst.Where(x => x.GetType() == type).Count() == 0)
                {
                    cc = (UserControl)Activator.CreateInstance(type);
                    if (cc == null) continue;
                    lst.Add(cc);
                    cc.DataContext = this;
                }
            }
            return lst.ToArray();
        }

        #endregion
    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Controllers.DataClasses.DigitalChannelListControl))]
    public class DigitalChannelList : ObservableCollection<DigitalChannel>, ISerializable, IProvideUserControls, IProfileName
    {
        public DigitalChannelList() { }

        #region "INotifyPropertyChanged Pattern"

        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        private ObservableCollection<string> _digitalChannelNames = new ObservableCollection<string>();
        public ObservableCollection<string> digitalChannelNames
        {
            get { return _digitalChannelNames; }
            set
            {
                _digitalChannelNames = value;
                OnPropertyChanged("digitalChannelNames");
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


        private string _selectedDevice = "";
        public string selectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("selectedDevice");
            }
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

                if (lst.Where(x => x.GetType() == type).Count() == 0)
                {
                    cc = (UserControl)Activator.CreateInstance(type);
                    if (cc == null) continue;
                    lst.Add(cc);
                    cc.DataContext = this;
                }
            }
            return lst.ToArray();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("profileName", _profileName, typeof(string));
            info.AddValue("selectedDevice", _selectedDevice, typeof(string));
            // info.AddValue("digitalChannelNames", _digitalChannelNames, typeof(ObservableCollection<string>));
        }

        public DigitalChannelList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _profileName = (string)info.GetValue("profileName", typeof(string));
            _selectedDevice = (string)info.GetValue("selectedDevice", typeof(string));
            //  _digitalChannelNames = (ObservableCollection<string>)info.GetValue("digitalChannelNames", typeof(ObservableCollection<string>));
        }

        #endregion


    }
}

