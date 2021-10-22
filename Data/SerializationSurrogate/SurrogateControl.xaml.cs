using AutomationControls.Interfaces;
using AutomationControls.WPF;
using System.Windows;
using System.Windows.Controls;

namespace AutomationControls.UserControls.SerializationSurrogate
{
    /// <summary>
    /// Interaction logic for SurrogateControl.xaml
    /// </summary>
    public partial class SurrogateControl : UserControl
    {
        public SurrogateControl()
        {
            InitializeComponent();

            db.DataReadyEvent += (sender, e) =>
            {
                ISerializationSurrogate surr = DataContext as ISerializationSurrogate;
                if (surr == null || surr.getData == null) return;
                bool b = surr.getData == dglst.SelectedItem;
                if (!b)
                {
                    surr.LoadProfile(surr.profileName);
                }
                tb.Text = "Type: " + dglst.SelectedItem.GetType().FullName;
            };
        }

        private void dglst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISerializationSurrogate surr = DataContext as ISerializationSurrogate;
            if (surr == null) return;
            bool b = dglst.SelectedItem == surr.getData;
            cc.Content = DataBinding.Generate_UserControl(dglst.SelectedItem);

        }

        private void lbProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.lbProfiles.IsMouseOver) return;

            var surr = DataContext as ISerializationSurrogate;
            if (surr == null) return;
            var selprofile = lbProfiles.SelectedItem;
            if (selprofile != null)
            {
                surr.LoadProfile(selprofile.ToString());
                cc.Content = DataBinding.Generate_UserControl(surr.getData);
            }
        }

        private void miRemoveClass_Click(object sender, RoutedEventArgs e)
        {
            SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            if (data == null) return;

            data.RemoveClass(data.currentClass);
        }

        private void miRemoveProfile_Click(object sender, RoutedEventArgs e)
        {
            var surr = DataContext as ISerializationSurrogate;
            if (surr == null || lbProfiles.SelectedItem == null) return;
            surr.DeleteProfile(lbProfiles.SelectedItem.ToString());
        }

        private void btnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            var surr = DataContext as ISerializationSurrogate;
            if (surr == null) return;
            surr.AddProfile(tbProfileName.Text);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var surr = DataContext as ISerializationSurrogate;
            if (surr == null) return;
            surr.ProfileNames.CollectionChanged += (sender2, e2) =>
            {
                surr.LoadProfile(surr.profileName);
                cc.Content = DataBinding.Generate_UserControl(surr.getData);
            };
        }
    }
}