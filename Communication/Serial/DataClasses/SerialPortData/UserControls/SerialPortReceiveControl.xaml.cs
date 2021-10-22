using AutomationControls.Communication.Serial.DataClasses;
using AutomationControls.Extensions;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Communication.Serial.UserControls
{
    /// <summary>
    /// Interaction logic for SerialPortReceiveControl.xaml
    /// </summary>
    public partial class SerialPortReceiveControl : UserControl
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public SerialPortReceiveControl()
        {
            InitializeComponent();

            cbMonitorToDelimiter.Checked += async (sender, e) =>
            {
                SerialPortData data = DataContext as SerialPortData;
                if (data != null)
                {
                    while ((bool)cbMonitorToDelimiter.IsChecked && !cts.IsCancellationRequested)
                    {
                        string res = await data.sp.WaitForDelimiter(tbDelimiter.Text, cts.Token);
                        if (res.Contains(data.ReadDelimiter))
                        {
                            res = res.Replace(data.ReadDelimiter, "");
                            data.OnRaiseDataReceivedEvent(new Events.DataReceivedEventArgs() { recent = res });
                            data.LastResponse = res;
                            txbStatus.Dispatcher.Invoke((Action)(() => { txbStatus.Text = res; }));
                        }
                    };
                }
            };
            cbMonitorToDelimiter.Unchecked += (sender, e) =>
            {
                cts.Cancel();
                cts = new CancellationTokenSource();
            };
        }

        private void cbMonitorToDelimiter_Unchecked(object sender, RoutedEventArgs e)
        {
            txbStatus.Dispatcher.Invoke((Action)(() => { txbStatus.Text = "<--- Check to Monitor"; }));
        }
    }
}
