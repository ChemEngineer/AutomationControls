using System.Windows.Controls;

namespace AutomationControls
{

    public partial class SqlDataListControl : UserControl
    {

        public SqlDataListControl() : base()
        {
            InitializeComponent();
        }

        private void miQueryIdAndDate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dg.ItemsSource = Sql.StaticMethods.GetLastUpdated("PostingData");
        }

        private void miQueryAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dg.ItemsSource = Sql.StaticMethods.GetAllData("PostingData");
        }

    }
}