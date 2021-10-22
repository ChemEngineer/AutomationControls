using AutomationControls.BaseClasses;
using AutomationControls.Controllers.DataClasses;
using AutomationControls.Interfaces;
using AutomationControls.WPF;
using System.Linq;
using System.Windows.Controls;

namespace AutomationControls.Devices.LED.WS2812
{

    public partial class WS2812DigitalInputControl : UserControl
    {



        public WS2812DigitalInputControl() : base()
        {
            InitializeComponent();
            db.DataReadyEvent += (sender, e) =>
            {
                var surrogates = db.sscdata.lstSurrogates.Where(x => ((ISerializationSurrogate)x).getData.GetType().Name == DataContext.GetType().Name);
                if (surrogates.Count() > 0)
                {
                    ISerializationSurrogate surr = ((ISerializationSurrogate)surrogates.ToArray()[0]);
                    if (surr != null)
                    {
                        this.DataContext = surr.getList[0];
                    }
                }
            };


        }

        private void UAutomationControlsate()
        {
            WS2812DigitalInput data = DataContext as WS2812DigitalInput;
            if (data == null) return;

            tc.Items.Clear();
            data.led.GetUserControls().ToList().ForEach(x => tc.Items.Add(new TabItem() { Content = x, Header = x.GetType().Name }));
            if (data.digitalChannels != null)
            {
                DigitalChannelListControl lstctrl = data.digitalChannels.GetUserControls().ToArray()[0] as DigitalChannelListControl;

                dAutomationControlsigitalChannels.Children.Clear();
                dAutomationControlsigitalChannels.Children.Add(lstctrl);


                // tc.Items.Add(new TabItem() { Content = lstctrl, Header = lstctrl.DataContext.GetType().Name });
                // lstctrl.ToList().ForEach(x => tc.Items.Add(new TabItem() { Content = x, Header = x.GetType().Name }));
            }
        }



        private void DigitalChannelsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WS2812DigitalInput data = DataContext as WS2812DigitalInput;
            if (data == null) return;
            ComboBox cb = sender as ComboBox;

            if (e.AddedItems.Count == 0) return;
            ComboBoxItem cbi = e.AddedItems[0] as ComboBoxItem;
            var tag = cbi.Tag;

            var classname = tag.GetType().Name;
            var res = db.sscdata.lstSurrogates.Where(x => ((ISerializationSurrogate)x).getData.GetType().Name == classname).Select(x => x);
            if (res.Count() > 0)
            {
                ISerializationSurrogate surr = (ISerializationSurrogate)res.ToArray()[0];

                DeviceBase devbase = surr.getData as DeviceBase;
                if (surr != null)
                {

                }
                IDigitalChannels dcs = surr.getData as IDigitalChannels;
                if (dcs != null)
                {
                    data.digitalChannels = dcs.DigitalChannels;
                    // data.digitalChannel = surr.getData as DigitalChannel; 
                }
            }
            UAutomationControlsate();
            // db.OnDataReadyEvent(new db.DataReadyEventArgs() { Message = this.GetType().FullName });
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var newval = e.NewValue as WS2812DigitalInput;
            if (newval == null) return;
            // dg.ItemsSource = newval.digitalChannels;
            //dg.SelectedItem = newval.digitalChannel;
            UAutomationControlsate();
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WS2812DigitalInput data = DataContext as WS2812DigitalInput;
            if (data == null) return;

            dAutomationControlsigital.Children.Clear();
            if ((sender as DataGrid).SelectedItem != null)
                dAutomationControlsigital.Children.Add(DataBinding.Generate_UserControl((sender as DataGrid).SelectedItem));

            UAutomationControlsate();
        }

    }
}