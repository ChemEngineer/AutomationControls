using System;
using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTTestControl : UserControl
    {
        IProgress<string> pClient = new Progress<string>();
        IProgress<string> pServer = new Progress<string>();

        MQTTClient clientData;
        MQTTBroker brokerData;

        public MQTTTestControl() : base()
        {
            InitializeComponent();
            DataContextChanged += (sender, e) =>
            {
                MQTTTest data = DataContext as MQTTTest;
                if (data == null) return;
                data.PropertyChanged += (sender2, e2) =>
                {
                    if(e2.PropertyName == "client" && data.client != null)
                    {
                        data.client.TopicReadyEvent += (sender2, e2) =>
                        {
                            pClient.Report(e2.e.ClientId + " -- " + e2.e.ApplicationMessage);
                        };
                    }
                };

                
                //data.broker.TopicReadyEvent += (sender2, e2) =>
                //{
                //    pServer.Report(e2.e.ClientId + " -- " + e2.e.ApplicationMessage);
                //};
                ucMonitor.Load(pServer, pClient);
            };
        }
    }
}
