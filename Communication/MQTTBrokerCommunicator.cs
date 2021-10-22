using AutomationControls.Communication.MQTT;
using AutomationControls.Interfaces;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutomationControls.Communication
{
    class MQTTBrokerCommunicator : MQTTBroker, ICommunications, ISerializable
    {
       

        public MQTTBrokerCommunicator()
        {
           

        }
        public bool IsBusy
        {
            get { return false;  }
        }

        public Enums.DeviceCommunications _bus
        {
            get { return Enums.DeviceCommunications.MQTTBroker;  }
        }

        public void OpenCommunicationChannel()
        {
            //MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(100);
            mqttServer.StartAsync( new MqttServerOptionsBuilder().WithConnectionBacklog(100).Build()) ;
            base.IsConnected = true;
        }

        public Task OpenCommunicationChannelAsync()
        {
            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(100);
            mqttServer.StartAsync(optionsBuilder.Build());
            base.IsConnected = true;
            return Task.Delay(0);
        }

        public void CloseCommunicationChannel()
        {
            mqttServer.StopAsync();
                base.IsConnected = false;
        }

        public bool IsChannelOpen
        {
            get { return IsConnected; }
        }

        public void SendString(string command, string destination = "")
        {
            SendBytes(Encoding.ASCII.GetBytes(command), Topic);
        }

        public void SendBytes(byte[] b, string destination = "")
        {
            mqttServer.PublishAsync(new MqttApplicationMessage() { Topic = Topic, Payload = b, QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce });
        }

        public string ReadString()
        {
            throw new NotImplementedException();
        }

        public Task SendStringAsync(string command, string destination = "")
        {
             return SendBytesAsync(Encoding.ASCII.GetBytes(command), destination);
        }

        public Task SendBytesAsync(byte[] b, string destination = "")
        {
             mqttServer.PublishAsync(new MqttApplicationMessage() { Topic = Topic, Payload = b });
             return Task.Delay(0);
        }

        public Task<string> ReadStringAsync()
        {
            throw new NotImplementedException();
        }

        public System.Windows.Controls.UserControl GetUserControl()
        {
            return new MQTT.MQTTBrokerControl() { DataContext = this };
        }

        string _ReadDelimiter = "";
        public string ReadDelimiter
        {
            get
            {
                return  _ReadDelimiter;
            }
            set
            {
                _ReadDelimiter = value;
            }
        }

        public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
    }
}
