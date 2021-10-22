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
using System.Windows.Controls;

namespace AutomationControls
{
    [Serializable]
    [DataProfile(typeof(SerializationSurrogateControl))]
    public class SerializationSurrogateControlData : INotifyPropertyChanged, System.Runtime.Serialization.ISerializable, IProvideUserControls
    {

        private SerializationSurrogateControlDataList _backupData = new SerializationSurrogateControlDataList();
        public SerializationSurrogateControlDataList backupData
        {
            get { return _backupData; }
            set { _backupData = value; }
        }

        private string _currentProfile;
        public string currentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                LoadClass(value);
                LoadProfile(currentProfile);
                OnPropertyChanged("currentProfile");
            }
        }

        private ObservableCollection<string> _classNames = new ObservableCollection<string>();
        public ObservableCollection<string> classNames
        {
            get { return _classNames; }
            set
            {
                foreach (var v in value)
                {
                    if (!_classNames.Contains(v))
                        _classNames.Add(v);
                }
                OnPropertyChanged("classNames");
            }
        }

        private string _currentClass;
        public string currentClass
        {
            get { return _currentClass; }
            set
            {
                _currentClass = value;
                LoadClass(value);
                LoadProfile(currentProfile);
                OnPropertyChanged("currentClass");
            }
        }

        private ISerializationSurrogate _surr;
        public ISerializationSurrogate surr
        {
            get { return _surr; }
            set
            {
                _surr = value;
                OnPropertyChanged("surr");
            }
        }

        public ObservableCollection<object> lstSurrogates { get; set; }

        public SerializationSurrogateControlData()
        {
            lstSurrogates = new ObservableCollection<object>();
            //  profileNames.Add("new profile");

        }

        public void AddClass(Type type)
        {
            if (type == null) return;

            if (!classNames.Contains(type.Name)) classNames.Add(type.Name);

            if (ContainsClass(type.Name))
            {
                var surrogates = lstSurrogates.Where(x => ((ISerializationSurrogate)x).getData.GetType().Name == type.Name);
                if (surrogates.Count() > 0)
                {
                    surr = ((ISerializationSurrogate)surrogates.ToArray()[0]);
                    surr.profileName = "new";
                    surr.ProfileNames.Add("new");
                }
            }
            else
            {
                try
                {
                    Type genericClass = typeof(SerializationSurrogate<>);
                    Type constructedClass = genericClass.MakeGenericType(type);
                    object created = Activator.CreateInstance(constructedClass, new object[] { });

                    lstSurrogates.Add(created);

                    surr = created as ISerializationSurrogate;
                    surr.path = GeneratePath(surr);
                    var s = surr.Deserialize();
                    if (surr.profileName.IsNull())
                    {
                        surr.profileName = "new";
                    }
                }
                catch (Exception) { }
            }
        }


        public void Serialize()
        {
            backupData.Clear();
            foreach (ISerializationSurrogate surr in lstSurrogates)
            {
                backupData.Add(new SerializationSurrogateControlData() { ClassName = surr.getData.GetType().AssemblyQualifiedName, FilePath = GeneratePath(surr) });
                surr.path = GeneratePath(surr);
                surr.Serialize();
            }
            //Data to repopulate user control
            Serializer<SerializationSurrogateControlDataList> ser = new Serializer<SerializationSurrogateControlDataList>(backupData);
            ser.ToJSON(GeneratePath(backupData));
        }

        public void Deserialize()
        {
            lstSurrogates.Clear();

            Serializer<SerializationSurrogateControlDataList> ser = new Serializer<SerializationSurrogateControlDataList>(backupData);
            string path = GeneratePath(backupData);
             backupData = ser.FromJSON(path);
            if (backupData == null) backupData = new SerializationSurrogateControlDataList();
            for (int i = 0; i < backupData.Count(); i++)
            {
                try
                {
                    Type type = Type.GetType(backupData[i].ClassName);
                    AddClass(type);

                }
                catch { }
            }

            db.OnDataReadyEvent(new db.DataReadyEventArgs() { Message = "MainWindow Loaded" });
        }

        private bool ContainsClass(string className)
        {
            bool ret = false;
            foreach (ISerializationSurrogate s in lstSurrogates)
            {
                foreach (DictionaryEntry vv in s.getListList)
                {
                    if ((vv.Value.GetType().GetGenericArguments()[0]).Name == className)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool LoadClass(string className)
        {
            bool ret = false;
            foreach (ISerializationSurrogate s in lstSurrogates)
            {
                foreach (DictionaryEntry vv in s.getListList as IDictionary)
                {
                    if ((vv.Value.GetType().GetGenericArguments()[0]).Name == className)
                    {
                        ret = true;
                        surr = s;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool ContainsProfile(string profileName) { return surr.ContainsProfile(profileName); }

        public void AddProfile(string profileName) { surr.AddProfile(profileName); }

        public void RemoveProfile(string profileName) { surr.DeleteProfile(profileName); }

        public void LoadProfile(string profileName)
        {
            if (surr == null) return;
            surr.LoadProfile(profileName);
        }

        public void RemoveClass(string ClassName)
        {
            for (int i = 0; i < lstSurrogates.Count(); i++)
            {
                if ((lstSurrogates[i] as ISerializationSurrogate).getData.GetType().Name.Contains(ClassName))
                {
                    lstSurrogates.Remove(lstSurrogates[i]);
                    classNames.Remove(ClassName);
                }
            }
        }

        private string GeneratePath(Object o, string extension = ".json")
        {
            // string ret = @"C:\Saved Data\";
            // string ret = Environment.CurrentDirectory + "\\";
            string ret = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\";
            o.GetType().Namespace.Split(new[] { ".", "~", "`", "'", "`1" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => ret += x + "\\");
            ret += o.GetType().Name + "\\";

            if (o is ISerializationSurrogate)
            {
                var v = o as ISerializationSurrogate;
                var data = v.getData;
                var propPath = v.path;
                ret += v.getData.GetType().ToString() + "_" + o.GetType().Name + extension;
                v.path = ret;
            }
            else if (o is IProfileName)
            {
                ret += (o as IProfileName).profileName + "_" + o.GetType().Name + extension;
            }
            else
            {
                ret += o.GetType().Name + extension;
            }
            // Create directory if not present
            string p = Path.GetDirectoryName(ret);
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);

            return ret;
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

        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private string _ClassName;
        public string ClassName
        {
            get { return _ClassName; }
            set
            {
                _ClassName = value;
                OnPropertyChanged("ClassName");
            }
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("FilePath", _FilePath, typeof(string));
            info.AddValue("ClassName", _ClassName, typeof(string));
        }
        public SerializationSurrogateControlData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _FilePath = (string)info.GetValue("FilePath", typeof(string));
            _ClassName = (string)info.GetValue("ClassName", typeof(string));
        }

        #region IProvideUserControls

        private UserControl[] controls;
        public UserControl[] GetUserControls()
        {
            if (controls == null)
                controls = this.GenerateControl();
            return controls;
        }

        //public UserControl[] GetUserControls()
        //{
        //    List<UserControl> lst = new List<UserControl>();
        //    UserControl cc = new UserControl();
        //    var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
        //    foreach (var attr in attrs)
        //    {
        //        var type = ((DataProfileAttribute)attr).ClassName;
        //        cc = (UserControl)Activator.CreateInstance(type);
        //        if (cc == null) continue;
        //        lst.Add(cc);
        //        cc.DataContext = this;
        //    }
        //    var attrs2 = this.GetType().GetProperties();


        //    if (surr != null)
        //        lst.AddRange(surr.GetUserControls());

        //    return lst.ToArray();
        //}

        #endregion
    }

    [Serializable]
    [DataProfile(typeof(AutomationControls.SerializationSurrogateControlDataListControl))]
    public class SerializationSurrogateControlDataList : ObservableCollection<SerializationSurrogateControlData>, System.Runtime.Serialization.ISerializable, IProvideUserControls
    {
        public SerializationSurrogateControlDataList() { }

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

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("lst", this, this.GetType());
        }

        public SerializationSurrogateControlDataList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {

        }

        #endregion
    }
}

