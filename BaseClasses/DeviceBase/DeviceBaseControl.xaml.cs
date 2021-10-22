using System.Windows.Controls;

namespace AutomationControls.BaseClasses
{
    /// <summary>
    /// Interaction logic for DeviceBaseControl.xaml
    /// </summary>
    public partial class DeviceBaseControl : UserControl
    {
        public DeviceBaseControl()
        {
            InitializeComponent();
            cb.SelectionChanged += (sender, e) =>
            {
                DeviceBase data = DataContext as DeviceBase;
                if (data != null)
                {
                    cc.Content = data.comm.GetUserControl();
                    //data.comm.RaiseDataReceivedEvent += (sender2, e2) => { };
                }
            };
        }
    }
}
