using AutomationControls.Extensions;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AutomationControls.WPF
{
    public partial class DataBinding
    {

        public static TreeViewItem Generate_TreeViewItem(Object obj)
        {
            if (obj == null) { return null; }


            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            TreeViewItem tvi = new TreeViewItem();
            tvi.Header = obj.GetType().ToString(); tvi.Foreground = Brushes.Green;
            for (int i = 0; i <= pi.GetUpperBound(0); i++)
            {
                try
                {
                    Label lbl1 = new Label() { Content = pi[i].Name.PadRight(40), Padding = new Thickness(0), Margin = new Thickness(0) };
                    Label lbl2 = new Label() { Content = pi[i].GetValue(obj).ToString(), HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left, Padding = new Thickness(0), Margin = new Thickness(0) };
                    StackPanel sp = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                    sp.Children.Add(lbl1); sp.Children.Add(lbl2);

                    tvi.Items.Add(sp);
                }
                catch (Exception) { continue; }
            }
            return tvi;
        }

        public static UserControl Generate_TreeView(Object obj)
        {
            if (obj == null) { return null; }
            UserControl ret = new UserControl();

            TreeView tv = new TreeView();

            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            TreeViewItem tvi = new TreeViewItem();
            Grid grd = new Grid(); grd.Margin = new Thickness(0);
            grd.ColumnDefinitions.Add(new ColumnDefinition());
            grd.ColumnDefinitions.Add(new ColumnDefinition());
            tvi.Items.Add(grd);
            tvi.Header = obj.GetType().ToString(); tvi.Foreground = Brushes.Green;
            for (int i = 0; i <= pi.GetUpperBound(0); i++)
            {
                try
                {
                    var val = pi[i].GetValue(obj);
                    Type t = val.GetType();
                    if (t.IsClass && !t.IsPrimitive && t != typeof(Decimal) && t != typeof(String))
                    {
                        TreeViewItem tvi2 = Generate_TreeViewItem(val);
                        tvi.Items.Add(tvi2);
                    }
                    else
                    {
                        Label lblname = new Label() { Content = pi[i].Name, HorizontalAlignment = HorizontalAlignment.Right, Padding = new Thickness(5, 0, 5, 0), Margin = new Thickness(0) };
                        Label lblval = new Label() { Content = pi[i].GetValue(obj), HorizontalAlignment = HorizontalAlignment.Left, Padding = new Thickness(5, 0, 5, 0), Margin = new Thickness(0) };
                        grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                        grd.Children.Add(lblname); Grid.SetRow(lblname, grd.RowDefinitions.Count() - 1); Grid.SetColumn(lblname, 0);
                        grd.Children.Add(lblval); Grid.SetRow(lblval, grd.RowDefinitions.Count() - 1); Grid.SetColumn(lblval, 1);
                    }
                }
                catch (Exception) { continue; }
            }
            tv.Items.Add(tvi);
            // ret.Content = new ScrollViewer() { Content = grd, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            ret.Content = tv;
            tv.ExpandAll();
            return ret;
        }

        public static UserControl Generate_UserControl(Object obj, BindingMode bindingMode = BindingMode.OneWay)
        {
            if (obj == null) { return null; }

            var interfaces = obj.GetType().GetInterfaces();
            var result = from a in obj.GetType().GetInterfaces()
                         where a == typeof(IList)
                         select a.Name;

            if (result.Count() > 0)
            {
                return Generate_UserControl(obj as IList);
            }
            UserControl ret = new UserControl();
            Grid grd = new Grid();


            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            //   PropertyInfo[] pi = obj.GetType().GetProperties();
            for (int i = 0; i <= pi.GetUpperBound(0); i++)
            {
                var prop = pi[i].GetValue(obj);
                Type type = pi[i].PropertyType;
                try
                {
                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });
                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });

                    IList lst = prop as IList;
                    if (lst != null)
                    {
                        //ComboBox cb = new ComboBox() { ItemsSource = lst };
                        //grd.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });
                        //grd.Children.Add(cb); 
                        //Grid.SetColumn(cb, 0); Grid.SetRow(cb, i);
                        DataGrid cb = new DataGrid() { ItemsSource = lst };
                        grd.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });
                        grd.Children.Add(cb);
                        Grid.SetColumn(cb, 0); Grid.SetRow(cb, i); Grid.SetColumnSpan(cb, 2);
                    }
                    else
                    {

                        Label lbl = new Label()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center
                        };
                        TextBox tb = new TextBox()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center
                        };
                        grd.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });

                        Binding bnd = new Binding();
                        bnd.Path = new System.Windows.PropertyPath(pi[i].Name);
                        bnd.Mode = bindingMode;
                        bnd.Source = obj;
                        tb.SetBinding(TextBox.TextProperty, bnd);

                        object res = pi[i].GetValue(obj);
                        if (res != null) { tb.Text = res.ToString(); }
                        else { tb.Text = "N/A"; }
                        lbl.Content = pi[i].Name;
                        grd.Children.Add(lbl); grd.Children.Add(tb);
                        Grid.SetColumn(lbl, 0); Grid.SetRow(lbl, i);
                        Grid.SetColumn(tb, 1); Grid.SetRow(tb, i);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            ScrollViewer sv = new ScrollViewer();
            StackPanel sp = new StackPanel();
            sv.Content = sp;
            sp.Children.Add(grd);
            ret.Content = sv;
            ret.DataContext = obj;
            return ret;
        }

        public static UserControl Generate_UserControl(IList lstObj)
        {
            if (lstObj == null) { return null; }

            //for(int j = 0; j < lstObj.Count; j++) { sp.Children.Add(Generate_UserControl(lstObj[j])); }


            return new UserControl() { Content = new DataGrid() { ItemsSource = lstObj } };
        }

        public UserControl GenerateUserControl(Object obj, BindingMode bindingMode = BindingMode.OneWay)
        {
            if (obj == null) { return null; }

            var result = from a in obj.GetType().GetInterfaces()
                         where a == typeof(IList)
                         select a.Name;

            if (result.Count() > 0)
            {
                return Generate_UserControl(obj as IList);
            }
            UserControl ret = new UserControl();
            Grid grd = new Grid();


            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            //   PropertyInfo[] pi = obj.GetType().GetProperties();
            for (int i = 0; i <= pi.GetUpperBound(0); i++)
            {
                try
                {
                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });
                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });


                    Label lbl = new Label()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };
                    TextBox tb = new TextBox()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };
                    grd.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto) });

                    Binding bnd = new Binding();
                    bnd.Path = new System.Windows.PropertyPath(pi[i].Name);
                    bnd.Mode = bindingMode;
                    bnd.Source = obj;
                    tb.SetBinding(TextBox.TextProperty, bnd);

                    object res = pi[i].GetValue(obj);
                    if (res != null) { tb.Text = res.ToString(); }
                    else { tb.Text = "N/A"; }
                    lbl.Content = pi[i].Name;
                    grd.Children.Add(lbl); grd.Children.Add(tb);
                    Grid.SetColumn(lbl, 0); Grid.SetRow(lbl, i);
                    Grid.SetColumn(tb, 1); Grid.SetRow(tb, i);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            // ret.Content = new ScrollViewer() { Content = grd, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            ret.Content = grd;
            ret.DataContext = obj;
            return ret;
        }

    }
}
