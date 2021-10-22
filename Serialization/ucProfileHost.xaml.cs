using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using AutomationControls.WPF;
using AutomationControls.WPF.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomationControls.Serialization
{
    /// <summary>
    /// Interaction logic for ucProfileHost.xaml
    /// </summary>
    public partial class ucProfileHost : UserControl, ISupportProfiles
    {

        #region PropertyChanged Pattern

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion


        private IList _lst;
        public IList lst
        {
            get { return _lst; }
            set
            {
                _lst = value;
                dglst2.ItemsSource = _lst;
                OnPropertyChanged("lst");
            }
        }

        private object _data;
        public object data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("data");
            }
        }

        private ObservableCollection<ISupportProfiles> profileControls = new ObservableCollection<ISupportProfiles>();
        private ObservableCollection<string> profileNames = new ObservableCollection<string>();


        private String _profileName = "new";
        public String profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("profileName");
            }
        }

        private String _serializationPath = @"C:\Saved Data";
        public String serializationPath
        {
            get { return _serializationPath; }
            set
            {
                _serializationPath = value;
                OnPropertyChanged("serializationPath");
            }
        }

        public ucProfileHost()
        {
            InitializeComponent();
        }

        public void Add(ISupportProfiles p)
        {
            profileControls.Add(p);
            p.AddRemove += (sender, e) => { GetProfileNames(); };
            GetProfileNames();
            lbHosts.Items.Add(new ListBoxItem() { Content = p.ToString().Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last(), ToolTip = p.ToString(), Tag = p });
            lbHosts.SelectedItem = lbHosts.Items[0];

            var lstlst = p.getList;
            var datalst = p.getData;
            var lst = datalst.GetType().GetProperty("lst").GetValue(datalst) as IList;
            var data = datalst.GetType().GetProperty("data").GetValue(datalst);
            dglst2.ItemsSource = lst;
            dglst2.SelectedItem = data;
            if (lbClasses.Items.Count > 0)
                lbClasses.SelectedItem = lbClasses.Items[0];
        }

        public void Remove(ISupportProfiles p)
        {
            if (!profileControls.Contains(p)) return;
            profileControls.Remove(p);
        }

        public override string ToString()
        {
            return GetProfileNames().Aggregate((x, y) => x + Environment.NewLine + y);
        }

        #region ISupportProfiles Members

        public event EventHandler ProfileChanged;
        public event EventHandler AddRemove;

        protected virtual void OnProfileChanged(EventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior
        {
            EventHandler handler = ProfileChanged; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            if (handler != null) { handler(this, e); }
        }

        public void ChangeProfileName(string name)
        {
            string oldprofile = profileName;
            foreach (var v in profileControls)
            {
                v.ChangeProfileName(name);
                v.DeleteProfile(profileName);
                //v.profileName = name;
            }
            profileName = name;
            GetProfileNames();
            OnProfileChanged(new EventArgs());
        }

        public void DeleteProfile(string name)
        {
            foreach (var v in profileControls)
            {
                v.DeleteProfile(name);
            }
            OnProfileChanged(new EventArgs());
            GetProfileNames();
        }

        public void AddProfile(string name)
        {
            foreach (var v in profileControls)
            {
                v.AddProfile(name);
            }
            this.profileName = name;
            OnProfileChanged(new EventArgs());
            GetProfileNames();
        }

        public void LoadProfile(string name)
        {
            foreach (var v in profileControls) { v.LoadProfile(name); }
            this.profileName = name;
            OnProfileChanged(new EventArgs());
            GetProfileNames();

            dp2.Children.Clear();

            var lbi = lbHosts.SelectedItem as ListBoxItem;
            var u = lbi.Tag as UserControl;
            var t = lbi.Tag as ISupportProfiles;
            if (t != null)
            {
                var dat = t.getData;
                lst = dat.GetType().GetProperty("lst").GetValue(dat) as IList;
                dglst2.ItemsSource = lst;
                data = dat.GetType().GetProperty("data").GetValue(dat);
                dglst2.SelectedItem = data;

                if (u != null)
                {
                    UserControl uc = Activator.CreateInstance(u.GetType()) as UserControl;
                    if (uc != null)
                    {
                        uc.DataContext = u.DataContext;
                        dp2.Children.Add(uc);
                    }
                }

                if (lst == null) return;
                foreach (var p in lst)
                {
                    try
                    {
                        var pname = p.GetType().GetProperty("profileName");
                        var value = pname.GetValue(p);
                        if (value.ToString() == profileName)
                        {

                            (data as INotifyPropertyChanged).PropertyChanged += (sender, e) =>
                            {
                                if (e.PropertyName == "data") { dglst2.SelectedItem = sender; }
                                if (e.PropertyName == "lst")
                                {
                                    dglst2.ItemsSource = (sender as IList);
                                }
                            };

                            //dglst2.ItemsSource = lst;
                            dp.Children.Clear();
                            dp.Children.Add(DataBinding.Generate_UserControl(data, BindingMode.TwoWay));

                            dp2.Children.Clear();
                            var v = lst.GetType().GetMethod("CreateTreeView");
                            var v2 = lst.GetType().GetMethod("CreateDataGrid");
                            if (v == null) return;
                            //v.Invoke(l, null);
                            dp2.Children.Add(DataBinding.Generate_UserControl(lst, BindingMode.TwoWay));
                            dp2.Children.Add(v.Invoke(lst, null) as UIElement);
                            dp2.Children.Add(v2.Invoke(lst, null) as UIElement);

                            break;
                        }
                    }
                    catch { }
                }
            };
        }

        public List<string> GetProfileNames()
        {
            List<string> lst = new List<string>();
            foreach (ISupportProfiles v in profileControls) { lst.AddRange(v.GetProfileNames()); }
            lst = new List<string>(lst.Where(x => !string.IsNullOrEmpty(x)).Select(x => x).Distinct());
            lbClasses.ItemsSource = lst;
            return lst;
        }

        public bool SerializeXML()
        {
            try
            {
                foreach (ISupportProfiles v in profileControls) { v.Serialize(); }
                OnProfileChanged(new EventArgs());
                return true;
            }
            catch { return false; }
        }

        public bool DeserializeXML()
        {
            try
            {
                foreach (ISupportProfiles v in profileControls) { v.Deserialize(); }
                OnProfileChanged(new EventArgs());
                return true;
            }
            catch { return false; }
        }

        public IList getList
        {
            get { return null; }
            set { var a = value; }
        }

        public object getData
        {
            set { var a = value; }
            get { return null; }
        }

        #endregion

        public object Get(Type type)
        {
            var res = profileControls.Where(x => x.GetType() == type);
            if (res.Count() > 0) return res.ToArray()[0];
            else return null;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            TextInputMessageBox msg = new TextInputMessageBox() { Background = Brushes.Black, Foreground = Brushes.White };
            this.ApplyContentAdorner(msg); msg.Show("Change Profile Name");
            msg.RaiseResponseReadyEvent += (sender2, e2) => { ChangeProfileName(e2.data); this.ClearAdorners(); };
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteProfile(profileName);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TextInputMessageBox msg = new TextInputMessageBox() { Width = this.ActualWidth, Opacity = 1, Background = Brushes.Green };
            this.ApplyContentAdorner(msg); msg.Show("Add Profile");
            msg.RaiseResponseReadyEvent += (sender2, e2) =>
            {
                AddProfile(e2.data);
                this.ClearAdorners();
                OnProfileChanged(new EventArgs());
            };
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SerializeXML();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            DeserializeXML();
        }

        private void lbHosts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                if (item.Tag != null && item.Tag.GetType() == typeof(IList)) return;
                lst = item.Tag as IList;
                if (lbClasses.SelectedItem == null)
                {
                    lbClasses.SelectedItem = lbClasses.Items[0];
                }
                LoadProfile(lbClasses.SelectedItem.ToString());

                // var info = dglst.SelectedItem.GetType().GetProperty("lst");
                // dglst2.ItemsSource = info.GetValue(lbClasses.SelectedItem) as IList;
            }
        }

        private void lbHosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var lbi = lbHosts.SelectedItem as ListBoxItem;
            dp2.Children.Clear();
            var c = lbi.Content;
            var t = lbi.Tag as ISupportProfiles;
            if (t != null)
            {
                var dat = t.getData;
                lst = dat.GetType().GetProperty("lst").GetValue(dat) as IList;
                dglst2.ItemsSource = lst;
                data = dat.GetType().GetProperty("data").GetValue(dat);
                dglst2.SelectedItem = data;
            }
            var u = (lbHosts.SelectedItem as ListBoxItem).Tag as UserControl;
            if (u != null)
            {
                UserControl uc = Activator.CreateInstance(u.GetType()) as UserControl;
                if (uc != null)
                {
                    uc.DataContext = u.DataContext;
                    dp2.Children.Add(uc);
                }
            }
        }

        private void lbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem == null) return;

            profileName = lb.SelectedItem.ToString();
            LoadProfile(profileName);

        }

        private void tbSerialization_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (spSerialization.ActualHeight == 0) spSerialization.AnimateHeight(0, 45, TimeSpan.FromMilliseconds(250));
            else spSerialization.AnimateHeight(spSerialization.ActualHeight, 0, TimeSpan.FromMilliseconds(250));
        }

        public string Serialize(SerializationType serializeType = SerializationType.XML)
        {
            foreach (ISupportProfiles v in profileControls)
            {
                try
                {
                    string path = serializationPath + @"\";
                    v.GetType().ToString().Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => path += x + "\\");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    switch (serializeType)
                    {
                        case SerializationType.JSON:
                            path += v.GetType().ToString() + ".json";
                            break;
                        case SerializationType.XML:
                            path += v.GetType().ToString() + ".xml";
                            break;
                        default:
                            break;
                    }

                    v.Serialize(serializeType);
                    //var data = v.getData.GetType().GetProperty("data");
                    //var l = v.getData.GetType().GetProperty("lst");
                    //var lst = l.GetValue(v.getData);
                    //if(lst.GetType().IsSerializable)
                    //{
                    //    Type typeArg = lst.GetType();
                    //    Type genericClass = typeof(Serializer<>);
                    //    Type constructedClass = genericClass.MakeGenericType(typeArg);
                    //    object created = Activator.CreateInstance(constructedClass, new object[] { lst });
                    //    path += v.GetType().ToString() + ".json";
                    //    var mi = created.GetType().GetMethod("ToJSON").Invoke(created, new object[] { path });
                    //}
                    //else 
                    //{
                    //    path += v.GetType().ToString() + ".xml";
                    //    v.Serialize(SerializationType.XML);     
                    //}
                    //v.Serialize(SerializationType.XML); 
                }
                catch { continue; }
            }
            OnProfileChanged(new EventArgs());
            return "";
        }

        public bool Deserialize(SerializationType serializeType = SerializationType.XML)
        {
            try
            {
                for (int i = 0; i < profileControls.Count(); i++)
                {
                    try
                    {
                        string path = serializationPath + @"\";
                        profileControls[i].GetType().ToString().Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => path += x + "\\");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                        switch (serializeType)
                        {
                            case SerializationType.JSON:
                                path += profileControls[i].GetType().ToString() + ".json";
                                break;
                            case SerializationType.XML:
                                path += profileControls[i].GetType().ToString() + ".xml";
                                break;
                            default:
                                break;
                        }

                        profileControls[i].Deserialize(serializeType);
                        //var data = profileControls[i].getData.GetType().GetProperty("data");
                        //var l = profileControls[i].getData.GetType().GetProperty("lst");
                        //var lst = l.GetValue(profileControls[i].getData);
                        //if(lst.GetType().IsSerializable)
                        //{
                        //    Type typeArg = lst.GetType();
                        //    Type genericClass = typeof(Serializer<>);
                        //    Type constructedClass = genericClass.MakeGenericType(typeArg);
                        //    object created = Activator.CreateInstance(constructedClass, new object[] { lst });
                        //    var mi = created.GetType().GetMethod("FromJSON").Invoke(created, new object[] { path });

                        //   lst  = (IList)mi;
                        //}
                        //else { profileControls[i].Deserialize(SerializationType.XML); }
                    }
                    catch { continue; }
                }
                //foreach(ISupportProfiles v in profileControls)
                //{
                //    try
                //    {

                //      v.Deserialize(SerializationType.XML);
                //    } catch { continue; }
                //}      
                OnProfileChanged(new EventArgs());
                return true;
            }
            catch { return false; }
        }


    }
}