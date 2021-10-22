using AutomationControls.Interfaces;
using System.Windows.Controls;

namespace AutomationControls.UserControls
{
    /// <summary>
    /// Interaction logic for SurrogateProfileControl.xaml
    /// </summary>
    public partial class SurrogateProfileControl : UserControl
    {
        public SurrogateProfileControl()
        {
            InitializeComponent();

            db.DataReadyEvent += (sender, e) =>
            {
                ISerializationSurrogate surr = DataContext as ISerializationSurrogate;
                if (surr == null) return;

            };
        }
    }
}
