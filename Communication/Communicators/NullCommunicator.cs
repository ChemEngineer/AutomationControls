using AutomationControls.Events;
using AutomationControls.Interfaces;
using System;
using System.Threading.Tasks;

namespace AutomationControls.Communication
{
    public class NullCommunicator : ICommunications
    {

        #region ICommunications Members

        public bool IsBusy
        {
            get { return false; }
        }

        public void OpenCommunicationChannel()
        {

        }

        public void CloseCommunicationChannel()
        {

        }

        public bool IsChannelOpen
        {
            get { return false; }
        }

        public void SendString(string command, string destination = "")
        {

        }

        public void SendBytes(byte[] b, string destination = "")
        {

        }

        public string ReadString()
        {
            return "N/A";
        }

        public System.Windows.Controls.UserControl GetUserControl()
        {
            return new System.Windows.Controls.UserControl();
        }


        public Task SendStringAsync(string command, string destination = "")
        {
            return Task.Delay(1);
        }

        public Task SendBytesAsync(byte[] b, string destination = "")
        {
            return Task.Delay(1);
        }

        public Task<string> ReadStringAsync()
        {
            return null;
        }

        #endregion
        //Used to buffer and recieve commands 
        #region DataReceived Event Arguments

        public event EventHandler<DataReceivedEventArgs> RaiseDataReceivedEvent;
        public virtual void OnRaiseDataReceivedEvent(DataReceivedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior     
        {
            EventHandler<DataReceivedEventArgs> handler = RaiseDataReceivedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.                   if (handler != null)     
            {
                handler(this, e);
            }

        }

        public Enums.DeviceCommunications _bus
        {
            get
            {
                return Enums.DeviceCommunications.None;
            }

        }

        private bool _ConnectAtStartup = false;
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; }
        }

        private String _ReadDelimiter;
        public String ReadDelimiter
        {
            get { return _ReadDelimiter; }
            set
            {
                if (value != ReadDelimiter)
                {
                    _ReadDelimiter = value;
                }
            }
        }

        #endregion


        public Task OpenCommunicationChannelAsync()
        {
            return Task.Delay(1);
        }



        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {

        }
    }
}
