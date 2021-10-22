using AutomationControls.Attributes;
using AutomationControls.Communication.Pipes;
using AutomationControls.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Communication
{

    [DataProfile(typeof(AutomationControls.Communication.Pipes.NamedPipeClientStreamDataControl))]
    public class PipeClientCommunicator : NamedPipeClientStreamData, ICommunications, ISerializable
    {

        public PipeClientCommunicator()
        {
            IsConnected = false;

        }

        public Task OpenCommunicationChannelAsync()
        {
            return Task.Run(() => { OpenCommunicationChannel(); }, CancellationToken.None);
        }

        #region ICommunications Members

        public bool IsBusy
        {
            get
            {
                return false;
            }
        }

        private bool _ConnectAtStartup = false;
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; }
        }

        public async void OpenCommunicationChannel()
        {
            try { await pipeServer.ConnectAsync(); }
            catch { }
        }



        public void CloseCommunicationChannel()
        {
            try { pipeServer.Close(); } catch { }
        }

        public bool IsChannelOpen
        {
            get { return pipeServer.IsConnected; }
        }

        public async void SendString(string command, string destination = "")
        {
            try
            {
                byte[] b = Encoding.ASCII.GetBytes(command);
                await pipeServer.WriteAsync(b, 0, b.Count());
            }
            catch { }

        }

        public void SendBytes(byte[] b, string destination = "")
        {
            try
            {
                pipeServer.Write(b, 0, b.Count());
            }
            catch { }
        }

        public string ReadString()
        {
            return "";
            //pipeServer.ReadAsync()
        }

        public Task SendStringAsync(string command, string destination = "")
        {
            return Task.Run(() => { }, CancellationToken.None);
        }

        public async Task SendBytesAsync(byte[] b, string destination = "")
        {
            await pipeServer.WriteAsync(b, 0, b.Count());
        }

        public Task<string> ReadStringAsync()
        {
            return Task.Factory.StartNew<string>(() =>
            {
                return "";
            });
        }

        public System.Windows.Controls.UserControl GetUserControl()
        {
            return new NamedPipeClientStreamDataControl() { DataContext = this };
        }

        #endregion

        public new void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }

        public PipeClientCommunicator(SerializationInfo info, StreamingContext context) : base(info, context) { }




        public Enums.DeviceCommunications _bus
        {
            get
            {
                return Enums.DeviceCommunications.Pipe;
            }

        }

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
