using AutomationControls.Attributes;
using AutomationControls.Controllers.DataClasses;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Devices.LED.WS2812
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.Devices.LED.WS2812.WS2812DigitalInputControl))]
    public class DigitalInputControlData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {



        private DigitalChannel _digitalChannel;
        public DigitalChannel digitalChannel
        {
            get { return _digitalChannel; }
            set
            {
                _digitalChannel = value;
                OnPropertyChanged("digitalChannel");
            }
        }


        public DigitalInputControlData()
        {
            Initialize();
        }

        private void Initialize() { }

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


        //private ObservableCollection<IDigitalChannel> _lstDigitalChannels;
        //public ObservableCollection<IDigitalChannel> lstDigitalChannels
        //{
        //    get
        //    {

        //        return new ObservableCollection<IDigitalChannel>(db.lstSerialize.Where(x => x is IDigitalChannel).Select(x => (IDigitalChannel)x));

        //        //return new ObservableCollection<Type>(AppDomain.CurrentDomain.GetAssemblies()
        //        //    .SelectMany(s => s.GetTypes())
        //        //    .Where(p => typeof(IDigitalChannel).IsAssignableFrom(p)));
        //    }
        //    set
        //    {
        //        _lstDigitalChannels = value;
        //        OnPropertyChanged("lstDigitalChannels");
        //    }
        //}

        private Type _DigitalChannelSelection;
        public Type DigitalChannelSelection
        {
            get { return _DigitalChannelSelection; }
            set
            {
                if (value == _DigitalChannelSelection) return;
                var typ = value.GetType();

                var obj = Activator.CreateInstance(value);
                digitalChannel = obj as DigitalChannel;

                IProvideUserControls ctrls = obj as IProvideUserControls;
                if (ctrls == null) return;

                _DigitalChannelSelection = value;
                OnPropertyChanged("DigitalChannelSelection");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // info.AddValue("lstDigitalChannels", _lstDigitalChannels, typeof(ObservableCollection<string>));
            // info.AddValue("DigitalChannelSelection", _DigitalChannelSelection, typeof(string));
        }
        public DigitalInputControlData(SerializationInfo info, StreamingContext context)
        {
            // _lstDigitalChannels = (ObservableCollection<string>)info.GetValue("lstDigitalChannels", typeof(ObservableCollection<string>));
            // _DigitalChannelSelection = (string)info.GetValue("DigitalChannelSelection", typeof(string));
        }


        #region IProvideUserControl

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

    }

    [Serializable]
    public class DigitalInputControlDataList : ObservableCollection<DigitalInputControlData>
    {

    }
}

