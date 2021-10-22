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
    [DataProfile(typeof(AutomationControls.communication.mqtt.HermesIntentResponseDataControl))]
    public class HermesIntentResponseData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public HermesIntentResponseData()
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


        private string _sessionId;
        public string sessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                OnPropertyChanged("sessionId");
            }
        }

        private string _customData;
        public string customData
        {
            get { return _customData; }
            set
            {
                _customData = value;
                OnPropertyChanged("customData");
            }
        }

        private string _input;
        public string input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged("input");
            }
        }

        private AsrTokenData[][] _asrTokens;
        public AsrTokenData[][] asrTokens
        {
            get { return _asrTokens; }
            set
            {
                _asrTokens = value;
                OnPropertyChanged("asrTokens");
            }
        }

        private HermesIntentData _intent;
        public HermesIntentData intent
        {
            get { return _intent; }
            set
            {
                _intent = value;
                OnPropertyChanged("intent");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("sessionId", _sessionId, typeof(string));
            info.AddValue("customData", _customData, typeof(string));
            info.AddValue("input", _input, typeof(string));
            info.AddValue("asrTokens", _asrTokens, typeof(AsrTokenData[][]));
            info.AddValue("intent", _intent, typeof(HermesIntentData));
        }
        public HermesIntentResponseData(SerializationInfo info, StreamingContext context)
        {
            _sessionId = (string)info.GetValue("sessionId", typeof(string));
            _customData = (string)info.GetValue("customData", typeof(string));
            _input = (string)info.GetValue("input", typeof(string));
            _asrTokens = (AsrTokenData[][])info.GetValue("asrTokens", typeof(AsrTokenData[][]));
            _intent = (HermesIntentData)info.GetValue("intent", typeof(HermesIntentData));
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
            AutomationControls.Serialization.Serializer<HermesIntentResponseData> ser = new AutomationControls.Serialization.Serializer<HermesIntentResponseData>(this);
            ser.ToJSON(s);
        }

        public static HermesIntentResponseData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentResponseData> ser = new AutomationControls.Serialization.Serializer<HermesIntentResponseData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.communication.mqtt.HermesIntentResponseDataListControl))]
    public class HermesIntentResponseDataList : ObservableCollection<HermesIntentResponseData>, ISerializable, IProvideUserControls
    {
        public HermesIntentResponseDataList() { }

        public HermesIntentResponseDataList(IEnumerable<HermesIntentResponseData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public HermesIntentResponseDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentResponseDataList> ser = new AutomationControls.Serialization.Serializer<HermesIntentResponseDataList>(this);
            ser.ToJSON(s);
        }

        public static HermesIntentResponseDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<HermesIntentResponseDataList> ser = new AutomationControls.Serialization.Serializer<HermesIntentResponseDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

