using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTPubDataListControl : UserControl
    {

        public MQTTPubDataListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public MQTTPubDataList GetSelectedItems()
        {
            MQTTPubDataList lst = new MQTTPubDataList();
            foreach (MQTTPubData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public MQTTPubDataList GetItemsSource()
        {
            MQTTPubDataList lst = new MQTTPubDataList();
            foreach (MQTTPubData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}