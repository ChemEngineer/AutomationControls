using AutomationControls.Attributes;
using AutomationControls.Extensions;
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
    [AutomationControls.Attributes.DataProfile(typeof(AutomationControls.Codex.Data.PropertiesDataControl))]
    public class PropertiesData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public PropertiesData()
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


        private System.String _name;
        public System.String name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        private System.String _type;
        public System.String type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("type");
            }
        }

        private System.Boolean _IsEnum;
        public System.Boolean IsEnum
        {
            get { return _IsEnum; }
            set
            {
                _IsEnum = value;
                if (value && lstEnum == null)
                    lstEnum = new EnumDataList();

                OnPropertyChanged("IsEnum");
            }
        }

        private System.Boolean _IsList;
        public System.Boolean IsList
        {
            get { return _IsList; }
            set
            {
                _IsList = value;
                OnPropertyChanged("IsList");
            }
        }

        private System.String _profileName;
        public System.String profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("profileName");
            }
        }

        private System.String _defaultSerializationPath;
        public System.String defaultSerializationPath
        {
            get { return _defaultSerializationPath; }
            set
            {
                _defaultSerializationPath = value;
                OnPropertyChanged("defaultSerializationPath");
            }
        }



        private EnumDataList _lstEnum;
        public EnumDataList lstEnum
        {
            get { return _lstEnum; }
            set
            {
                _lstEnum = value;
                OnPropertyChanged("lstEnum");
            }
        }

        private System.Boolean _isObject;
        public System.Boolean isObject
        {
            get { return _isObject; }
            set
            {
                _isObject = value;
                OnPropertyChanged("isObject");
            }
        }


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



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", _name, typeof(System.String));
            info.AddValue("type", _type, typeof(System.String));
            info.AddValue("IsEnum", _IsEnum, typeof(System.Boolean));
            info.AddValue("IsList", _IsList, typeof(System.Boolean));
            info.AddValue("profileName", _profileName, typeof(System.String));
            info.AddValue("defaultSerializationPath", _defaultSerializationPath, typeof(System.String));
            info.AddValue("lstEnum", _lstEnum, typeof(EnumDataList));
            info.AddValue("isObject", _isObject, typeof(System.Boolean));
            info.AddValue("position", _position, typeof(int));
        }
        public PropertiesData(SerializationInfo info, StreamingContext context)
        {
            _name = (System.String)info.GetValue("name", typeof(System.String));
            _type = (System.String)info.GetValue("type", typeof(System.String));
            _IsEnum = (System.Boolean)info.GetValue("IsEnum", typeof(System.Boolean));
            _IsList = (System.Boolean)info.GetValue("IsList", typeof(System.Boolean));
            _profileName = (System.String)info.GetValue("profileName", typeof(System.String));
            _defaultSerializationPath = (System.String)info.GetValue("defaultSerializationPath", typeof(System.String));
            _lstEnum = (EnumDataList)info.GetValue("lstEnum", typeof(EnumDataList));
            _isObject = (System.Boolean)info.GetValue("isObject", typeof(System.Boolean));
               _position = (int)info.GetValue("position", typeof(int));
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
            AutomationControls.Serialization.Serializer<PropertiesData> ser = new AutomationControls.Serialization.Serializer<PropertiesData>(this);
            ser.ToJSON(s);
        }

        public static PropertiesData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<PropertiesData> ser = new AutomationControls.Serialization.Serializer<PropertiesData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [AutomationControls.Attributes.DataProfile(typeof(Codex.Data.PropertiesDataListControl))]
    public class PropertiesDataList : ObservableCollection<PropertiesData>, ISerializable, IProvideUserControls
    {
        public PropertiesDataList() { }

        public PropertiesDataList(IEnumerable<PropertiesData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public PropertiesDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<PropertiesDataList> ser = new AutomationControls.Serialization.Serializer<PropertiesDataList>(this);
            ser.ToJSON(s);
        }

        public static PropertiesDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<PropertiesDataList> ser = new AutomationControls.Serialization.Serializer<PropertiesDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

