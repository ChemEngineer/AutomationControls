using AutomationControls.Codex.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace AutomationControls
{
    public static class db
    {
        static db()
        {

        }


        public static AutomationControls.Serialization.ucProfileHost host { get; set; }

        public static SerializationSurrogateControl ssc { get { return (SerializationSurrogateControl)sscdata.GetUserControls()[0]; } }
        public static SerializationSurrogateControlData sscdata = new SerializationSurrogateControlData();

        public static ObservableCollection<object> lstSerialize = new ObservableCollection<object>();
        public static List<UserControl> lstUserControls = new List<UserControl>();

        #region "INotifyPropertyChanged Pattern"

        public static event PropertyChangedEventHandler PropertyChangedEvent = delegate { };
        public static void OnPropertyChanged(string name)
        {
            if (PropertyChangedEvent != null)
            {
                PropertyChangedEvent(typeof(db), new PropertyChangedEventArgs(name));
            }
        }

        #endregion


        #region DataReadyEventHandler

        public delegate void DataReadyEventHandler(object sender, DataReadyEventArgs a);
        public static event EventHandler<DataReadyEventArgs> DataReadyEvent;
        public static void OnDataReadyEvent(DataReadyEventArgs e)
        {
            EventHandler<DataReadyEventArgs> handler = DataReadyEvent;
            if (handler != null)
            {
                handler(typeof(db), e);
            }
        }
        public class DataReadyEventArgs : EventArgs
        {
            public DataReadyEventArgs() { }
            private string _Message;
            public string Message { get { return _Message; } set { _Message = value; } }
        }

        #endregion

        public static CodexData FromType(Type t)
        {
            CodexData ret = new CodexData();
            ret.className = t.Name;
            ret.IsNotifyPropertyChanged = true;
            ret.IsISerializable = true;
            ret.csNamespaceName = t.Namespace;
            ret.profileName = t.Name;

            if (ret.lstProperties == null) ret.lstProperties = new PropertiesDataList();
            foreach (var pi in t.GetProperties())
            {
                ret.lstProperties.Add(new PropertiesData() { name = pi.Name, type = pi.PropertyType.ToString(), profileName = t.Name });
            }
            return ret;
        }

    }
}