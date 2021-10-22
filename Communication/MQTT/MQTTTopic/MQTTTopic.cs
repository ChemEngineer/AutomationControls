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
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTTopicControl))]
    public class MQTTTopic : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public MQTTTopic()
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



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Topic", _Topic, typeof(string));
        }
        public MQTTTopic(SerializationInfo info, StreamingContext context)
        {
            _Topic = (string)info.GetValue("Topic", typeof(string));
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
            AutomationControls.Serialization.Serializer<MQTTTopic> ser = new AutomationControls.Serialization.Serializer<MQTTTopic>(this);
            ser.ToJSON(s);
        }

        public static MQTTTopic Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTopic> ser = new AutomationControls.Serialization.Serializer<MQTTTopic>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTTopicListControl))]
    public class MQTTTopicList : ObservableCollection<MQTTTopic>, ISerializable, IProvideUserControls
    {
        public MQTTTopicList() { }

        public MQTTTopicList(IEnumerable<MQTTTopic> lst) { lst.ForEach(x => this.Add(x)); }

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

        public MQTTTopicList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTopicList> ser = new AutomationControls.Serialization.Serializer<MQTTTopicList>(this);
            ser.ToJSON(s);
        }

        public static MQTTTopicList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTopicList> ser = new AutomationControls.Serialization.Serializer<MQTTTopicList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

