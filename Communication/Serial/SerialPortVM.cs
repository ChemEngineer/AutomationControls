using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomationControls.Extensions;

namespace AutomationControls.Communication.Serial
{
    public interface ISerialPortVM
    {
        //Object properties
       // System.IO.Stream BaseStream { get; }
        System.Int32 BaudRate { get; set; }
        System.Boolean BreakState { get; set; }
        System.Int32 BytesToWrite { get; }
        System.Int32 BytesToRead { get; }
        System.Boolean CDHolding { get; }
        System.Boolean CtsHolding { get; }
        System.Int32 DataBits { get; set; }
        System.Boolean DiscardNull { get; set; }
        System.Boolean DsrHolding { get; }
        System.Boolean DtrEnable { get; set; }
        System.String Encoding { get; set; }
        System.IO.Ports.Handshake Handshake { get; set; }
        System.Boolean IsOpen { get; }
        System.String NewLine { get; set; }
        System.IO.Ports.Parity Parity { get; set; }
        System.Byte ParityReplace { get; set; }
        System.String PortName { get; set; }
        System.Int32 ReadBufferSize { get; set; }
        System.Int32 ReadTimeout { get; set; }
        System.Int32 ReceivedBytesThreshold { get; set; }
        System.Boolean RtsEnable { get; set; }
        System.IO.Ports.StopBits StopBits { get; set; }
        System.Int32 WriteBufferSize { get; set; }
        System.Int32 WriteTimeout { get; set; }
        
        // Object Methods
        void Open();
        void Close();

        // My properties
        string[] changedProperties {get; set; }
        // My methods
        void Send(String text);
         void Send(byte[] b);
         void SendByte(int b);
        string[] GetEncodings();
        byte[] GetBytes(string text);
        string ReadAll();

    }

    [GuidAttribute("1A585C4D-3371-48dc-AF8A-AFFECC1B0967")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface SerialPortVMEvents
    {
        void Say(string name, string value);
    }

    [ComVisible(true)]
    [Guid("F2B64FA9-3C98-423C-B536-C80438C32B48")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(SerialPortVMEvents))]
    public class SerialPortVM : SerialPort, ISerialPortVM
    {
        
       public delegate void SayDelegate(string name, string value);
       public event SayDelegate Say;
       public void CauseSay(string name, string value)
        {
            try  {  if(Say != null) Say(name, value);  } catch {  }
        }
       
        public CancellationTokenSource cts = new CancellationTokenSource();
        public Progress<String> progressSend = new Progress<string>();
        public Progress<String> progressReceive = new Progress<string>();

        string[] lastPropertyValues = new string[0];
        public SerialPortVM()
        {
            //this.PortName = SerialPort.GetPortNames()[0];
            //this.BaudRate = 115200;                                
            //typeof(TCP.TcpClientVM).CreateCOMInterfaceDefinition().ToFile(@"c:\interface.txt");
        }

        #region Extension Wrappers

        private List<string> _changedProperties = new List<string>();
        public string[] changedProperties
        {
            get { return _changedProperties.ToArray(); }
            set { _changedProperties.Clear(); }
        }

       // string[] changedProperties { get; set; }
        Thread MonitorPropertiesThread;
        public new void Open()
        {
            base.Open();
            if(IsOpen)
            {
                MonitorPropertiesThread = MonitorProperties();
                MonitorPropertiesThread.Start();
            }
        }

        public new void Close()
        {
            MonitorPropertiesThread.Abort();
            base.Close();
        }

        public Thread MonitorProperties()
        {
            Thread t1 = new Thread(() =>
            {
                ISerialPortVM isp = this as ISerialPortVM;
                PropertyInfo[] pi = isp.GetType().GetProperties();

                List<string> lst = new List<string>();
                foreach(var v in pi)
                {
                    var obj = v.GetValue(this);
                    if(obj == null)
                    {
                        lst.Add("");  // placeholder
                        continue;
                    }
                    else lst.Add(obj.ToString());
                    _changedProperties.Add(v.Name);
                    _changedProperties = _changedProperties.Distinct().ToList();
                    CauseSay(v.Name, obj.ToString());
                }
                lastPropertyValues = lst.ToArray();
                do
                {
                    int i = 0;
                    foreach(var v in pi)
                    {
                        if(IsOpen)
                        {
                            var val = v.GetValue(isp);
                            if(val == null) { i++; continue; }
                            if(val.ToString() != lastPropertyValues.ToArray()[i])     // Property change occurs here
                            {
                                _changedProperties.Add(v.Name);
                                _changedProperties = _changedProperties.Distinct().ToList();
                                CauseSay(v.Name, val.ToString());
                            }
                            lastPropertyValues[i] = val.ToString();
                            i++;
                        }
                        Thread.Sleep(10);
                    }
                } while(true);
            });
            return t1;
        }

        public byte[] GetBytes(string text)  { return base.Encoding.GetBytes(text); }

        public void Send(String text)
        {
            if(!IsOpen) return;
             byte[] b = base.Encoding.GetBytes(text);
            this.Write(b,0,b.Length );
        }

        public void Send(byte[] b)
        {
            this.Write(b, 0, b.Length );
        }

        public void SendByte(int b)
        {
            this.Write(new[] { (byte)b },0 ,1);
        }

        public  string ReadAll()
        {
            string ret = string.Empty;
            int ct = BytesToRead;
            for(int i = 0; i < ct ; i++ )
            {
               Byte b = (byte)ReadByte();
               char c = base.Encoding.GetChars(new[] { b })[0];
               ret += c;
            }
            return ret;
        }
        #endregion

        #region Async Extension Wrappers

        public async Task SendAsync(String text)
        {
            if(!IsOpen) return;
            cts.Cancel(); cts = new CancellationTokenSource();
            await this.WriteAsync(text, progressSend, cts.Token);
        }

        private async Task SendLineAsync(string s)
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            await this.SendLineAsync(s, "\n", progressSend);
        }

        private Task CloseAsync()
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            return this.CloseAsync(progressSend, cts.Token);
        }

        private Task OpenAsync()
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            return this.OpenAsync(progressSend, cts.Token);
        }

        private Task OpenAsync(String portName)
        {
            cts.Cancel(); cts = new CancellationTokenSource();
            PortName = portName;
            return this.OpenAsync(progressSend, cts.Token);
        }

        #endregion

        public new String Encoding
        {
            get { return base.Encoding.ToString(); }
            set { base.Encoding = System.Text.Encoding.GetEncoding(value); }
        }
         
        public string[] GetEncodings()
        {
            List<string> lst = new List<string>();         
            var res = System.Text.Encoding.GetEncodings().Select(x => x.Name);
            return res.ToArray();         
        }
    }
}
