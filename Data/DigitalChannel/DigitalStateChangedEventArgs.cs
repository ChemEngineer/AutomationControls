using AutomationControls.Enums;
using System;

namespace AutomationControls.EventArguments
{

    public class DigitalStateChangedEventArgs : EventArgs
    {
        public DigitalStateChangedEventArgs() { }
        private DigitalState _data;
        public DigitalState data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
