using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Servers
{
    /// <summary>
    /// Interaction logic for TcpServerListControl.xaml
    /// </summary>
    public partial class TcpServerListControl : UserControl
    {
        public TcpServerListControl()
        {
            InitializeComponent();
        }

        private void miIdentify_Click(object sender, RoutedEventArgs e)
        {
            var data = (lbItems.SelectedItem as IAsyncTcpServer);
            if (data == null) return;

        }

        private void miStart_Click(object sender, RoutedEventArgs e)
        {
            var data = (lbItems.SelectedItem as IAsyncTcpServer);
            if (data == null) return;
            data.Start();
            // tbStatus.Text = "Started Service on " + data.ipAddress + ":" + data.port;
        }

        private void miStop_Click(object sender, RoutedEventArgs e)
        {
            var data = (lbItems.SelectedItem as IAsyncTcpServer);
            if (data == null) return;
            data.cts.Cancel();
            data.Stop();
            //   tbStatus.Text = "Stopped Service on " + data.ipAddress + ":" + data.port;
        }
    }
}
