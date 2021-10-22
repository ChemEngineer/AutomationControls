using AutomationControls.Communication.MQTT;
using AutomationControls.Interfaces;
using MQTTnet;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Communication
{
    public class MQTTClientCommunicator : MQTTClient, ICommunications, ISerializable
    {
        public bool IsBusy
        {
            get { return false; }
        }

        public Enums.DeviceCommunications _bus
        {
            get { return Enums.DeviceCommunications.MQTTClient; }
        }

        private void InitClient()
        {
            var options = new MQTTnet.Client.Options.MqttClientOptionsBuilder()
                            .WithClientId(Id)
                            .WithTcpServer(host)
                            .WithCleanSession()
                            .Build();

            client.ConnectAsync(options, CancellationToken.None);



            //client.ConnectedHandler.a.Connected += (sender, e) =>
            //{
            //    base.IsConnected = true;
            //};

            //client.Disconnected += (sender, e) =>
            //{
            //    base.IsConnected = false;
            //};

        }

        public async void OpenCommunicationChannel()
        {

            var options = new MQTTnet.Client.Options.MqttClientOptionsBuilder()
                            .WithClientId(Id)
                            .WithTcpServer(host, Int16.Parse(port))
                            .WithCleanSession()
                            .Build();

            await client.ConnectAsync(options, CancellationToken.None);
        }

        public Task OpenCommunicationChannelAsync()
        {
            InitClient();
            return Task.Delay(0);
        }

        public void CloseCommunicationChannel()
        {

            client.DisconnectAsync(new MQTTnet.Client.Disconnecting.MqttClientDisconnectOptions() { ReasonCode = 0 }, CancellationToken.None);
            //base.IsConnected = false;
        }

        public bool IsChannelOpen
        {
            get { return IsConnected; }
        }

        public void SendString(string command, string destination = "")
        {
            foreach (var v in lstTopic)
            {
                SendBytes(Encoding.ASCII.GetBytes(command), v.Topic);
            }
        }

        public void SendBytes(byte[] b, string destination = "")
        {
            foreach (var v in lstTopic)
            {
                client.PublishAsync(new MQTTnet.MqttApplicationMessage() { Topic = v.Topic, Payload = b, QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce }, CancellationToken.None);
            }
        }

        public string ReadString()
        {
            return "";
        }

        public Task SendStringAsync(string command, string destination = "")
        {
            return SendBytesAsync(Encoding.ASCII.GetBytes(command), destination);
        }

        public Task SendBytesAsync(byte[] b, string destination = "")
        {
            foreach (var v in lstTopic)
            {
                client.PublishAsync(new MqttApplicationMessage() { Topic = v.Topic, Payload = b }, CancellationToken.None);
            }
            return Task.Delay(0);
        }

        public Task<string> ReadStringAsync()
        {
            throw new NotImplementedException();
        }

        public System.Windows.Controls.UserControl GetUserControl()
        {
            return new MQTT.MQTTClientControl() { DataContext = this };
        }

        string _ReadDelimiter = "";
        public string ReadDelimiter
        {
            get
            {
                return _ReadDelimiter;
            }
            set
            {
                _ReadDelimiter = value;
            }
        }

        public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
    }
}