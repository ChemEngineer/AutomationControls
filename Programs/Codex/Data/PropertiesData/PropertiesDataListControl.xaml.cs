using System.Windows.Controls;

namespace AutomationControls.Codex.Data
{

    public partial class PropertiesDataListControl : UserControl
    {

        public PropertiesDataListControl() : base()
        {
            InitializeComponent();
        }

        public PropertiesDataList GetSelectedItems()
        {
            PropertiesDataList lst = new PropertiesDataList();
            foreach (PropertiesData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public PropertiesDataList GetItemsSource()
        {
            PropertiesDataList lst = new PropertiesDataList();
            foreach (PropertiesData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}