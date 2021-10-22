using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for WS2812Control.xaml
    /// </summary>
    public partial class WS2812Control : UserControl
    {
        // WS2812LED led = new WS2812LED();
        public WS2812Control()
        {
            InitializeComponent();

        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            StackPanel sp = new StackPanel();
            WS2812LED led = DataContext as WS2812LED;
            if (led == null) return;

            led.PropertyChanged += (sender2, e2) =>
           {
               led.SendCommand();

           };

            sp.Orientation = Orientation.Horizontal;
            sp.Children.Clear();
            // sp.Children.Add(WPF.DataBinding.Generate_UserControl(led, BindingMode.TwoWay));
            sp.Children.Add(led.comm.GetUserControl());
            this.Content = sp;
        }
    }
}