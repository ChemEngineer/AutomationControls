using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class Extensions
        {

            public static async void ToNamedPipeAsync(this string s, string pipeName)
            {
                try
                {
                    using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous))
                    using (var stream = new StreamWriter(pipe))
                    {
                        pipe.Connect(100000);
                        await stream.WriteAsync(s);
                    }
                }
                catch { }
            }

            public static bool ToNamedPipe(this string s, string pipeName)
            {
                try
                {

                    if (s.IsNull() || pipeName.IsNull()) return false;
                    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                    pipeClient.Connect();
                    pipeClient.Write(Encoding.ASCII.GetBytes(s), 0, s.Count());
                    pipeClient.Close();
                    return true;
                }
                catch { return false; }
            }

            public static async Task WaitForConnectionAsync(this NamedPipeServerStream stream, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
            {
                var tcs = new TaskCompletionSource<bool>();
                var cancelRegistration = ct.Register(() => tcs.SetCanceled());
                var iar = stream.BeginWaitForConnection(null, null);
                var rwh = ThreadPool.RegisterWaitForSingleObject(iar.AsyncWaitHandle, delegate { tcs.TrySetResult(true); }, null, -1, true);
                try
                {
                    await tcs.Task.ConfigureAwait(false);
                    if (iar.IsCompleted)
                    {
                        stream.EndWaitForConnection(iar);
                    }
                }
                finally
                {
                    cancelRegistration.Dispose();
                    rwh.Unregister(null);
                }
            }

            public static async Task<string> GetStringAsync(this NamedPipeServerStream stream, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
            {
                string ret = "";
                await Task.Factory.StartNew(() =>
                {
                    var received = new byte[4096];
                    var bytesRead = 0;
                    bytesRead = stream.Read(received, 0, 4096);
                    System.Array.Resize(ref received, bytesRead);
                    ret = Encoding.ASCII.GetString(received);
                    if (status != null) status.Report(ret);
                });
                return ret;
            }

            public static Task ConnectAsync(this NamedPipeClientStream stream, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
            {
                return Task.Run(() =>
                {
                    if (!stream.IsConnected)
                    {
                        try
                        {
                            if (status != null) status.Report("Connecting to Pipe Server ...");
                            stream.Connect(1);
                            if (status != null) status.Report("Connected to Piper Server");
                        }
                        catch (Exception e)
                        {
                            if (status != null) status.Report(e.ToString());
                        }
                    }
                }, ct);
            }
        }
    }
}