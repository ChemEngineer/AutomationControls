using System;

namespace AutomationControls.Events
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs() { }

        private string _buffer;
        public string buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        private string _recent;
        public string recent
        {
            get { return _recent; }
            set { _recent = value; }
        }
    }
}
