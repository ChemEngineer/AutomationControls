using AutomationControls.Attributes;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.communication.mqtt
{



    [Serializable]
    [DataProfile(typeof(AutomationControls.communication.mqtt.AsrTokenDataControl))]
    public class AsrTokenData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public AsrTokenData()
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


        private string _value;
        public string value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("value");
            }
        }

        private double _confidence;
        public double confidence
        {
            get { return _confidence; }
            set
            {
                _confidence = value;
                OnPropertyChanged("confidence");
            }
        }

        private int _range_start;
        public int range_start
        {
            get { return _range_start; }
            set
            {
                _range_start = value;
                OnPropertyChanged("range_start");
            }
        }

        private int _range_end;
        public int range_end
        {
            get { return _range_end; }
            set
            {
                _range_end = value;
                OnPropertyChanged("range_end");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", _value, typeof(string));
            info.AddValue("confidence", _confidence, typeof(double));
            info.AddValue("range_start", _range_start, typeof(int));
            info.AddValue("range_end", _range_end, typeof(int));
        }
        public AsrTokenData(SerializationInfo info, StreamingContext context)
        {
            _value = (string)info.GetValue("value", typeof(string));
            _confidence = (double)info.GetValue("confidence", typeof(double));
            _range_start = (int)info.GetValue("range_start", typeof(int));
            _range_end = (int)info.GetValue("range_end", typeof(int));
        }


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

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<AsrTokenData> ser = new AutomationControls.Serialization.Serializer<AsrTokenData>(this);
            ser.ToJSON(s);
        }

        public static AsrTokenData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<AsrTokenData> ser = new AutomationControls.Serialization.Serializer<AsrTokenData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.communication.mqtt.AsrTokenDataListControl))]
    public class AsrTokenDataList : ObservableCollection<AsrTokenData>, ISerializable, IProvideUserControls
    {
        public AsrTokenDataList() { }

        public AsrTokenDataList(IEnumerable<AsrTokenData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public AsrTokenDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<AsrTokenDataList> ser = new AutomationControls.Serialization.Serializer<AsrTokenDataList>(this);
            ser.ToJSON(s);
        }

        public static AsrTokenDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<AsrTokenDataList> ser = new AutomationControls.Serialization.Serializer<AsrTokenDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

