using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls.Codex.Data
{
    [Serializable]
    public class EnumData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public EnumData()
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


        private int _position;
        public int position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("position");
            }
        }

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



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("position", _position, typeof(int));
            info.AddValue("value", _value, typeof(string));
        }
        public EnumData(SerializationInfo info, StreamingContext context)
        {
            _position = (int)info.GetValue("position", typeof(int));
            _value = (string)info.GetValue("value", typeof(string));
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
            AutomationControls.Serialization.Serializer<EnumData> ser = new AutomationControls.Serialization.Serializer<EnumData>(this);
            ser.ToJSON(s);
        }

        public static EnumData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<EnumData> ser = new AutomationControls.Serialization.Serializer<EnumData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    public class EnumDataList : ObservableCollection<EnumData>, ISerializable, IProvideUserControls
    {
        public EnumDataList() { }

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

        public EnumDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion


        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                //OnPropertyChanged("name");
            }
        }

        //  info.AddValue("name", _name, typeof(string));
        //  _name= (string)info.GetValue("name", typeof(string));


        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<EnumDataList> ser = new AutomationControls.Serialization.Serializer<EnumDataList>(this);
            ser.ToJSON(s);
        }

        public static EnumDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<EnumDataList> ser = new AutomationControls.Serialization.Serializer<EnumDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

