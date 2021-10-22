using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication.Pipes
{

    public partial class NamedPipeClientStreamDataControl : UserControl
    {

        Progress<string> progress = new Progress<string>();

        public NamedPipeClientStreamDataControl() : base()
        {
            InitializeComponent();
            progress.ProgressChanged += (sender, e) =>
            {
                txbStatus.Text = e;
            };
        }

        private async void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                NamedPipeClientStreamData data = DataContext as NamedPipeClientStreamData;
                if (data != null)
                {
                    data.Connect();
                }
            }, CancellationToken.None);
        }


        private async void btnSend_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                NamedPipeClientStreamData data = DataContext as NamedPipeClientStreamData;
                if (data != null)
                {
                    data.Send(tbSend.Text);
                }
            }, CancellationToken.None);
        }


    }
}