using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Windows.Controls;
using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using AutomationControls.Extensions;

namespace AutomationControls.Communication.MQTT        
{

	

    [Serializable]
    [DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTTestControl))]
    public class MQTTTest :  INotifyPropertyChanged, ISerializable, IProvideUserControls
    {
        public MQTTTest() 
        {
            Initialize();
        }

        private void Initialize() {  }

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


   private MQTTBroker _broker;
   public MQTTBroker broker
   {
      get { return _broker; }
      set
      {
         _broker = value;
         OnPropertyChanged("broker");
      }
   }

   private MQTTClient _client;
   public MQTTClient client
   {
      get { return _client; }
      set
      {
         _client = value;
         OnPropertyChanged("client");
      }
   }

   private IProgress<string> _progressSend;
   public IProgress<string> progressSend
   {
      get { return _progressSend; }
      set
      {
         _progressSend = value;
         OnPropertyChanged("progressSend");
      }
   }



        public void GetObjectData(SerializationInfo info, StreamingContext context)   
{
info.AddValue("broker",_broker, typeof(MQTTBroker));
info.AddValue("client",_client, typeof(MQTTClient));
info.AddValue("progressSend",_progressSend, typeof(IProgress<string>));
}
public  MQTTTest(SerializationInfo info, StreamingContext context)
{
_broker= (MQTTBroker) info.GetValue("broker", typeof(MQTTBroker));
_client= (MQTTClient) info.GetValue("client", typeof(MQTTClient));
_progressSend= (IProgress<string>) info.GetValue("progressSend", typeof(IProgress<string>));
}


		#region IProvideUserControls

		public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach(var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if(cc == null) continue;
                lst.Add(cc);
                cc.DataContext = this;
            }
            return lst.ToArray();
        }

        #endregion

		public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTest> ser = new AutomationControls.Serialization.Serializer<MQTTTest>(this);
            ser.ToJSON(s);
        }

        public static MQTTTest Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTest> ser = new AutomationControls.Serialization.Serializer<MQTTTest>();
            var res = ser.FromJSON( s);
            return res;
        }

    }

    [Serializable]
	[DataProfile(typeof(AutomationControls.Communication.MQTT.MQTTTestListControl))]
    public class MQTTTestList : ObservableCollection<MQTTTest> , ISerializable, IProvideUserControls
    {
		   public MQTTTestList() { }

           public MQTTTestList(IEnumerable<MQTTTest> lst) { lst.ForEach(x => this.Add(x));}

		    #region IProvideUserControls

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();
            UserControl cc = new UserControl();											
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach(var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                cc = (UserControl)Activator.CreateInstance(type);
                if(cc == null) continue;
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

        public MQTTTestList(SerializationInfo info, StreamingContext context)
        {
           
        }

        #endregion

		public void Serialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTestList> ser = new AutomationControls.Serialization.Serializer<MQTTTestList>(this);
            ser.ToJSON(s);
        }

        public static MQTTTestList Deserialize(string s)
        {
            AutomationControls.Serialization.Serializer<MQTTTestList> ser = new AutomationControls.Serialization.Serializer<MQTTTestList>();
            var res = ser.FromJSON( s);
            return res;
        }
    }
}

