using System.Windows.Controls;

namespace AutomationControls
{

    public partial class CodexDataListControl : UserControl
    {

        public CodexDataListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
        }

        public CodexDataList GetSelectedItems()
        {
            CodexDataList lst = new CodexDataList();
            foreach (CodexData v in dg.SelectedItems) { lst.Add(v); }
            return lst;
        }

        public CodexDataList GetItemsSource()
        {
            CodexDataList lst = new CodexDataList();
            foreach (CodexData v in dg.ItemsSource) { lst.Add(v); }
            return lst;
        }
    }
}