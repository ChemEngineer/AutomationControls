using System.Threading;

namespace AutomationControls.Servers
{
    public interface IAsyncTcpServer
    {
        CancellationTokenSource cts { get; set; }
        void Dispose();
        void SerializeJSON(string path);
        void DeserializeJSON(string path);

        string Identify();

        bool Start();

        void Stop();
    }
}
