using AutomationControls.Controllers.DataClasses;
using AutomationControls.Enums;
using AutomationControls.EventArguments;
using System;

namespace AutomationControls.Interfaces
{
    public interface IDigitalChannels
    {
        DigitalChannelList DigitalChannels { get; set; }
    }

    public interface IDigitalChannel
    {
        string PinDesignation { get; set; }
        DigitalState State { get; set; }
        CommunicationDirection Direction { get; set; }
        EventHandler<DigitalStateChangedEventArgs> RaiseDigitalStateChangedEvent { get; set; }
    }


    //public interface IDigitalInput  : IDigitalChannel
    //{
    //    EventHandler<DigitalState> StateChanged { get; }    
    //}

    //public interface IDigitalOutput : IDigitalChannel
    //{
    //    void SetState(DigitalState state);  
    //}
}
