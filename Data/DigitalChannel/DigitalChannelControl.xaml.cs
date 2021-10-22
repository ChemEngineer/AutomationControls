using System.Windows.Controls;

namespace AutomationControls.Controllers.DataClasses
{

    public partial class DigitalChannelControl : UserControl
    {

        public DigitalChannelControl()
            : base()
        {
            InitializeComponent();
        }



        private void cbDigitalInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var type = typeof(IDigitalChannel);
            //var types = AutomationControls.SerializationSurrogateControl.lstSurrogates.Where(x => x is IDigitalChannel);
            //cbDigitalInput.ItemsSource = types;

        }

    }
}