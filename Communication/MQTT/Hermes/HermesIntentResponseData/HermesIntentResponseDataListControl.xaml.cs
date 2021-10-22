using System.Windows.Controls;

namespace AutomationControls.communication.mqtt
{

    public partial class HermesIntentResponseDataListControl : UserControl
    {

        public HermesIntentResponseDataListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public HermesIntentResponseDataList GetSelectedItems()
        {
            HermesIntentResponseDataList lst = new HermesIntentResponseDataList();
            foreach (HermesIntentResponseData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public HermesIntentResponseDataList GetItemsSource()
        {
            HermesIntentResponseDataList lst = new HermesIntentResponseDataList();
            foreach (HermesIntentResponseData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}