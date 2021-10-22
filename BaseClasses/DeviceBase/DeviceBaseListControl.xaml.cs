using AutomationControls.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace AutomationControls.BaseClasses
{

    public partial class DeviceBaseListControl : UserControl
    {

        public DeviceBaseListControl() : base()
        {
            InitializeComponent();
            dg.Items.Clear();
            dg.ContextMenu = new System.Windows.Controls.ContextMenu();

            dg.ContextMenuOpening += (sender, e) =>
            {
                dg.ContextMenu.Items.Clear();
                var type = typeof(DeviceBase);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p));

                foreach (var v in types)
                {
                    MenuItem mi = new MenuItem() { Header = v.Name };
                    dg.ContextMenu.Items.Add(mi);

                    mi.Click += (sender2, e2) =>
                    {
                        var res = (DeviceBase)Activator.CreateInstance(v);
                        GetItemsSource().Add(res);

                    };
                }
            };

            this.DataContext = new ObservableCollection<DeviceBase>()
            {

            };

            dg.SelectionChanged += (sender, e) =>
            {

                if (dg.SelectedItem == null || dg.SelectedItem.ToString() == "{NewItemPlaceholder}") return;
                var s = dg.SelectedItem.ToString();
                var res = (DeviceBase)dg.SelectedItem;
                sp.Children.Clear();
                res.GetUserControls().ForEach(x => sp.Children.Add(x));


            };


        }


        public DeviceBaseList GetItemsSource()
        {

            return dg.ItemsSource as DeviceBaseList;
        }
    }
}