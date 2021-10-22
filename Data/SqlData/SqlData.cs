using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls
{
    [Serializable]
    [DataProfile(typeof(AutomationControls.SqlDataControl))]
    public class SqlData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public SqlData()
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


        private string _id;
        public string id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }

        private DateTime _lastUpdated;
        public DateTime lastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                OnPropertyChanged("lastUpdated");
            }
        }

        private string _data;
        public string data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("data");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", _id, typeof(string));
            info.AddValue("lastUpdated", _lastUpdated, typeof(DateTime));
            info.AddValue("data", _data, typeof(string));
        }
        public SqlData(SerializationInfo info, StreamingContext context)
        {
            _id = (string)info.GetValue("id", typeof(string));
            _lastUpdated = (DateTime)info.GetValue("lastUpdated", typeof(DateTime));
            _data = (string)info.GetValue("data", typeof(string));
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
            Serialization.Serializer<SqlData> ser = new Serialization.Serializer<SqlData>(this);
            ser.ToJSON(s);
        }

        public static SqlData Deserialize(string s)
        {
            Serialization.Serializer<SqlData> ser = new Serialization.Serializer<SqlData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.SqlDataListControl))]
    public class SqlDataList : ObservableCollection<SqlData>, ISerializable
    {
        public SqlDataList() { }

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

        public SqlDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            Serialization.Serializer<SqlDataList> ser = new Serialization.Serializer<SqlDataList>(this);
            ser.ToJSON(s);
        }

        public static SqlDataList Deserialize(string s)
        {
            Serialization.Serializer<SqlDataList> ser = new Serialization.Serializer<SqlDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

