using AutomationControls.Attributes;
using AutomationControls.Communication.Serial.DataClasses;
using AutomationControls.Communication.Serial.UserControls;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication
{

    //[DataProfile(typeof(Communication.Serial.UserControls.SerialPortReceiveControl))]
    //[DataProfile(typeof(Communication.Serial.UserControls.SerialPortOptionsControl))]
    [Serializable]
    public class SerialCommunication : SerialPortData, ICommunications, ISerializable, IProvideUserControls
    {

        public SerialCommunication() : base()
        {

        }

        #region ICommunications Members

        public Enums.DeviceCommunications _bus
        {
            get
            {
                return Enums.DeviceCommunications.RS232;
            }

        }

        private bool _ConnectAtStartup = false;
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; }
        }

        public Task OpenCommunicationChannelAsync()
        {
            return Task.Run(() => { OpenCommunicationChannel(); }, CancellationToken.None);
        }

        public bool IsBusy
        {
            get
            {
                return false;
            }
        }

        public void OpenCommunicationChannel()
        {
            if (!sp.IsOpen)
                sp.Open();
        }

        public void CloseCommunicationChannel()
        {
            sp.Close();
        }

        public bool IsChannelOpen
        {
            get
            {
                if (sp == null) return false;
                return sp.IsOpen;
            }
        }

        public void SendString(string command, string destination = "")
        {
            if (!sp.IsOpen) sp.Open();
            sp.Write(command);
            if (!keepOpen)
                CloseCommunicationChannel();
        }

        public void SendBytes(byte[] b, string destination = "")
        {
            try
            {
                if (!sp.IsOpen) sp.Open();
                if (sp.IsOpen)
                {
                    base.sp.Write(b, 0, b.Length);
                }
            }
            catch { }
            finally
            {
                if (!keepOpen)
                    CloseCommunicationChannel();
            }
        }

        public string ReadString()
        {
            if (!sp.IsOpen) return "";
            List<byte> lst = new List<byte>();
            while (sp.BytesToRead > 0)
            {
                lst.Add((byte)sp.ReadByte());
            }
            return (ASCIIEncoding.ASCII.GetString(lst.ToArray()));
        }

        public Task SendStringAsync(string command, string destination = "")
        {
            if (!sp.IsOpen) sp.Open();
            return sp.WriteAsync(command, progressSend, cts.Token);

        }

        public Task SendBytesAsync(byte[] b, string destination = "")
        {
            if (!sp.IsOpen) sp.Open();
            return sp.WriteAsync(b, 0, b.Length, progressSend, cts.Token).ContinueWith((e) =>
            {
                if (!keepOpen)
                    CloseCommunicationChannel();
            });
        }

        public Task<string> ReadStringAsync()
        {
            return Task.Run(() => { return sp.ReadExisting(); }, cts.Token);
        }


        public System.Windows.Controls.UserControl GetUserControl()
        {
            SerialPortOptionsControl uc = new SerialPortOptionsControl();
            uc.DataContext = this;

            return uc;
        }

        public UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();

            UserControl uc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                uc = (UserControl)Activator.CreateInstance(type);
                if (uc == null) continue;
                uc.DataContext = this;
                lst.Add(uc);
            }
            //lst.AddRange(base.GetUserControl());
            return lst.ToArray();
        }

        #endregion

        public new void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }

        public SerialCommunication(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
        public virtual void OnRaiseDataReceivedEvent(Events.DataReceivedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior     
        {
            EventHandler<Events.DataReceivedEventArgs> handler = RaiseDataReceivedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.                   if (handler != null)     
            {
                handler(this, e);
            }

        }
    }
}
