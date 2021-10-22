using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Extensions
{
    public static partial class Extensions
    {
        public static async Task OpenAsync(this SerialPort sp, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            Task task = Task.Run(() =>
            {
                if (sp.IsOpen)
                {
                    if (progress != null) progress.Report("Serial Port " + sp.PortName + " is already connected" + Environment.NewLine);
                    return;
                }
                try
                {
                    if (progress != null) progress.Report("Connecting" + Environment.NewLine);
                    sp.Open();

                    if (sp.IsOpen) if (progress != null) progress.Report("Connected" + Environment.NewLine);
                        else if (progress != null) progress.Report("Unable to connect to " + sp.PortName);
                }
                catch (Exception e)
                {
                    if (progress != null) progress.Report("Unable to connect " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                }
            }, ct);
            await task;
        }

        public static async Task CloseAsync(this SerialPort sp, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            Task task = Task.Run(() =>
            {
                if (!sp.IsOpen)
                {
                    if (progress != null) progress.Report("Serial Port " + sp.PortName + " is already closed" + Environment.NewLine);
                    return;
                }
                try
                {
                    sp.Close();
                    if (progress != null) progress.Report(sp.PortName + " closed" + Environment.NewLine);
                }
                catch (Exception e)
                {
                    if (progress != null) progress.Report("Unable to disconnect " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                }
            }, ct);
            await task;
        }

        public static async Task WriteAsync(this SerialPort sp, byte[] buffer, int offset, int count, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            Task task = Task.Run(() =>
            {
                if (sp.IsOpen)
                    try
                    {
                        sp.Write(buffer, offset, count);
                        if (progress != null) progress.Report("Serial Port " + sp.PortName + " write successful " + Encoding.UTF8.GetString(buffer) + Environment.NewLine);
                        return;
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report("Unable to write to " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                    }
            }, ct);
            await task;
        }

        public static async Task WriteAsync(this SerialPort sp, String s, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            Task task = Task.Run(() =>
            {
                if (sp.IsOpen)
                    try
                    {
                        sp.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
                        if (progress != null) progress.Report("Serial Port " + sp.PortName + " write successful " + s + Environment.NewLine);
                        return;
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report("Unable to write to " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                    }
            }, ct);
            await task;
        }

        public static async Task SendLineAsync(this SerialPort sp, String s, String terminator = "\r\n", IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            Task task = Task.Run(() =>
            {
                if (sp.IsOpen)
                    try
                    {
                        byte[] b = Encoding.ASCII.GetBytes(s + "\r\n");
                        sp.Write(b, 0, b.Length);
                        if (progress != null) progress.Report(sp.PortName + " write successful " + s + Environment.NewLine);
                        return;
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report("Unable to write to " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                    }
            }, ct);
            await task;
        }


        public static async Task<byte[]> WriteWithResponseAsync(this SerialPort sp, byte[] sendBuffer, int bytesToReceive, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {
            return await Task<byte[]>.Factory.StartNew(() =>
            {
                List<byte> ret = new List<byte>();
                if (sp.IsOpen)
                    try
                    {
                        sp.Write(sendBuffer, 0, sendBuffer.Length);
                        if (progress != null) progress.Report("Serial Port " + sp.PortName + " write successful " + Encoding.UTF8.GetString(sendBuffer) + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report("Unable to write to " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                        return ret.ToArray();
                    }

                while (ret.Count < bytesToReceive)
                {
                    if (sp.BytesToRead > 0)
                    {
                        string s = sp.ReadByte().ToString();
                        //if(progressSend != null) progressSend.Report(s);
                        ret.Add(Byte.Parse(s));
                    }
                }
                return ret.ToArray();
            }, ct);
        }

        public static async Task WriteWithResponseAsync(this SerialPort sp, string sendBuffer, IProgress<string> progress = null, IProgress<string> receiveBuffer = null, CancellationToken ct = default(CancellationToken))
        {
            await Task<string>.Factory.StartNew(() =>
            {
                string ret = "";
                if (sp.IsOpen)
                    try
                    {
                        sp.Write(Encoding.ASCII.GetBytes(sendBuffer), 0, sendBuffer.Length);
                        if (progress != null) progress.Report("Serial Port " + sp.PortName + " write successful " + sendBuffer + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        if (progress != null) progress.Report("Unable to write to " + sp.PortName + Environment.NewLine + e.ToString() + Environment.NewLine);
                        return ret;
                    }

                while (ret.Length == 0)
                {
                    while (sp.BytesToRead == 0) { Thread.Sleep(250); }
                    while (sp.BytesToRead > 0) { ret += sp.ReadExisting(); Thread.Sleep(1000); }

                    if (sp.BytesToRead == 0) break;
                }
                if (receiveBuffer != null) receiveBuffer.Report(ret);
                return ret;
            }, ct);
        }

        public static Task ReceiveMonitor(this SerialPort sp, IProgress<string> progress = null, CancellationToken ct = default(CancellationToken))
        {

            return Task<string>.Factory.StartNew(() =>
           {
               string ret = string.Empty;
               while (sp.IsOpen)
               {
                   if (ct.IsCancellationRequested)
                       break;
                   if (sp.BytesToRead == 0)
                   {
                       Thread.Sleep(250);
                   }
                   else
                   {
                       while (true)
                       {
                           int next = sp.BaseStream.ReadByte();
                           if (next == -1) break;
                           else
                           {
                               if (progress != null)
                                   progress.Report(((char)next).ToString());
                           }
                       }
                   }
               }
               return ret;
           }, ct);
        }

        public static async Task<string> WaitForDelimiter(this SerialPort sp, string delimiter, CancellationToken ct, IProgress<string> progress = null, int maxBufferSize = 1024)
        {
            string ret = string.Empty;
            await Task<string>.Factory.StartNew(() =>
            {
                while (sp.IsOpen)
                {
                    if (ct.IsCancellationRequested) return ret;
                    if (sp.BytesToRead == 0)
                    {
                        Thread.Sleep(50);
                    }
                    else
                    {
                        try
                        {
                            while (!ct.IsCancellationRequested && ret.Count() < maxBufferSize)
                            {
                                int next = sp.BaseStream.ReadByte();
                                if (next == -1) break;

                                ret += Encoding.ASCII.GetString(new byte[] { (byte)next });
                                if (ret.EndsWith(delimiter))
                                {
                                    if (progress != null)
                                        progress.Report(ret);

                                    return ret;
                                }
                            }
                        }
                        catch { }
                    }
                }
                return ret;
            }, ct);
            return ret;
        }
    }
}