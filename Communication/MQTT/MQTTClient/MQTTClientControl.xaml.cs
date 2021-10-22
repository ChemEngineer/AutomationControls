using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTClientControl : UserControl
    {

        private bool mSubscribed = false;
        CancellationTokenSource cts = new CancellationTokenSource();
        public MQTTClientControl()
            : base()
        {
            InitializeComponent();

            if (!mSubscribed)
            {
                DataContextChanged += (sender, e) =>
                {
                    MQTTClient data = DataContext as MQTTClient;
                    if (data == null) return;

                    data.client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnAppMessage);
                    data.client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
                    data.client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);

                    Connect();
                };
            }


        }

        #region MQTTNet Async Event Handlers

        private void OnAppMessage(MqttApplicationMessageReceivedEventArgs e)
        {
           
        }

        private void OnConnected(MqttClientConnectedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                MQTTClient data = DataContext as MQTTClient;
                if (data == null) return;
                data.IsConnected = true; mSubscribed = true;
                //SubscribeToTopics();
            }));
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
           {
               MQTTClient data = DataContext as MQTTClient;
               if (data == null) return;
               data.IsConnected = false;
           }));
        }

        #endregion

        //List<Pair<string, string>> lstMsg = new List<Pair<string, string>>();
        //string lastMsg = "";
        void client_ApplicationMessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            //bool execute = false;
            //System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    var v = this.GetType().GetHashCode();

            //    MQTTClient data = DataContext as MQTTClient;
            //    if (data == null) return;
            //    tb.Text = e.ClientId + " - " + e.ApplicationMessage.Topic + " --> " + Encoding.ASCII.GetString(e.ApplicationMessage.Payload) + Environment.NewLine + tb.Text;

            //    if (e.ApplicationMessage.Topic.StartsWith("hermes/intent/Neuro"))
            //    {
            //        string newTopic = "led/snips";
            //        var d = data.ProcessCommand(Encoding.ASCII.GetString(e.ApplicationMessage.Payload));
            //        if (e.ApplicationMessage.Topic != lastMsg)
            //        {
            //            lastMsg = e.ApplicationMessage.Topic;
            //            if (d.intent.intentName.EndsWith("TurnOn"))
            //                data.client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("on"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
            //            else if (d.intent.intentName.EndsWith("TurnOff"))
            //                data.client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("off"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
            //            else if (d.intent.intentName.EndsWith("Rainbow"))
            //                data.client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("rainbowup"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
            //            else if (d.intent.intentName.EndsWith("Random"))
            //                data.client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("random"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
            //            else if (d.intent.intentName.EndsWith("Night"))
            //                data.client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = newTopic, Payload = System.Text.Encoding.ASCII.GetBytes("night"), QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });//OnTopicReadyEvent(new TopicReadyEventArgs() { Topic = "LED", Payload = "On" });
            //        }
            //    }
            //}));
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { tb.Text = e.ClientId + " - " + e.ApplicationMessage.Topic + " --> " + Encoding.ASCII.GetString(e.ApplicationMessage.Payload) + Environment.NewLine + tb.Text; }));
        }

        //void client_Disconnected(object sender, MQTTnet.Client.MqttClientDisconnectedEventArgs e)
        //{
        //    System.Windows.Application.Current.Dispatcher.Invoke((Action)(async () =>
        //    {
        //        MQTTClient data = DataContext as MQTTClient;
        //        if (data == null) return;
        //        //data.IsConnected = false;

        //        //await System.Threading.Tasks.Task.Delay(250);
        //        //Connect();
        //    }));
        //}

        //void client_Connected(object sender, MQTTnet.Client.MqttClientConnectedEventArgs e)
        //{
        //    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
        //    {
        //        MQTTClient data = DataContext as MQTTClient;
        //        if (data == null) return;
        //        data.IsConnected = true;
        //        SubscribeToTopics();
        //    }));
        //}

        private void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Connect();
        }

        public async void Connect()
        {
            MQTTClient data = DataContext as MQTTClient;
            if (data == null || data.Id == null || data.host == null || data.port == null) return;



            var options = new MQTTnet.Client.Options.MqttClientOptionsBuilder()
                            .WithClientId(data.Id)
                            .WithTcpServer(data.host, Int16.Parse(data.port))
                            .WithCleanSession()
                            .Build();


            try
            {
                await data.client.ConnectAsync(options, CancellationToken.None);
            }
            catch (Exception)
            {

                data.IsConnected = false;
            }

            if (data.client.IsConnected)
            {
                data.IsConnected = true;
                SubscribeToTopics();
            }
        }
        private void btnSubscribe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubscribeToTopics();
        }

        private async void SubscribeToTopics()
        {
            MQTTClient data = DataContext as MQTTClient;
            if (data == null) return;
            List<TopicFilter> lst = new List<TopicFilter>();
            foreach (var v in data.lstSubs)
            {
                TopicFilter filter = new TopicFilter()
                {
                    Topic = v.Topic,
                    QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce
                };
                lst.Add(filter);
            }
           
            if (lst.Count == 0)
                lst.Add(new TopicFilter() { Topic = "default" });

            MQTTnet.Client.Subscribing.MqttClientSubscribeOptions mqttClientSubscribeOptions = new MQTTnet.Client.Subscribing.MqttClientSubscribeOptions() { TopicFilters = lst };

            if(data.client.IsConnected)
            {
                try
                {
                    await data.client.SubscribeAsync(mqttClientSubscribeOptions, CancellationToken.None);
                }
                catch 
                {

                    
                }
            }
        }

        private void btnUnsubscribe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MQTTClient data = DataContext as MQTTClient;
            if (data == null) return;
            List<string> lst = new List<string>();
            foreach (var v in data.lstSubs)
            {
                lst.Add(v.Topic);
            }
            MQTTnet.Client.Unsubscribing.MqttClientUnsubscribeOptions mqttClientUnsubscribeOptions = new MQTTnet.Client.Unsubscribing.MqttClientUnsubscribeOptions() { TopicFilters = lst };
            data.client.UnsubscribeAsync(mqttClientUnsubscribeOptions, CancellationToken.None);
        }

        private async void btnPublish_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MQTTClient data = DataContext as MQTTClient;
            if (data == null) return;

            foreach (var v in data.lstTopic)
            {
                MqttApplicationMessage mqttApplicationMessage = new MqttApplicationMessage()
                {
                    Topic = v.Topic,
                    Payload = Encoding.ASCII.GetBytes(tbPublish.Text)
                };
                await data.client.PublishAsync(mqttApplicationMessage, CancellationToken.None);
            }
        }
    }

    public class Pair<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }
}