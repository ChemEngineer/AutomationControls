using AutomationControls.Extensions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Communication.Pipes
{
    public class PipeServer
    {
        public static int DEFAULT_TIME_OUT = 50000;
        public static string LOCAL_SERVER = ".";

        public static async void SendMessageAsync(string pipeName, string message)
        {
            try
            {
                using (var pipe = new NamedPipeClientStream(LOCAL_SERVER, pipeName,
                PipeDirection.Out, PipeOptions.Asynchronous))
                using (var stream = new StreamWriter(pipe))
                {
                    pipe.Connect(DEFAULT_TIME_OUT);

                    // write the message to the pipe stream 
                    await stream.WriteAsync(message);
                }
            }
            catch
            {

            }
        }

        public static async Task StartListeningAsync(string pipeName, Action<string> messageRecieved, IProgress<string> progress = default(IProgress<string>), CancellationToken ct = default(CancellationToken))
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 20, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    {
                        // wait for the connection
                        await pipe.WaitForConnectionAsync(progress, ct);

                        var str = await pipe.GetStringAsync(progress, ct);

                        if (progress != null) { progress.Report(str); }
                        if (messageRecieved != null) { messageRecieved(str); }
                        if (pipe.IsConnected) { pipe.Disconnect(); }
                    }
                }
                catch { }
            }
        }
    }
}