using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace AutomationControls
{
    [Serializable]
    public class ObservableProfileCollection<T> : ObservableCollection<T>, IList, IProfileName, System.Runtime.Serialization.ISerializable where T : new()
    {

        public ObservableProfileCollection()
        {
            this.CollectionChanged += (sender, e) =>
            {
                //foreach(T v in e.NewItems)
                //{                                       
                //    if(v is IProfileName)
                //        if((v as IProfileName).profileName.IsNull())
                //            (v as IProfileName).profileName = "profile " + this.Count();

                //}
            };
        }

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

        private string _profileName;
        public string profileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("profileName");
            }
        }

        #region ISerializable

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("profileName", _profileName, typeof(string));
        }

        public ObservableProfileCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            profileName = (string)info.GetValue("profileName", typeof(string));
        }

        #endregion

        public override string ToString() { return profileName + " -- " + base.ToString(); }
    }

    [Serializable]
    public class ObservableProfileDictionary<T> : Dictionary<string, ObservableProfileCollection<T>>, ISerializable, INotifyPropertyChanged where T : new()
    {

        public ObservableProfileDictionary()
        {

        }

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

        private ObservableCollection<string> _ProfileNames = new ObservableCollection<string>();
        public ObservableCollection<string> ProfileNames
        {
            get { return _ProfileNames; }
            set
            {
                _ProfileNames = value;
                OnPropertyChanged("ProfileNames");
            }
        }


        public bool ContainsProfile(string profileName)
        {
            if (profileName.IsNull()) return false;
            bool ret = false;
            var res = this.Select(x => x.Key == profileName);
            if (res.Count() > 0)
            {
                ret = true;
            }
            return ret;
        }

        public ObservableCollection<string> GetProfileNames()
        {
            ProfileNames.Clear();
            this.Select(x => x.Key).ToList().ForEach(x => ProfileNames.Add(x));
            return ProfileNames;
        }

        public ObservableProfileCollection<T> AddProfile()
        {
            string name = "new";
            ObservableProfileCollection<T> ret = new ObservableProfileCollection<T>() { profileName = name };
            ret.Add(new T());
            this.Add(name, ret);
            if (!ProfileNames.Contains(name))
                ProfileNames.Add(name);
            return ret;
        }

        public ObservableProfileCollection<T> AddProfile(string profileName)
        {
            if (profileName.IsNull()) return AddProfile();
            ObservableProfileCollection<T> ret;
            if (this.ContainsKey(profileName))
            {
                ret = this[profileName];
            }
            else
            {

                ret = new ObservableProfileCollection<T>() { profileName = profileName };
                ret.Add(new T());
                this.Add(profileName, ret);
            }
            //if(!ProfileNames.Contains(profileName))
            //    ProfileNames.Add(profileName);
            return ret;
        }

        public void DeleteProfile(string profileName)
        {
            if (profileName.IsNull()) return;
            if (this.ContainsKey(profileName))
                this.Remove(profileName);
            if (ProfileNames.Contains(profileName))
                ProfileNames.Remove(profileName);
        }

        public ObservableProfileCollection<T> LoadProfile(string ProfileName)
        {
            if (ProfileName.IsNull()) ProfileName = "new";
            ObservableProfileCollection<T> ret = default(ObservableProfileCollection<T>);
            var res = this.Where(x => x.Key.Equals(ProfileName)).Select(y => y);
            if (res.Count() > 0)
            {
                ret = res.ToArray()[0].Value;
                IProfileName pn = ret as IProfileName;
                if (pn != null) { pn.profileName = ProfileName; }
            }
            else { return AddProfile(ProfileName); }
            return ret;
        }

        #region ISerializable

        public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //base.GetObjectData(info, context);

            info.AddValue("keys", this.Keys.ToArray(), typeof(string[]));
            foreach (var key in Keys)
            {
                ObservableProfileCollection<T> res = null;
                if (this.TryGetValue(key, out res))
                {
                    info.AddValue(key, res, typeof(ObservableProfileCollection<T>));
                }
            }

        }

        public ObservableProfileDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            string[] keys = (string[])info.GetValue("keys", typeof(string[]));
            foreach (var key in keys)
            {
                var v = info.GetValue(key, typeof(ObservableProfileCollection<T>));
                ObservableProfileCollection<T> val = (ObservableProfileCollection<T>)info.GetValue(key, typeof(ObservableProfileCollection<T>));
                if (!ProfileNames.Contains(key)) ProfileNames.Add(key);
                val.profileName = key;
                Add(key, val);
            }
        }

        #endregion
    }
}