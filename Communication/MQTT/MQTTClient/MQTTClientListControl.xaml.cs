using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTClientListControl : UserControl
    {

        public MQTTClientListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public MQTTClientList GetSelectedItems()
        {
            MQTTClientList lst = new MQTTClientList();
            foreach (MQTTClient v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public MQTTClientList GetItemsSource()
        {
            MQTTClientList lst = new MQTTClientList();
            foreach (MQTTClient v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MQTTClient data = dg.SelectedItem as MQTTClient;
            if (data == null) return;
            dpTopics.Children.Clear();
            dpTopics.Children.Add(data.lstTopic.GetUserControls()[0]);
        }
    }
}