using System.Windows.Controls;
using System.Windows.Media;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for ColorCanvas.xaml
    /// </summary>
    public partial class ColorCanvas : UserControl
    {
        public ColorCanvas()
        {
            InitializeComponent();
            //clr.SelectedColorChanged += (sender, e) =>
            //{
            //    Color c = e.NewValue.Value;

            //    WS2812LED data = DataContext as WS2812LED;
            //    if (data != null)
            //    {
            //        data.blue = c.B;
            //        data.red = c.R;
            //        data.green = c.G;
            //        data.brightness = c.A;
            //        data.SendCommand();
            //    }
            //};
        }
    }
}
