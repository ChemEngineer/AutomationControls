using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for ucRGBSliders.xaml
    /// </summary>
    public partial class WS2812Sliders : UserControl
    {

        public WS2812Sliders()
        {
            InitializeComponent();
        }

        private void sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            WS2812LED data = DataContext as WS2812LED;
            if (data != null)
            {
                if (sld != null)
                {
                    this.Foreground = new SolidColorBrush(Color.FromArgb((byte)data.brightness, (byte)data.red, (byte)data.green, (byte)data.blue));
                    if (sld.IsMouseOver)
                        data.SendCommand();
                }
            }

        }


    }
}
