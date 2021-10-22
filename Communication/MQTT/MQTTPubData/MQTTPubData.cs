using AutomationControls.Attributes;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{



    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTPubDataControl))]
    public class MQTTPubData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public MQTTPubData()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (Topic == null)
                Topic = "Topic";
            if (Payload == null)
                Payload = "Payload";
        }

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


        private string _Topic;
        public string Topic
        {
            get { return _Topic; }
            set
            {
                _Topic = value;
                OnPropertyChanged("Topic");
            }
        }

        private string _Payload;
        public string Payload
        {
            get { return _Payload; }
            set
            {
                _Payload = value;
                OnPropertyChanged("Payload");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Topic", _Topic, typeof(string));
            info.AddValue("Payload", _Payload, typeof(string));
            Initialize();
        }
        public MQTTPubData(SerializationInfo info, StreamingContext context)
        {
            _Topic = (string)info.GetValue("Topic", typeof(string));
            _Payload = (string)info.GetValue("Payload", typeof(string));
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

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTPubData> ser = new AutomationControls.Serialization.Serializer<MQTTPubData>(this);
            ser.ToJSON(s);
        }

        public static MQTTPubData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTPubData> ser = new AutomationControls.Serialization.Serializer<MQTTPubData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTPubDataListControl))]
    public class MQTTPubDataList : ObservableCollection<MQTTPubData>, ISerializable, IProvideUserControls
    {
        public MQTTPubDataList() { }

        public MQTTPubDataList(IEnumerable<MQTTPubData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public MQTTPubDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTPubDataList> ser = new AutomationControls.Serialization.Serializer<MQTTPubDataList>(this);
            ser.ToJSON(s);
        }

        public static MQTTPubDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTPubDataList> ser = new AutomationControls.Serialization.Serializer<MQTTPubDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

