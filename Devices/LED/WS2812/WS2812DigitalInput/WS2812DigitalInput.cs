using AutomationControls.Attributes;
using AutomationControls.BaseClasses;
using AutomationControls.Controllers.DataClasses;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Devices.LED.WS2812
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Devices.LED.WS2812.WS2812DigitalInputControl))]
    public class WS2812DigitalInput : DeviceBase, INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public WS2812DigitalInput()
        {
            this.Initialize();

        }


        private void Initialize()
        {
            db.DataReadyEvent += (sender, e) =>
            {
                // Find surrogate in list   /////////////////
                var surrogates = db.sscdata.lstSurrogates.Where(x => ((AutomationControls.Interfaces.ISerializationSurrogate)x).getData.GetType() == typeof(DigitalChannel));
                if (surrogates.Count() > 0)
                {
                    var surr = surrogates.ToArray()[0] as AutomationControls.Interfaces.ISerializationSurrogate;
                    var dcs = surr.getList[0] as IDigitalChannels;
                    if (dcs != null)
                    {
                        digitalChannels = dcs.DigitalChannels;
                        var chans = digitalChannels.Where(x => x.PinDesignation == digitalChannel.PinDesignation);
                        if (chans.Count() > 0)
                        {
                            var chan = chans.ToArray()[0] as DigitalChannel;
                            if (chan != null)
                            {
                                digitalChannel = chan;
                            }
                        }
                    }
                }
            };
        }



        private DigitalChannel _digitalChannel = new DigitalChannel();
        public DigitalChannel digitalChannel
        {
            get { return _digitalChannel; }
            set
            {
                _digitalChannel = value;
                OnPropertyChanged("digitalChannel");
            }
        }

        private DigitalChannelList _digitalChannels = new DigitalChannelList();
        public DigitalChannelList digitalChannels
        {
            get { return _digitalChannels; }
            set
            {
                _digitalChannels = value;
                OnPropertyChanged("digitalChannels");
            }
        }

        private WS2812LED _led = new WS2812LED();
        public WS2812LED led
        {
            get { return _led; }
            set
            {
                _led = value;
                OnPropertyChanged("led");
            }
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("led", _led, typeof(WS2812LED));
            info.AddValue("digitalChannel", _digitalChannel, typeof(DigitalChannel));
            info.AddValue("digitalChannels", _digitalChannels, typeof(DigitalChannelList));
        }

        public WS2812DigitalInput(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            led = (WS2812LED)info.GetValue("led", typeof(WS2812LED));
            digitalChannel = (DigitalChannel)info.GetValue("digitalChannel", typeof(DigitalChannel));
            digitalChannels = (DigitalChannelList)info.GetValue("digitalChannels", typeof(DigitalChannelList));



            Initialize();

        }


        #region IProvideUserControls

        public new UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            lst.AddRange(base.GetUserControls());

            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), true);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }

            //foreach(var pi in this.GetType().GetProperties())
            //{
            //    foreach(var v in pi.PropertyType.GetCustomAttributes(true))
            //    {
            //        DataProfileAttribute attr = v as DataProfileAttribute;
            //        if(attr != null)
            //        {
            //            var type = attr.ClassName;
            //            cc = (UserControl)Activator.CreateInstance(type);
            //            if(cc == null) continue;
            //            lst.Add(cc);
            //            cc.DataContext = pi.GetValue(this);
            //        }
            //    }
            //}
            return lst.ToArray();
        }

        #endregion
    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.Devices.LED.WS2812.WS2812DigitalInputListControl))]
    public class WS2812DigitalInputList : ObservableCollection<WS2812DigitalInput>, ISerializable, IProvideUserControls
    {
        public WS2812DigitalInputList() { }

        #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if (cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }
            return lst.ToArray();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lst", this, this.GetType());
        }

        public WS2812DigitalInputList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}
