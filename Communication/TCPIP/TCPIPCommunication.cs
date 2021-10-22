using AutomationControls.Attributes;
using AutomationControls.Communication.TCPIP;
using AutomationControls.Communication.TCPIP.UserControls;
using AutomationControls.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Communication
{
    [Serializable]
    [DataProfile(typeof(TCPClient))]
    public class TCPIPCommunication : TcpClientVM, ICommunications, ISerializable
    {
        #region ICommunications Members

        public Enums.DeviceCommunications _bus
        {
            get
            {
                return Enums.DeviceCommunications.TCPIP;
            }

        }

        public Task OpenCommunicationChannelAsync()
        {
            return Task.Run(() => { OpenCommunicationChannel(); }, CancellationToken.None);
        }

        public bool IsBusy
        {
            get { return false; }
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        public void OpenCommunicationChannel()
        {
            var dat = (this as TcpClientVM);
            dat.Connect();
            //cts.Cancel();
            // cts = new CancellationTokenSource();
            //  dat.socket.ReceiveAsync(dat.progressReceive, dat.progressSend, cts.Token);
        }

        public void CloseCommunicationChannel()
        {
            (this as TcpClientVM).Disconnect();
        }

        public bool IsChannelOpen
        {
            get { return (this as TcpClientVM).socket.Connected; }
        }

        public void SendString(string command, string destination = "")
        {
            if (!IsChannelOpen)
                OpenCommunicationChannel();
            (this as TcpClientVM).Send(command);
        }

        public void SendBytes(byte[] b, string destination = "")
        {
            if (!IsChannelOpen)
            {
                OpenCommunicationChannel();
                (this as TcpClientVM).SendBytes(ref b);
            }
            else { (this as TcpClientVM).SendBytes(ref b); }
        }

        public string ReadString()
        {
            return (this as TcpClientVM).Receive();
        }

        public UserControl GetUserControl()
        {
            TCPClient uc = new TCPClient();
            uc.DataContext = this;
            return uc;
        }

        public TCPIPCommunication()
            : base()
        {

        }

        public TCPIPCommunication(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public Task SendStringAsync(string command, string destination = "")
        {
            if (!IsChannelOpen)
                OpenCommunicationChannel();
            return (this as TcpClientVM).SendAsync(command, cts.Token);
        }

        public Task SendBytesAsync(byte[] b, string destination = "")
        {
            if (!IsChannelOpen)
                OpenCommunicationChannel();
            return (this as TcpClientVM).SendAsync(b, cts.Token);
        }

        public Task<string> ReadStringAsync()
        {
            if (!IsChannelOpen)
                OpenCommunicationChannel();
            return (this as TcpClientVM).ReceiveAsync(100);
        }

        #endregion

        //public event EventHandler<Events.DataReceivedEventArgs> RaiseDataReceivedEvent;
        //public virtual void OnRaiseDataReceivedEvent(Events.DataReceivedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior     
        //{
        //    EventHandler<Events.DataReceivedEventArgs> handler = RaiseDataReceivedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.                   if (handler != null)     
        //    {
        //        handler(this, e);
        //    }
        //}
    }
}