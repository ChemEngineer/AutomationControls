using AutomationControls.Communication.Serial.DataClasses;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
//using AutomationControls.Extensions;

namespace AutomationControls.Communication.Serial.UserControls
{
    /// <summary>
    /// Interaction logic for SerialPortOptionsControl.xaml
    /// </summary>
    public partial class SerialPortOptionsControl : UserControl
    {
        public SerialPortOptionsControl()
        {
            InitializeComponent();
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();

            SerialPortData data = DataContext as SerialPortData;
            if (data != null)
            {
                data.progressReceive.ProgressChanged += (sender2, e2) => { System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { tbStatus.Text = e2; })); };
                data.progressSend.ProgressChanged += (sender2, e2) => { System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { tbStatus.Text = e2; })); };
                await data.OpenAsync();
                if (data.sp.IsOpen)
                {
                    // data.ReadBufferSize = data.sp.ReadBufferSize;
                    data.ReadTimeout = data.sp.ReadTimeout;
                    //data.WriteBufferSize = data.sp.WriteBufferSize;
                    data.WriteTimeout = data.sp.WriteTimeout;
                    data.ReceivedBytesThreshold = data.sp.ReceivedBytesThreshold;
                    data.ParityReplace = data.sp.ParityReplace;

                    data.CDHolding = data.sp.CDHolding;
                    data.CtsHolding = data.sp.CtsHolding;
                    data.DsrHolding = data.sp.DsrHolding;
                    //AutomationControls.Windows.Utilities.PropertiesMonitor monitor = new Windows.Utilities.PropertiesMonitor(data.sp, data);
                    //monitor.MonitorPropertiesAsync(cts.Token);
                }
            }
        }

        private async void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            SerialPortData data = DataContext as SerialPortData;
            if (data != null)
            {
                await data.CloseAsync();
                cts.Cancel();
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SerialPortData data = DataContext as SerialPortData;
            if (data != null)
            {
                await data.OpenAsync();
                await data.SendAsync(tbSend.Text);
                if (!data.keepOpen)
                    await data.CloseAsync();
            }
        }
    }
}
