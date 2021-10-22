using AutomationControls.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutomationControls.Controls
{
    public class SurrogateClassListBox : ListBox
    {

        public SurrogateClassListBox()
        {
            MinWidth = 100;

            this.ContextMenu = new ContextMenu();

            MenuItem mi = new MenuItem();
            mi.Header = "Remove Class";
            mi.Click += (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;

                var s = this.SelectedItem;
                if (s == null) return;

                data.RemoveClass(data.currentClass);
            };
            ContextMenu.Items.Add(mi);

            ListBox lbAddClass = new ListBox();
            lbAddClass.SelectionChanged += (sender, e) =>
            {
                if (!lbAddClass.IsMouseOver) return;    //Ignore system changes 

                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;
                var t = (Type)(sender as ListBox).SelectedItem;
                data.AddClass(t);
            };
            ContextMenu.Items.Add(new ScrollViewer() { Content = lbAddClass });

            this.ContextMenuOpening += (sender, e) =>
            {
                var types3 = Assembly.GetExecutingAssembly().GetTypes();
                // var types3 = Assembly.GetExecutingAssembly().GetTypes().Where( type => Attribute.IsDefined(type, typeof(DataProfileAttribute))).ToList();
                var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where Attribute.IsDefined(type, typeof(DataProfileAttribute))
                             select type).ToList();

                var vvvv = types.Where(x => x.Name.Contains("Codex", StringComparison.OrdinalIgnoreCase));

                lbAddClass.ItemsSource = types;
            };


            db.DataReadyEvent += (sender, e) =>
           {
               SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
               if (data == null) return;
               //  this.ItemsSource = data.classNames;
           };

            this.DataContextChanged += (sender, e) =>
            {
                Binding bnd = new Binding();
                bnd.Path = new System.Windows.PropertyPath("classNames");
                bnd.Mode = BindingMode.TwoWay;
                bnd.Source = DataContext;
                this.SetBinding(ListBox.ItemsSourceProperty, bnd);

                Binding bnd2 = new Binding();
                bnd2.Path = new System.Windows.PropertyPath("currentClass");
                bnd2.Mode = BindingMode.TwoWay;
                bnd2.Source = DataContext;
                this.SetBinding(ListBox.SelectedItemProperty, bnd2);
            };



        }
    }
}