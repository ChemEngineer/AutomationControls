using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTTopicListControl : UserControl
    {

        public MQTTTopicListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public MQTTTopicList GetSelectedItems()
        {
            MQTTTopicList lst = new MQTTTopicList();
            foreach (MQTTTopic v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public MQTTTopicList GetItemsSource()
        {
            MQTTTopicList lst = new MQTTTopicList();
            foreach (MQTTTopic v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}