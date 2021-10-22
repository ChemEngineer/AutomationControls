using AutomationControls.Attributes;
using AutomationControls.BaseClasses;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ISerializationSurrogate = AutomationControls.Interfaces.ISerializationSurrogate;

namespace AutomationControls
{
    /// <summary>
    /// Interaction logic for SerializationSurrogateControl.xaml
    /// </summary>
    [Serializable]
    public partial class SerializationSurrogateControl : UserControl
    {

        public SerializationSurrogateControl()
        {
            InitializeComponent();

            db.DataReadyEvent += (sender, e) =>
            {
                GetSerializationSurrogateControl();
            };

            Application.Current.MainWindow.ContentRendered += async (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;

                ObservableCollection<object> lst = new ObservableCollection<object>();
                data.lstSurrogates.ForEach(x => lst.Add(x));
                foreach (ISerializationSurrogate v in lst)
                {
                    try
                    {
                        var res = v.getList.Cast<object>().Where(x => x is DeviceBase).Select(y => y);
                        DeviceBase devbase = (v.getData) as DeviceBase;
                        if (devbase != null && devbase.connectAtStartup)
                        {
                            await devbase.comm.OpenCommunicationChannelAsync();
                        }
                    }
                    catch { continue; }
                }
            };
            GetSerializationSurrogateControl();
        }

        private void AddProfile(string profileName)
        {
            SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            if (data == null) return;
            data.AddProfile(profileName);

            GetSerializationSurrogateControl();
        }

        #region Control Event Handlers

        private void Button_Serialize(object sender, RoutedEventArgs e)
        {
            //SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            //if (data == null) return;
            db.sscdata.Serialize();
        }


        private void Button_Deserialize(object sender, RoutedEventArgs e)
        {
            //SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            // if (data == null) return;

            db.sscdata.Deserialize();
            GetSerializationSurrogateControl();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            if (data == null || data.lstSurrogates.Count == 0) return;

            if (lbClasses.IsMouseOver)
                data.LoadClass(data.currentClass);
            if (lbProfiles.IsMouseOver)
                data.LoadProfile(data.currentProfile);
            GetSerializationSurrogateControl();

        }

        #endregion


        public UserControl GetSerializationSurrogateControl()
        {
            UserControl uc = new UserControl();
            SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
            if (data == null || data.lstSurrogates.Count == 0) return uc;

            tc.Items.Clear();
            foreach (var item in data.surr.getList)
            {
                if (item is IProvideUserControls)
                {
                    foreach (var v in GetUserControls(item))
                    {
                        if ((v as UserControl) != null)
                        {
                            TabItem t = new TabItem() { Content = v, Header = (v as UserControl).GetType().Name };
                            tc.Items.Add(t);
                        }
                    };
                }
            }

            var ctls = data.surr.GetUserControls();
            for (int i = ctls.Count() - 1; i >= 0; i--)
            {
                var su = ctls[i];
                TabItem t = new TabItem() { Content = su, Header = (su as UserControl).GetType().Name };
                tc.Items.Add(t);
            }
            uc.Content = tc;
            return uc;
        }

        public UserControl[] GetUserControls(object obj)
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])obj.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = obj;
            }
            return lst.ToArray();
        }

    }
}
