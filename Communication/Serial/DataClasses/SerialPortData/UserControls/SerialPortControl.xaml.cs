using AutomationControls.Communication.Serial.DataClasses;
using AutomationControls.Extensions;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomationControls.Communication.Serial.UserControls
{
    /// <summary>
    /// Interaction logic for ucSerialPort.xaml
    /// </summary>
    public partial class SerialPortControl : UserControl
    {
        private FlowDocument fd;

        public SerialPortControl()
        {
            InitializeComponent();
            this.DataContext = data = new SerialPortData();
            fd = commMonitor.rtb.Document;

            // data.RefreshSerialPorts();
            commMonitor.Load(data.progressSend, data.progressReceive);
            //  if(sp == null) sp = new SerialPort("COM29", 9600);
            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue.GetType().IsSubclassOf(typeof(SerialPortData)))
                {
                    this.data = e.NewValue as SerialPortData;
                    data.RefreshSerialPorts();
                    lbPorts.ItemsSource = data.lstPorts;
                    commMonitor.Load(data.progressSend, data.progressReceive);
                }
            };

            Unloaded += (sender, e) =>
            {
                ctsReceive.Cancel(); ctsReceive = new CancellationTokenSource();
                data.cts.Cancel();
                data.cts = new CancellationTokenSource();
            };

        }

        #region PropertyChanged Pattern

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        public SerialPortData data { get; set; }

        private async void lbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbPorts.SelectedItem == null) return;
            if (data == null) return;
            if (data.sp == null) data.sp = new SerialPort();

            if (!data.sp.IsOpen) await data.sp.CloseAsync();

            data.PortName = lbPorts.SelectedItem.ToString();
            try { await data.sp.OpenAsync(); } catch { }

            if (data.sp.IsOpen) { this.Background = Brushes.LightGreen; }
            else { this.Background = Brushes.PaleVioletRed; }
        }


        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await data.OpenAsync();
            await data.SendAsync(tbSend.Text);
            if (!data.keepOpen)
                await data.CloseAsync();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            data.cts.Cancel();
            data.cts = new CancellationTokenSource();
        }



        private async void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { await data.sp.SendLineAsync(tbSend.Text); };
        }


        private void lbPorts_MouseEnter(object sender, MouseEventArgs e)
        {
            data.RefreshSerialPorts();
        }

        #region Async Receive



        CancellationTokenSource ctsReceive = new CancellationTokenSource();
        private void cbReceive_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if ((bool)cb.IsChecked)
            {
                data.sp.ReceiveMonitor(data.progressReceive, data.cts.Token);
            }
            else
            {
                ctsReceive.Cancel(); ctsReceive = new CancellationTokenSource();
            }
        }

        #endregion



    }


}
