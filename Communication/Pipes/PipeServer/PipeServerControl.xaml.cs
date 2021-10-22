using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Communication.Pipes
{
    /// <summary>
    /// Interaction logic for PipeServerControl.xaml
    /// </summary>
    public partial class PipeServerControl : UserControl
    {
        Progress<string> status = new Progress<string>();
        CancellationTokenSource cts = new CancellationTokenSource();

        public PipeServerControl()
        {
            InitializeComponent();
            status.ProgressChanged += (sender, e) =>
            {
                //  this.txbStatus.Text = e;
            };
        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            await PipeServer.StartListeningAsync("Codex", null);
            //            await PipeServer.StartListeningAsync("Codex", s => { txbStatus.Text = s; });
            ////  NamedPipeServerStreamData data = DataContext as NamedPipeServerStreamData;
            //NamedPipeServerStreamDataList lst = new NamedPipeServerStreamDataList();
            ////  if(data == null) return;
            //if(!String.IsNullOrEmpty(tbPipeName.Text))
            //{
            //    await lst.MonitorPipes(tbPipeName.Text);
            //    // await data.StartServerAsync("Codex", status);
            //    // txbStatus.Dispatcher.Invoke((Action)(() => { this.txbStatus.Text = "Started Pipe: Codex"; })); 
            //    // }
            //}
        }


        public async void Monitor(String serverName, Action<string> action)
        {
            cts = new CancellationTokenSource();
            while (!cts.IsCancellationRequested)
            {
                await PipeServer.StartListeningAsync(serverName, action);
            }
        }

        private void cbMonitor_Checked(object sender, RoutedEventArgs e)
        {
            Monitor("Codex", s => { txbStatus.Text = s; });
        }

        private void cbMonitor_Unchecked(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}