using AutomationControls.Attributes;
using AutomationControls.communication.mqtt;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientControl))]
    public class MQTTClient : MQTTClientBase<HermesIntentResponseData>
    {

        public MQTTClient()
        {
            

        }

        public override HermesIntentResponseData ProcessCommand(string command)
        {
            HermesIntentResponseData d = JsonConvert.DeserializeObject<HermesIntentResponseData>(command);
            return d;
        }

        void client_ApplicationMessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            //bool execute = false;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                var v = this.GetType().GetHashCode();


                //  tb.Text = e.ClientId + " - " + e.ApplicationMessage.Topic + " --> " + Encoding.ASCII.GetString(e.ApplicationMessage.Payload) + Environment.NewLine + tb.Text;

                if (e.ApplicationMessage.Topic.StartsWith("hermes/intent/Neuro"))
                {
                    //string newTopic = "led/snips";
                    var d = ProcessCommand(Encoding.ASCII.GetString(e.ApplicationMessage.Payload));

                    //if (d.intent.intentName.EndsWith("TurnOn"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("on"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //else if (d.intent.intentName.EndsWith("TurnOff"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("off"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //else if (d.intent.intentName.EndsWith("Rainbow"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("rainbowup"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //else if (d.intent.intentName.EndsWith("Random"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("random"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //else if (d.intent.intentName.EndsWith("NightLight"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("nightlight"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });//OnTopicReadyEvent(new TopicReadyEventArgs() { Topic = "LED", Payload = "On" });
                    //if (d.intent.intentName.EndsWith("Fire"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("fire"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //if (d.intent.intentName.EndsWith("BrightWhite"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("brightwhite"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                    //if (d.intent.intentName.EndsWith("RandomRepeat"))
                    //    client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("randomrepeat"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
                }
            }));
        }
    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientControl))]
    public abstract class MQTTClientBase<T> : INotifyPropertyChanged, IProvideUserControls
    {
        public MQTTnet.Client.IMqttClient client;

        public abstract T ProcessCommand(string command);

        public MQTTClientBase()
        {
            Initialize();
            var factory = new MQTTnet.MqttFactory();
            client = factory.CreateMqttClient();

            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnAppMessage);
            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);

        }

        #region Redirecting MqttNet Async Handlers to Standard Event Handlers

        #region MQTTNet Async Event Handlers

        private void OnAppMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            OnTopicReadyEvent(new TopicReadyEventArgs() {  e = e }) ; 
        }

        private void OnConnected(MqttClientConnectedEventArgs e)
        {
            OnConnectedEvent(e);
            IsConnected = true;
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            OnDisconnected(e);
        }



        #region ConnectedEventHandler

        public delegate void ConnectedEventHandler(object sender, MqttClientConnectedEventArgs e);
        public event EventHandler<MqttClientConnectedEventArgs > ConnectedEvent;

                protected virtual void OnConnectedEvent(MqttClientConnectedEventArgs  e)
        {
            EventHandler < MqttClientConnectedEventArgs  > handler = ConnectedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        

    #endregion

    #endregion



    #endregion

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


        private string _host;
        public string host
        {
            get { return _host; }
            set
            {
                _host = value;
                OnPropertyChanged("host");
            }
        }


        private string _port;
        public string port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged("port");
            }
        }


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





        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", _Id, typeof(string));
            info.AddValue("lstTopic", _lstTopic, typeof(MQTTTopicList));
            info.AddValue("IsConnected", _IsConnected, typeof(bool));
            info.AddValue("host", _host, typeof(string));
            info.AddValue("port", _port, typeof(string));
            info.AddValue("lstSubs", _lstSubs, typeof(MQTTTopicList));
        }
        public MQTTClientBase(SerializationInfo info, StreamingContext context)
        {
            _Id = (string)info.GetValue("Id", typeof(string));
            _lstTopic = (MQTTTopicList)info.GetValue("lstTopic", typeof(MQTTTopicList));
            //  _IsConnected = (bool)info.GetValue("IsConnected", typeof(bool));
            _host = (string)info.GetValue("host", typeof(string));
            _port = (string)info.GetValue("port", typeof(string));
            _lstSubs = (MQTTTopicList)info.GetValue("lstSubs", typeof(MQTTTopicList));
        }


        #region IProvideUserControls

        MQTTClientControl uc = new MQTTClientControl();
        public UserControl[] GetUserControls()
        {
            var v = this.GetType().GetHashCode();
            uc.DataContext = this;
            return new UserControl[] { uc };
        }

        #endregion



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

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientListControl))]
    public class MQTTClientList : MQTTClientBaseListBase<HermesIntentResponseData>
    {

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTClientListControl))]
    public abstract class MQTTClientBaseListBase<T> : ObservableCollection<MQTTClientBase<T>>, ISerializable, IProvideUserControls
    {
        public MQTTClientBaseListBase() { }

        public MQTTClientBaseListBase(IEnumerable<MQTTClientBase<T>> lst) { lst.ForEach(x => this.Add(x)); }

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

        public MQTTClientBaseListBase(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}