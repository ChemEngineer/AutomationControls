using System;
using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Servers
{
    public partial class TcpServerControl : UserControl
    {
        // Type type = null;

        public TcpServerControl()
        {
            InitializeComponent();

            Application.Current.DispatcherUnhandledException += (sender, e) =>
            {
                var data = (DataContext as SqlDataServer);
                if (data == null) return;
                data.Dispose();
            };

            Application.Current.Exit += (sender, e) =>
            {
                var data = (DataContext as SqlDataServer);
                if (data == null) return;
                data.Dispose();
            };

            DataContextChanged += (sender, e) =>
            {
                var v = e.NewValue;
                var data = (DataContext as SqlDataServer);
                if (data == null) return;
                data.DataReadyEvent += (sender2, e2) =>
                {
                    tblastReceived.Dispatcher.Invoke((Action)(() => { if (e2.text != "") tblastReceived.Text += e2.text; }));
                };
                if (data.openOnStart) data.Start();
            };
        }


        private void miIdentify_Click(object sender, RoutedEventArgs e)
        {
            //Type typeArg = this.GetType();
            //Type genericClass = typeof(Serializer<>);
            //Type constructedClass = genericClass.MakeGenericType(typeArg);
            //object created = Activator.CreateInstance(constructedClass, new object[] { this });

            var data = (DataContext as SqlDataServer);
            if (data == null) return;

        }

        private void miStart_Click(object sender, RoutedEventArgs e)
        {
            var data = (DataContext as SqlDataServer);
            if (data == null) return;
            data.Start();
            tbStatus.Text = "Started Service on " + data.ipAddress + ":" + data.port;
        }

        private void miStop_Click(object sender, RoutedEventArgs e)
        {
            var data = (DataContext as SqlDataServer);
            if (data == null) return;
            data.cts.Cancel();
            data.Stop();
            tbStatus.Text = "Stopped Service on " + data.ipAddress + ":" + data.port;
        }


    }
}
