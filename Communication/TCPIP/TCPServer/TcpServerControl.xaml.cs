using AutomationControls.Communication.TCPIP.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Communication.TCPIP.UserControls.TCPServer
{
    /// <summary>
    /// Interaction logic for TcpServerControl.xaml
    /// </summary>
    public partial class TcpServerControl : UserControl
    {
        public IAsyncTcpServer data = null;

        public TcpServerControl()
        {
            InitializeComponent();
            if (DataContext != null)
            {
                if (DataContext.GetType().IsAssignableFrom(typeof(IAsyncTcpServer)))
                {
                    data = (IAsyncTcpServer)DataContext;
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var res = e.NewValue.GetType().GetInterfaces().Where(x => x.Name == "IAsyncTcpServer");
            if (res.Count() > 0)
            {
                data = (IAsyncTcpServer)e.NewValue;
            }
        }

    }
}
