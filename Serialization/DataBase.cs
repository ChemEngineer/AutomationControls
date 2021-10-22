using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace AutomationControls.Serialization
{
    public abstract class DataBase<T> : INotifyPropertyChanged, ISerializable, IProfileName where T : ISerializable, new()
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

        private string _profileName;
        public String profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("profileName");
            }
        }

        public DataBase()
        {
            //Type typeArg = this.GetType();
            //Type genericClass = typeof(Serializer<>);
            //Type constructedClass = genericClass.MakeGenericType(typeArg);
            //object created = Activator.CreateInstance(constructedClass, new object[] { this });

            //serializer = (Serializer<DataBase<T>>)created;
        }

        #region ISerializable Members

        public DataBase(SerializationInfo info, StreamingContext context)
        {
            profileName = (String)info.GetValue("profileName", typeof(String));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("profileName", profileName, typeof(String));
        }

        #endregion

    }


    public abstract class DataListBase<T> : ObservableCollection<T>, INotifyPropertyChanged, ISerializable where T : ISerializable, IProfileName, new()
    {
        #region "INotifyPropertyChanged Pattern"

        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        public Type typeT = null;

        private String _profileName;
        public String profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("profileName");
            }
        }

        public virtual void Initialize()
        {

        }

        #region Constructors

        public DataListBase()
        {
            typeT = typeof(T);
            Initialize();
        }

        public DataListBase(ObservableCollection<T> lst)
        {
            typeT = typeof(T);
            Initialize();
            this.AddRange(lst);
        }

        public DataListBase(List<T> lst)
        {
            typeT = typeof(T);
            Initialize();
            this.AddRange(lst);
        }

        public DataListBase(IEnumerable<T> lst)
        {
            typeT = typeof(T);
            Initialize();
            this.AddRange(lst);
        }

        public DataListBase(T t)
        {
            typeT = typeof(T);
            Initialize();
            this.Add(t);
        }

        #endregion

        public void ForEach(Action<T> action)
        {
            foreach (var cur in this) { action(cur); }
        }

        #region ISerializable Members

        public DataListBase(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("profileName", profileName, typeof(String));
            info.AddValue("lst", this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            profileName = (String)info.GetValue("profileName", typeof(String));
        }

        #endregion

        #region Add Remove

        internal static ObservableCollection<U> GetRandomItemsFromList<U>(int returnCount, ObservableCollection<U> source)
        {
            if (returnCount > source.Count) returnCount = source.Count;
            var returnList = new ObservableCollection<U>();
            for (var i = 0; i < returnCount; i++)
            {
                var randomElement = new Random().Next(source.Count);
                var element = source[randomElement];
                source.RemoveAt(randomElement);
                returnList.Add(element);
            }
            return returnList;
        }

        public new void Add(T t)
        {
            base.Add(t);
        }

        public void AddRange(ObservableCollection<T> lst) { foreach (T l in lst) { this.Add(l); } }

        public void AddRange(List<T> lst) { foreach (T l in lst) { this.Add(l); } }

        public void AddRange(IEnumerable<T> lst) { foreach (T l in lst) { this.Add(l); } }

        public void ChangeProfileName(string profileName)
        {
            var res = this.Where(x => !String.IsNullOrEmpty(x.profileName) && x.profileName == profileName);
            if (res.Count() > 0)
            {
                T data = res.ToArray()[0];
                data.profileName = profileName;
                //    OnRaiseDataReadyEvent(new DataReadyEventArgs() { data = data });
            }
        }

        public T LoadProfile(String profileName)
        {
            var res = this.Where(x => !String.IsNullOrEmpty(x.profileName) && x.profileName == profileName);
            if (res.Count() > 0)
            {
                foreach (var v in res)
                {
                    if (v.profileName == null) this.Remove(v);
                    if (v.profileName == profileName) return v;
                }
            }
            T data = new T();
            this.Add(data);
            data.profileName = profileName;
            // this.Serialize(SerializationType.XML);
            return data;
        }

        public T LoadData(T data)
        {
            if (data == null) return default(T);

            var res = this.Where(x => !String.IsNullOrEmpty(x.profileName) && x.profileName == profileName);
            if (res.Count() == 0)
            {
                this.Add(data);
                //    OnRaiseDataReadyEvent(new DataReadyEventArgs() { data = data });
                //serializer.ToXML();
            }
            return data;
        }

        public void DeleteProfile(String profileName)
        {
            var res = this.Where(x => !String.IsNullOrEmpty(x.profileName) && x.profileName == profileName);
            if (res.Count() > 0)
            {
                T data = res.ToArray()[0];
                this.Remove(data);
                //serializer.ToXML();
            }
        }

        #endregion
    }
}
