using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutomationControls.Controls
{
    public class SurrogateProfileListBox : ListBox
    {

        public SurrogateProfileListBox()
        {
            MinWidth = 100;

            this.ContextMenu = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Remove Profile";
            mi.Click += (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;

                var s = this.SelectedItem;
                if (s == null) return;


                data.surr.DeleteProfile(data.currentProfile);
                db.sscdata.RemoveProfile(data.currentProfile);
            };

            ContextMenu.Items.Add(mi);

            /////////////////////////////////////////////
            MenuItem mi2 = new MenuItem();
            mi2.Header = "Rename Profile";
            mi2.Click += (sender, e) =>
            {
                DockPanel dp = new DockPanel();
                TextBox tb = new TextBox() { Width = 100 };
                Button btnRename = new Button() { Content = "Rename", Width = 75 };
                btnRename.Click += (sender2, e2) =>
                {
                    SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                    if (data == null) return;
                    var index = ((ISerializationSurrogate)data.surr).ProfileNames.IndexOf(data.currentProfile);
                    ((ISerializationSurrogate)data.surr).ProfileNames[index] = tb.Text;
                };
                dp.Children.Add(tb);
                dp.Children.Add(btnRename);
                dp.ToWindow();
            };
            ContextMenu.Items.Add(mi2);


            ////////////////////////////////////////
            DockPanel dp2 = new DockPanel();
            TextBox tb2 = new TextBox() { Width = 100 };
            Button btnAdd = new Button() { Content = "Add Profile", Width = 75 };
            btnAdd.Click += (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;
                data.AddProfile(tb2.Text);
            };
            dp2.Children.Add(tb2);
            dp2.Children.Add(btnAdd);
            ContextMenu.Items.Add(dp2);

            db.DataReadyEvent += (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null || data.surr == null) return;
                // this.ItemsSource = data.surr.ProfileNames;
            };

            this.DataContextChanged += (sender, e) =>
            {
                Binding bnd = new Binding();
                bnd.Path = new System.Windows.PropertyPath("currentProfile");
                bnd.Mode = BindingMode.TwoWay;
                bnd.Source = DataContext;
                this.SetBinding(ListBox.SelectedItemProperty, bnd);

                Binding bnd2 = new Binding();
                bnd2.Path = new System.Windows.PropertyPath("surr.ProfileNames");
                bnd2.Mode = BindingMode.TwoWay;
                bnd2.Source = DataContext;
                this.SetBinding(ListBox.ItemsSourceProperty, bnd2);
            };
        }
    }
}
