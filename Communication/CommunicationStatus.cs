namespace AutomationControls.Communication
{
    public class Status
    {
        public enum ConnectionStatus
        {
            Disconnected,
            Connected,
            Connecting,
            Disconnecting,
            Sending,
            Receving,
            ConnectionFailed,
            DisconnectFailed
        }
    }
}
