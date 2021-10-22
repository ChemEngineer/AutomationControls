using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for ucColorsListBox.xaml
    /// </summary>
    public partial class WS2812ColorsListBox : UserControl
    {
        public WS2812ColorsListBox()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            PropertyInfo[] clrs = typeof(Colors).GetProperties();
            foreach (PropertyInfo info in clrs)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = info.Name;
                Color c = (Color)info.GetValue(this, null);
                item.Background = new SolidColorBrush(c);
                lb.Items.Add(item);
            }
        }



        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WS2812LED data = DataContext as WS2812LED;
            if (data != null)
            {
                ListBoxItem lbi = (ListBoxItem)lb.SelectedItem;
                if (lbi != null)
                {
                    Color c = ((SolidColorBrush)lbi.Background).Color;
                    data.red = c.R;
                    data.green = c.G;
                    data.blue = c.B;

                    data.SendCommand();
                }
            }
        }

    }
}
