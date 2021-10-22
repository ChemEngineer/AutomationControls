using System.Windows.Controls;

namespace AutomationControls.Communication.MQTT
{

    public partial class MQTTTestListControl : UserControl
    {

        public MQTTTestListControl()  : base()
        {
            InitializeComponent();
			dg.Items.Clear();
        }

		 public MQTTTestList GetSelectedItems()
        {
            MQTTTestList lst = new MQTTTestList();
            foreach (MQTTTest v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public MQTTTestList GetItemsSource()
        {
            MQTTTestList lst = new MQTTTestList();
            foreach (MQTTTest v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}