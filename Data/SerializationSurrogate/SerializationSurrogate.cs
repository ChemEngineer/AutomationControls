using AutomationControls.Attributes;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using AutomationControls.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace AutomationControls
{
    // [DataProfileAttribute(typeof(DeviceBaseControl))]
    [Serializable]
    [DataProfile(typeof(AutomationControls.UserControls.SerializationSurrogate.SurrogateControl))]
    public class SerializationSurrogate<T> : AutomationControls.Interfaces.ISerializationSurrogate, ISerializable, INotifyPropertyChanged, IProfileName, IProvideUserControls where T : class, ISerializable, new()
    {

        public SerializationSurrogate()
        {
            string pn = "new";
            _lstlst = new ObservableProfileDictionary<T>();
            _lst = new ObservableProfileCollection<T>() { profileName = pn };
            _data = new T();

            lst.Add(data);
            lstlst.Add(pn, lst);

            ProfileNames = lstlst.GetProfileNames();
        }

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

        private string _profileName;
        [Serialize()]
        public string profileName
        {
            get { return _profileName; }
            set
            {
                if (value == null) return;

                _profileName = value;
                //if(!_ProfileNames.Contains(value))
                //    ProfileNames.Add(value);
                OnPropertyChanged("profileName");
            }
        }

        private string _path;
        public string path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged("path");
                }
            }
        }


        private ObservableCollection<string> _ProfileNames = new ObservableCollection<string>();
        public ObservableCollection<string> ProfileNames
        {
            get { return _ProfileNames; }
            set
            {
                _ProfileNames = value;
                //foreach(var v in value)
                //{
                //    if(!_ProfileNames.Contains(v))
                //    {
                //        _ProfileNames.Add(v);
                //    }
                //}
                OnPropertyChanged("ProfileNames");
            }
        }

        public object getData
        {
            get { return (object)data; }
        }

        public void setData(object obj)
        {
            var v = obj as T;
        }

        public IList getList
        {
            get { return (IList)lst; }
        }

        public IDictionary getListList
        {
            get { return (IDictionary)lstlst; }

        }


        private ObservableProfileDictionary<T> _lstlst;
        [Serialize]
        public ObservableProfileDictionary<T> lstlst
        {
            get { return _lstlst; }
            set
            {
                _lstlst = value;
                OnPropertyChanged("lstlst");
            }
        }

        private ObservableProfileCollection<T> _lst;
        [Serialize]
        public ObservableProfileCollection<T> lst
        {
            get { return _lst; }
            set
            {
                _lst = value;
                OnPropertyChanged("lst");
            }
        }

        private T _data;
        // [Serialize]
        public T data
        {
            get
            {
                if (_data == null) _data = new T();
                return _data;
            }
            set
            {

                bool b = _data.Equals(value);
                if (b == false)
                {
                    _data = value;
                    OnPropertyChanged("data");
                }
            }
        }



        public string Serialize()
        {
            //return SerializeBinary();
            return SerializeJSON().ToString();
        }

        public string Deserialize()
        {
            //return DeserializeBinary();
            return DeserializeJSON();
        }

        public string SerializeBinary()
        {
            Type typeArg = this.GetType();
            Type genericClass = typeof(Serializer<>);
            Type constructedClass = genericClass.MakeGenericType(typeArg);
            object created = Activator.CreateInstance(constructedClass, new object[] { this });
            var mi = created.GetType().GetMethod("ToBinary").Invoke(created, new object[] { path });
            return mi.ToString();
        }

        public string DeserializeBinary()
        {
            string ret = "";
            if (File.Exists(path))
            {
                Type typeArg = this.GetType();
                Type genericClass = typeof(Serializer<>);
                Type constructedClass = genericClass.MakeGenericType(typeArg);
                object created = Activator.CreateInstance(constructedClass, new object[] { this });
                var mi = created.GetType().GetMethod("FromBinary").Invoke(created, new object[] { path });

                AutomationControls.Interfaces.ISerializationSurrogate surr = mi as AutomationControls.Interfaces.ISerializationSurrogate;
                this.lstlst = (ObservableProfileDictionary<T>)surr.getListList;
                this.lst = lstlst[lstlst.Keys.First()];
                this.data = lst[0];

                this.profileName = surr.profileName;
                ProfileNames = lstlst.GetProfileNames();
                ret = mi.ToString();

            }
            return ret;
        }


        public bool SerializeJSON()
        {
            Type typeArg = this.GetType();
            Type genericClass = typeof(Serializer<>);
            Type constructedClass = genericClass.MakeGenericType(typeArg);
            object created = Activator.CreateInstance(constructedClass, new object[] { this });
            var ff = created.GetType().GetMethod("ToJSON");
            var mi = created.GetType().GetMethod("ToJSON").Invoke(created, new object[] { path });
            // var mi = created.GetType().GetMethod("ToBinary").Invoke(created, new object[] { path });
            return true;
        }

        public string DeserializeJSON()
        {
            string ret = "";
            if (File.Exists(path))
            {
                string file = File.ReadAllText(path);
                Type typeArg = this.GetType();
                Type genericClass = typeof(Serializer<>);
                Type constructedClass = genericClass.MakeGenericType(typeArg);
                object created = Activator.CreateInstance(constructedClass, new object[] { this });
                var mi = created.GetType().GetMethod("FromJSON").Invoke(created, new object[] { path });
                //var mi = created.GetType().GetMethod("FromBinary").Invoke(created, new object[] { path });

                AutomationControls.Interfaces.ISerializationSurrogate surr = mi as AutomationControls.Interfaces.ISerializationSurrogate;
                this.lstlst = (ObservableProfileDictionary<T>)surr.getListList;
                this.lst = lstlst[lstlst.Keys.First()];
                this.data = lst[0];

                this.profileName = surr.profileName;
                ProfileNames = lstlst.GetProfileNames();
                ret = mi.ToString();
            }
            return ret;
        }

        #region ISerializable 

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type t = this.GetType();
            var lstpi = t.GetPropertiesWithCustomAttribute<SerializeAttribute>();
            foreach (var v in lstpi)
            {
                var name = v.Name;
                var type = v.PropertyType;
                var res = v.GetValue(this);
                info.AddValue(v.Name, v.GetValue(this), v.PropertyType);
                var ret = info.GetValue(v.Name, v.PropertyType);
            }
        }

        public SerializationSurrogate(SerializationInfo info, StreamingContext context)
        {
            Type t = this.GetType();
            var lstpi = t.GetPropertiesWithCustomAttribute<SerializeAttribute>();
            foreach (var v in lstpi)
            {
                var name = v.Name;
                var type = v.PropertyType;
                var ret = info.GetValue(v.Name, v.PropertyType);
                if (ret == null) continue;
                v.SetValue(this, ret);
            }
        }

        #endregion

        #region Profile

        public void AddProfile(string profileName)
        {
            if (profileName.IsNull()) return;
            this.profileName = profileName;
            lst = lstlst.LoadProfile(profileName);
            data = lst[0];
            if (!ProfileNames.Contains(profileName))
                ProfileNames.Add(profileName);
        }

        public void DeleteProfile(string profileName)
        {
            if (profileName.IsNull()) return;
            this.profileName = profileName;
            var res = _lstlst.Where(x => x.Key.Equals(profileName));
            _lstlst.Remove(profileName);

            if (ProfileNames.Contains(profileName))
                ProfileNames.Remove(profileName);
        }

        public void LoadProfile(string profileName)
        {
            if (profileName.IsNull()) return;
            this.profileName = profileName;

            ObservableProfileCollection<T> res = lstlst.LoadProfile(profileName);
            if (res == null || res.Count == 0)
            {
                // AddProfile(profileName);
                return;
            }
            else
            {
                lst = res;
                lst.profileName = profileName;
            }
        }

        public bool ContainsProfile(string ProfileName)
        {
            return lstlst.ContainsProfile(ProfileName);

        }

        #endregion

        #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            UserControl uc = new UserControl();
            List<UserControl> lst = new List<UserControl>();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                if (lst.Where(x => x.GetType() == type).Count() == 0)
                {
                    uc = (UserControl)Activator.CreateInstance(type);
                    if (uc == null) continue;
                    uc.DataContext = this;
                    lst.Add(uc);
                }
            }

            return lst.ToArray();
        }

        #endregion
    }

    [Serializable]
    public class SerializationSurrogateList : List<object>, IProfileName
    {

        #region "INotifyPropertyChanged Pattern"

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region IProfileName Members

        private string _profileName;
        public string profileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName != value)
                {
                    _profileName = value;
                    OnPropertyChanged("profileName");
                }
            }
        }


        #endregion
    }
}
