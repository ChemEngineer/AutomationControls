using AutomationControls.Attributes;
using AutomationControls.Extensions;
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
    [DataProfile(typeof(AutomationControls.CodexDataControl))]
    public class CodexData : INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public CodexData()
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


        private System.String _androidNamespace;
        public System.String androidNamespace
        {
            get { return _androidNamespace; }
            set
            {
                _androidNamespace = value;
                OnPropertyChanged("androidNamespace");
            }
        }

        private System.String _csNamespaceName;
        public System.String csNamespaceName
        {
            get { return _csNamespaceName; }
            set
            {
                _csNamespaceName = value;
                OnPropertyChanged("csNamespaceName");
            }
        }

        private System.String _extendedNamespace;
        public System.String extendedNamespace
        {
            get { return _extendedNamespace; }
            set
            {
                _extendedNamespace = value;
                OnPropertyChanged("extendedNamespace");
            }
        }

        private System.String _className;
        public System.String className
        {
            get { return _className; }
            set
            {
                _className = value;
                OnPropertyChanged("className");
            }
        }

        private System.String _constructor;
        public System.String constructor
        {
            get { return _constructor; }
            set
            {
                _constructor = value;
                OnPropertyChanged("constructor");
            }
        }

        private System.String _listConstructor;
        public System.String listConstructor
        {
            get { return _listConstructor; }
            set
            {
                _listConstructor = value;
                OnPropertyChanged("listConstructor");
            }
        }

        private System.Boolean _IsNotifyPropertyChanged;
        public System.Boolean IsNotifyPropertyChanged
        {
            get { return _IsNotifyPropertyChanged; }
            set
            {
                _IsNotifyPropertyChanged = value;
                OnPropertyChanged("IsNotifyPropertyChanged");
            }
        }

        private System.Boolean _IsISerializable;
        public System.Boolean IsISerializable
        {
            get { return _IsISerializable; }
            set
            {
                _IsISerializable = value;
                OnPropertyChanged("IsISerializable");
            }
        }

        private System.Boolean _IsEntityFramework;
        public System.Boolean IsEntityFramework
        {
            get { return _IsEntityFramework; }
            set
            {
                _IsEntityFramework = value;
                OnPropertyChanged("IsEntityFramework");
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

        private AutomationControls.Codex.Data.PropertiesDataList _lstProperties = new Codex.Data.PropertiesDataList();
        public AutomationControls.Codex.Data.PropertiesDataList lstProperties
        {
            get { return _lstProperties; }
            set
            {
                _lstProperties = value;
                OnPropertyChanged("lstProperties");
            }
        }



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("androidNamespace", _androidNamespace, typeof(System.String));
            info.AddValue("csNamespaceName", _csNamespaceName, typeof(System.String));
            info.AddValue("extendedNamespace", _extendedNamespace, typeof(System.String));
            info.AddValue("className", _className, typeof(System.String));
            info.AddValue("constructor", _constructor, typeof(System.String));
            info.AddValue("listConstructor", _listConstructor, typeof(System.String));
            info.AddValue("IsNotifyPropertyChanged", _IsNotifyPropertyChanged, typeof(System.Boolean));
            info.AddValue("IsISerializable", _IsISerializable, typeof(System.Boolean));
            info.AddValue("profileName", _profileName, typeof(System.String));
            info.AddValue("lstProperties", _lstProperties, typeof(AutomationControls.Codex.Data.PropertiesDataList));
        }
        public CodexData(SerializationInfo info, StreamingContext context)
        {
            _androidNamespace = (System.String)info.GetValue("androidNamespace", typeof(System.String));
            _csNamespaceName = (System.String)info.GetValue("csNamespaceName", typeof(System.String));
            _extendedNamespace = (System.String)info.GetValue("extendedNamespace", typeof(System.String));
            _className = (System.String)info.GetValue("className", typeof(System.String));
            _constructor = (System.String)info.GetValue("constructor", typeof(System.String));
            _listConstructor = (System.String)info.GetValue("listConstructor", typeof(System.String));
            _IsNotifyPropertyChanged = (System.Boolean)info.GetValue("IsNotifyPropertyChanged", typeof(System.Boolean));
            _IsISerializable = (System.Boolean)info.GetValue("IsISerializable", typeof(System.Boolean));
            _profileName = (System.String)info.GetValue("profileName", typeof(System.String));
            _lstProperties = (AutomationControls.Codex.Data.PropertiesDataList)info.GetValue("lstProperties", typeof(AutomationControls.Codex.Data.PropertiesDataList));
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
            AutomationControls.Serialization.Serializer<CodexData> ser = new AutomationControls.Serialization.Serializer<CodexData>(this);
            ser.ToJSON(s);
        }

        public static CodexData Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<CodexData> ser = new AutomationControls.Serialization.Serializer<CodexData>();
            var res = ser.FromJSON(s);
            return res;
        }

    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.CodexDataListControl2))]
    public class CodexDataList : ObservableCollection<CodexData>, ISerializable, IProvideUserControls
    {
        public CodexDataList() { }

        public CodexDataList(IEnumerable<CodexData> lst) { lst.ForEach(x => this.Add(x)); }

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

        public CodexDataList(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<CodexDataList> ser = new AutomationControls.Serialization.Serializer<CodexDataList>(this);
            ser.ToJSON(s);
        }

        public static CodexDataList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<CodexDataList> ser = new AutomationControls.Serialization.Serializer<CodexDataList>();
            var res = ser.FromJSON(s);
            return res;
        }
    }
}

