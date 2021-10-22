using AutomationControls.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class Extensions
        {
            // State object for receiving data from remote device.
            public class StateObject
            {
                // Client socket.
                public Socket workSocket = null;
                // Size of receive buffer.
                public const int BufferSize = 256;
                // Receive buffer.
                public byte[] buffer = new byte[BufferSize];
                // Received data string.
                public StringBuilder sb = new StringBuilder();
            }

            public static SocketAwaitable ConnectAsync(this Socket socket, SocketAwaitable awaitable, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                try
                {
                    awaitable.Reset();
                    awaitable.m_eventArgs.Completed += (sender, e) =>
                    {
                        if (e.ConnectSocket != null && e.ConnectSocket.Connected) { if (progress != null) progress.Report("Connection Successful" + Environment.NewLine); awaitable.m_wasCompleted = true; }
                        else {; if (progress != null) progress.Report("Connection Failed" + Environment.NewLine); awaitable.m_wasCompleted = true; }
                    };

                    if (progress != null) progress.Report("Connecting" + Environment.NewLine);
                    switch (socket.ConnectAsync(awaitable.m_eventArgs))
                    {
                        case true:
                            while (!awaitable.m_wasCompleted)
                            {
                                Task.Delay(100);
                                if (ct.IsCancellationRequested)
                                {
                                    progress.Report("Connect Canceled" + Environment.NewLine);
                                    awaitable.m_wasCompleted = true;
                                }
                            };
                            break;
                        case false:
                            if (awaitable.m_eventArgs.ConnectSocket.Connected) { if (progress != null) progress.Report("Connection Successful" + Environment.NewLine); }
                            else if (progress != null) progress.Report("Connection Failed" + Environment.NewLine);
                            awaitable.m_wasCompleted = true;
                            break;
                    }

                }
                catch (Exception e)
                { if (progress != null) progress.Report(e.ToString()); }
                return awaitable;
            }

            public static SocketAwaitable DisconnectAsync(this Socket socket, SocketAwaitable awaitable, IProgress<string> progress = null)
            {
                awaitable.Reset();
                awaitable.m_eventArgs.Completed += (sender, e) =>
                {
                    if (e.ConnectSocket == null || !e.ConnectSocket.Connected)
                    {
                        if (progress != null) progress.Report("Disconnected" + Environment.NewLine);
                        awaitable.m_wasCompleted = true;
                    }
                    else
                    {
                        if (progress != null) progress.Report("Disconnect Failed" + Environment.NewLine);
                        awaitable.m_wasCompleted = true;
                    }
                };

                if (progress != null) progress.Report("Disconnecting" + Environment.NewLine);
                if (!socket.DisconnectAsync(awaitable.m_eventArgs))
                {
                    if (progress != null) progress.Report("Disconnected" + Environment.NewLine);
                    awaitable.m_wasCompleted = true;
                }
                else
                {
                    while (!awaitable.m_wasCompleted || socket.Connected)
                    {
                        Thread.Sleep(100);
                    }
                }
                return awaitable;
            }

            public static SocketAwaitable ReceiveAsync(this Socket socket, SocketAwaitable awaitable, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                if (progress != null) progress.Report("Receiving" + Environment.NewLine);
                awaitable.Reset();
                awaitable.m_eventArgs.Completed += (sender, e) =>
                {
                    if (progress != null) progress.Report("Received " + awaitable.m_eventArgs.BytesTransferred + " Bytes - " + Encoding.ASCII.GetString(awaitable.m_eventArgs.Buffer, 0, awaitable.m_eventArgs.BytesTransferred) + Environment.NewLine);
                    awaitable.m_wasCompleted = true;
                };

                switch (socket.ReceiveAsync(awaitable.m_eventArgs))
                {
                    case true:
                        while (!awaitable.m_wasCompleted)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                if (progress != null) progress.Report("Receive Cancelled" + Environment.NewLine);
                                awaitable.m_wasCompleted = true;

                            }
                        }
                        break;
                    case false:
                        if (progress != null) progress.Report("Received " + awaitable.m_eventArgs.BytesTransferred + " Bytes - " + Encoding.ASCII.GetString(awaitable.m_eventArgs.Buffer, 0, awaitable.m_eventArgs.BytesTransferred) + Environment.NewLine);
                        awaitable.m_wasCompleted = true;
                        break;
                }
                return awaitable;
            }

            public static async Task<bool> ConnectAsync(this Socket s, IPEndPoint endpoint, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(new byte[0x1000], 0, 0x1000);
                args.RemoteEndPoint = endpoint;
                var awaitable = new SocketAwaitable(args);
                try
                {
                    await s.ConnectAsync(awaitable, progress);
                }
                catch { return false; }
                return true;
            }

            public static async Task<bool> DisconnectAsync(this Socket s, bool reuseSocket = true, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                var args = new SocketAsyncEventArgs() { DisconnectReuseSocket = reuseSocket };
                var awaitable = new SocketAwaitable(args);

                try { await s.DisconnectAsync(awaitable, progress); } catch { return false; }
                return true;
            }

            public static async Task<String> ReceiveAsync(this Socket socket, IProgress<String> progressResponse = null, IProgress<String> progress = null, CancellationToken ct = default(CancellationToken))
            {
                if (progress != null) progress = (Progress<String>)progress;
                string response = "";
                Task<string> task = Task<string>.Factory.StartNew(() =>
                {
                    try
                    {
                        // Create the state object.
                        StateObject state = new StateObject();
                        state.workSocket = socket;
                        int bytesRead = 0;
                        do
                        {
                            // Begin receiving the data from the remote device.
                            try
                            {
                                bytesRead = state.workSocket.Receive(state.buffer);
                                if (bytesRead > 0)
                                {
                                    response = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                                    if (progressResponse != null) progressResponse.Report(response);
                                }
                                Task.Delay(200);
                                if (ct.IsCancellationRequested) break;
                            }
                            catch (Exception) { }
                        } while (true);
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report(e.Message);
                    }
                    return response;
                }, ct);
                return await task;
            }

            public static async Task<String> ReceiveAsync(this Socket s, int bytesToRead, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                // Reusable SocketAsyncEventArgs and awaitable wrapper 
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(new byte[0x1000], 0, 0x1000);
                var awaitable = new SocketAwaitable(args);

                // Do processing, continually receiving from the socket 
                do
                {
                    await s.ReceiveAsync(awaitable, progress, ct);
                } while (args.BytesTransferred < bytesToRead);
                String full = Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
                return Encoding.UTF8.GetString(args.Buffer, 0, bytesToRead);
            }

            public static async Task<string> ReceiveAsync(this Socket s, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                // Reusable SocketAsyncEventArgs and awaitable wrapper 
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(new byte[0x1000], 0, 0x1000);
                var awaitable = new SocketAwaitable(args);

                if (ct == default(CancellationToken))
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    cts.CancelAfter(5000);
                    ct = cts.Token;
                }
                // Do processing, continually receiving from the socket 
                do
                {
                    await s.ReceiveAsync(awaitable, progress);
                    int bytesRead = args.BytesTransferred;
                } while (!ct.IsCancellationRequested);
                String full = Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
                return full;
            }

            public static async Task<String> ReceiveAsync(this Socket s, TimeSpan timeout, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                // Reusable SocketAsyncEventArgs and awaitable wrapper 
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(new byte[0x1000], 0, 0x1000);
                var awaitable = new SocketAwaitable(args);

                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(timeout);
                await s.ReceiveAsync(awaitable, progress, ct);
                // Do processing, continually receiving from the socket 
                DateTime start = DateTime.Now;
                DateTime finish = start.Add(timeout);
                do { await Task.Delay(100); } while (!cts.Token.IsCancellationRequested);
                // do { await Task.Delay(100); } while(!cts.Token.IsCancellationRequested || DateTime.Now < finish);
                String full = Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
                return Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
            }

            public static byte[] ReceiveToDelimiter(this Socket s, string delimiter = "||", IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                s.ReceiveTimeout = 1000;
                List<byte> bytes = new List<byte>();
                if (s == null) bytes.ToArray();
                using (NetworkStream ns = new NetworkStream(s))
                {
                    ns.ReadTimeout = 1000;
                    bool running = true;
                    while (running)
                    {
                        if (ns.DataAvailable)
                        {
                            bytes.Add((byte)ns.ReadByte());
                            if (bytes.Last() == 124 && bytes[bytes.Count() - 1] == 124)
                                break;
                        }
                    }
                    if (bytes.Count() > 0)
                        bytes.Remove(bytes.Last());
                    if (bytes.Count() > 0)
                        bytes.Remove(bytes.Last());
                    return bytes.ToArray();
                }
            }

            public static async Task SendAsync(this Socket socket, byte[] b, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                await Task.Run(() =>
                {
                    if (b.Count() == 0 || socket == null) return;
                    try { socket.Send(b); if (progress != null) progress.Report("Sent " + b.Count() + " Bytes :" + Encoding.ASCII.GetString(b) + Environment.NewLine); } catch { if (progress != null) progress.Report("Send Failed" + b.Count() + " Bytes :" + Encoding.ASCII.GetString(b) + Environment.NewLine); }
                }, ct);
            }

            public static async Task SendAsync(this Socket socket, String s, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
            {
                await Task.Run(() =>
                {
                    if (string.IsNullOrEmpty(s) || socket == null) return;

                    byte[] b = Encoding.ASCII.GetBytes(s);
                    socket.Send(b);
                    try { socket.Send(b); if (progress != null) progress.Report("Sent " + b.Count() + " Bytes :" + Encoding.ASCII.GetString(b) + Environment.NewLine); } catch { if (progress != null) progress.Report("Send Failed" + b.Count() + " Bytes :" + Encoding.ASCII.GetString(b) + Environment.NewLine); }

                }, ct);
            }
        }
    }
}