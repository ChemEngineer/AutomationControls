using AutomationControls.BaseClasses;
using AutomationControls.WPF;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for WS2812CompositeControl.xaml
    /// </summary>
    public partial class WS2812CompositeControl : UserControl
    {

        public WS2812CompositeControl()
        {
            InitializeComponent();


        }


        TimeSpan ts = TimeSpan.FromMilliseconds(0);
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WS2812LED data = DataContext as WS2812LED;
            if (data != null)
            {
                dp.Children.Add(DataBinding.Generate_UserControl(data, BindingMode.TwoWay));
            }

            // Send to ALL WS2812Data 
            DateTime lastUpdated = DateTime.Now;
            data.CommandExecutedEvent += async (sender2, e2) =>
            {
                if (!(bool)cbSendToAll.IsChecked)
                    return;

                TimeSpan tsNow = DateTime.Now - lastUpdated;
                if (tsNow < ts)
                    return;

                lastUpdated = DateTime.Now;
                foreach (DictionaryEntry v in (IDictionary)db.sscdata.surr.getListList)
                {
                    var res = (DeviceBase)((ObservableCollection<WS2812LED>)v.Value)[0];
                    await res.comm.SendBytesAsync(data.Create_Serial_Command());
                }
            };
        }
    }
}
