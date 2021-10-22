using System.Windows.Controls;

namespace AutomationControls.Devices.LED.WS2812
{
    /// <summary>
    /// Interaction logic for ColorPickerControl.xaml
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        public ColorPickerControl()
        {
            InitializeComponent();



            //rgbpicker.ColorChanged += (sender5, e5) =>
            //{
            //    WS2812LED data = DataContext as WS2812LED;
            //    if (data != null)
            //    {

            //        var c = e5.Value;
            //        if (data != null)
            //        {
            //            data.blue = c.B;
            //            data.red = c.R;
            //            data.green = c.G;
            //        }
            //    }
            //};

        }
    }
}
