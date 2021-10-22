using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientControl))]
    public class MQTTBrokerClient : INotifyPropertyChanged, IProvideUserControls
    {

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

        public MQTTBrokerClient()
        {

        }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }

        private MQTTTopicList _lstTopic = new MQTTTopicList();
        public MQTTTopicList lstTopic
        {
            get { return _lstTopic; }
            set
            {
                _lstTopic = value;
                OnPropertyChanged("lstTopic");
            }
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", _Id, typeof(string));
            info.AddValue("lstTopic", _lstTopic, typeof(MQTTTopicList));
        }

         public MQTTBrokerClient(SerializationInfo info, StreamingContext context)
        {
            _Id = (string)info.GetValue("Id", typeof(string));
            _lstTopic = (MQTTTopicList)info.GetValue("lstTopic", typeof(MQTTTopicList));
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
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientListControl))]
    public class MQTTBrokerClientList : ObservableCollection<MQTTBrokerClient> ,  ISerializable, IProvideUserControls
    {

        public MQTTBrokerClientList()
        {

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

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lst", this, this.GetType());
        }

        public MQTTBrokerClientList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

    }
}
