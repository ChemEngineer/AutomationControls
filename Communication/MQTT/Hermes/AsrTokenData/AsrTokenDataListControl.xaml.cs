using System.Windows.Controls;

namespace AutomationControls.communication.mqtt
{

    public partial class AsrTokenDataListControl : UserControl
    {

        public AsrTokenDataListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public AsrTokenDataList GetSelectedItems()
        {
            AsrTokenDataList lst = new AsrTokenDataList();
            foreach (AsrTokenData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public AsrTokenDataList GetItemsSource()
        {
            AsrTokenDataList lst = new AsrTokenDataList();
            foreach (AsrTokenData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}