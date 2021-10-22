using AutomationControls.Enums;
using AutomationControls.Events;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomationControls.Interfaces
{
    public interface ICommunications : ISerializable
    {
        bool IsBusy { get; }

        DeviceCommunications _bus { get; }

        void OpenCommunicationChannel();
        Task OpenCommunicationChannelAsync();
        void CloseCommunicationChannel();
        bool IsChannelOpen { get; }
        void SendString(string command, string destination = "");
        void SendBytes(byte[] b, string destination = "");
        string ReadString();
        Task SendStringAsync(string command, string destination = "");
        Task SendBytesAsync(byte[] b, string destination = "");
        Task<string> ReadStringAsync();
        UserControl GetUserControl();
        string ReadDelimiter { get; set; }
        //string LastError { get; }
        //string SendQuery(string command, string delimiter);
        event EventHandler<DataReceivedEventArgs> RaiseDataReceivedEvent;
        //EventHandler<DataReceivedEventArgs> RaiseDataReceivedEvent { get; set; } 
    }
}
