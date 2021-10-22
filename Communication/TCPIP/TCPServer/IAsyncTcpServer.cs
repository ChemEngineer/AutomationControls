namespace AutomationControls.Communication.TCPIP.Interfaces
{
    public interface IAsyncTcpServer
    {
        void Dispose();
        void SerializeJSON(string path);
        void DeserializeJSON(string path);

        bool Start();

        void Stop();
    }
}
