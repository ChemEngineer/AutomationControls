using System.Windows.Controls;

namespace AutomationControls.communication.mqtt
{

    public partial class HermesIntentDataListControl : UserControl
    {

        public HermesIntentDataListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public HermesIntentDataList GetSelectedItems()
        {
            HermesIntentDataList lst = new HermesIntentDataList();
            foreach (HermesIntentData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public HermesIntentDataList GetItemsSource()
        {
            HermesIntentDataList lst = new HermesIntentDataList();
            foreach (HermesIntentData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}