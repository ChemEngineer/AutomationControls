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
    [DataProfile(typeof(AutomationControls.communication.mqtt.HermesIntentDataControl))]
    public class HermesIntentData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public HermesIntentData()
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


        private string _intentName;
        public string intentName
        {
            get { return _intentName; }
            set
            {
                _intentName = value;
                OnPropertyChanged("intentName");
            }
        }

        private double _probability;
        public double probability
        {
            get { return _probability; }
            set
            {
                _probability = value;
                OnPropertyChanged("probability");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("intentName", _intentName, typeof(string));
            info.AddValue("probability", _probability, typeof(double));
        }
        public HermesIntentData(SerializationInfo info, StreamingContext context)
        {
            _intentName = (string)info.GetValue("intentName", typeof(string));
            _probability = (double)info.GetValue("probability", typeof(double));
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
            AutomationControls.Serialization.Serializer<HermesIntentData> ser = new AutomationControls.Serialization.Serializer<HermesIntentData>(this);
            ser.ToJSON(s);
        }

        public static HermesIntentData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentData> ser = new AutomationControls.Serialization.Serializer<HermesIntentData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.communication.mqtt.HermesIntentDataListControl))]
    public class HermesIntentDataList : ObservableCollection<HermesIntentData>, ISerializable, IProvideUserControls
    {
        public HermesIntentDataList() { }

        public HermesIntentDataList(IEnumerable<HermesIntentData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public HermesIntentDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentDataList> ser = new AutomationControls.Serialization.Serializer<HermesIntentDataList>(this);
            ser.ToJSON(s);
        }

        public static HermesIntentDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentDataList> ser = new AutomationControls.Serialization.Serializer<HermesIntentDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

