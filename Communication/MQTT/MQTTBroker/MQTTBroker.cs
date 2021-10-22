using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Windows.Controls;
using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using AutomationControls.Extensions;
using MQTTnet.Server;
using MQTTnet;
using System.Text;
using AutomationControls.Communication.MQTT;
using MQTTnet.Client.Receiving;

namespace AutomationControls.Communication.MQTT
{
    [Serializable]
 //   [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTBrokerControl))]
    public class MQTTBroker : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public IMqttServer mqttServer;

        public MQTTBroker()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (lstPubData == null)
                lstPubData = new MQTTPubDataList() { };
            if (lstClient == null)
                lstClient = new MQTTBrokerClientList() { };

            mqttServer = new MqttFactory().CreateMqttServer();

            mqttServer.ClientConnectedHandler  = new MqttServerClientConnectedHandlerDelegate( ( e) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lstClient.Add(new MQTTBrokerClient() { Id = e.ClientId });
                }));
            });

            mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate (( e) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
               {
                   var res = lstClient.Where(x => x.Id == e.ClientId);
                   if (res.Count() == 0) return;
                   lstClient.Remove(res.First());
               }));
            });

            mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate (( e) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    var res = lstClient.Where(x => x.Id == e.ClientId);
                    if (res.Count() == 0) return;
                    var res2 = res.First().lstTopic.Where(x => x.Topic == e.TopicFilter.Topic);
                    if (res2.Count() == 0)
                        res.First().lstTopic.Add(new MQTTTopic() { Topic = e.TopicFilter.Topic });
                }));

            });

            mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate( ( e) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    var res = lstClient.Where(x => x.Id == e.ClientId);
                    if (res.Count() == 0) return;
                    var res2 = res.First().lstTopic.Where(x => x.Topic == e.TopicFilter);
                    if (res2.Count() > 0)
                        res.First().lstTopic.Remove(res2.First());
                }));
                Console.WriteLine(e.ClientId + " Unsubscribed To: " + e.TopicFilter);
                // Remove from List Here     filter
            });

            mqttServer.ApplicationMessageReceivedHandler = new MessageHandler(this);
            //mqttServer.ApplicationMessageReceivedHandler = (e) =>
            //{
            //    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            //    {

            //    }));
            //    //  Console.WriteLine(e.ClientId + " -> " + Encoding.ASCII.GetString(e.ApplicationMessage.Payload));
            //);
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


        private MQTTTopicList _lstSubs = new MQTTTopicList();
        public MQTTTopicList lstSubs
        {
            get { return _lstSubs; }
            set
            {
                _lstSubs = value;
                OnPropertyChanged("lstSubs");
            }
        }



        private MQTTPubDataList _lstPubData = new MQTTPubDataList();
        public MQTTPubDataList lstPubData
        {
            get { return _lstPubData; }
            set
            {
                _lstPubData = value;
                OnPropertyChanged("lstPubData");
            }
        }

        private MQTTBrokerClientList _lstClient = new MQTTBrokerClientList();
        public MQTTBrokerClientList lstClient
        {
            get { return _lstClient; }
            set
            {
                _lstClient = value;
                OnPropertyChanged("lstClient");
            }
        }


        private bool _IsConnected;
        public bool IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }


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
            info.AddValue("lstPubData", _lstPubData, typeof(MQTTPubDataList));
            info.AddValue("lstClient", _lstClient, typeof(MQTTBrokerClientList));
            info.AddValue("IsConnected", _IsConnected, typeof(bool));
            info.AddValue("lstSubs", _lstSubs, typeof(MQTTTopicList));
            info.AddValue("Topic", _Topic, typeof(string));
        }
        public MQTTBroker(SerializationInfo info, StreamingContext context)
        {
            _lstPubData = (MQTTPubDataList)info.GetValue("lstPubData", typeof(MQTTPubDataList));
            _lstClient = (MQTTBrokerClientList)info.GetValue("lstClient", typeof(MQTTBrokerClientList));
            _IsConnected = (bool)info.GetValue("IsConnected", typeof(bool));
            _lstSubs = (MQTTTopicList)info.GetValue("lstSubs", typeof(MQTTTopicList));
            _Topic = (string)info.GetValue("Topic", typeof(string));
            Initialize();
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
            AutomationControls.Serialization.Serializer<MQTTBroker> ser = new AutomationControls.Serialization.Serializer<MQTTBroker>(this);
            ser.ToJSON(s);
        }

        public static MQTTBroker Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTBroker> ser = new AutomationControls.Serialization.Serializer<MQTTBroker>();
            var res = ser.FromJSON(s);
            return res;
        }

        #region TopicReadyEventHandler

        public delegate void TopicReadyEventHandler(object sender, TopicReadyEventArgs a);
        public event EventHandler<TopicReadyEventArgs> TopicReadyEvent;
        protected virtual void OnTopicReadyEvent(TopicReadyEventArgs e)
        {
            EventHandler<TopicReadyEventArgs> handler = TopicReadyEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public class TopicReadyEventArgs : EventArgs
        {
            public TopicReadyEventArgs() { }
            private MqttApplicationMessageReceivedEventArgs _e;
            public MqttApplicationMessageReceivedEventArgs e { get { return _e; } set { _e = value; } }
        }

        #endregion

        public class MessageHandler : IMqttApplicationMessageReceivedHandler
        {
            MQTTBroker _broker;
            public MessageHandler(MQTTBroker broker)
            {
                _broker = broker;
            }
            public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
            {

                return Task.Run(() => { 
                    _broker.OnTopicReadyEvent(new TopicReadyEventArgs() { e = e });  }, CancellationToken.None);           
            }
        }
    }

    [Serializable]
    //[DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTBrokerListControl))]
    public class MQTTBrokerList : ObservableCollection<MQTTBroker>, ISerializable, IProvideUserControls
    {
        public MQTTBrokerList() { }

        public MQTTBrokerList(IEnumerable<MQTTBroker> lst) { lst.ForEach(x => this.Add(x)); }

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

        public MQTTBrokerList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTBrokerList> ser = new AutomationControls.Serialization.Serializer<MQTTBrokerList>(this);
            ser.ToJSON(s);
        }

        public static MQTTBrokerList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTBrokerList> ser = new AutomationControls.Serialization.Serializer<MQTTBrokerList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}