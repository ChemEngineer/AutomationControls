using AutomationControls.Extensions;
using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationControls.Communication.Pipes
{
    public class PipeClient
    {
        NamedPipeClientStream clientStream;

        #region PropertyChanged Pattern

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        private String _log;
        public String log
        {
            get { return _log; }
            set
            {
                _log = value;
                OnPropertyChanged("log");
            }
        }



        private string _pipeName;
        public string pipeName
        {
            get { return _pipeName; }
            set
            {
                _pipeName = value;
                clientStream = new NamedPipeClientStream(_pipeName);
            }
        }


        public async Task SendAsync(string s, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
        {
            await clientStream.ConnectAsync();
            byte[] b = Encoding.ASCII.GetBytes(s);
            if (!clientStream.IsConnected) await clientStream.ConnectAsync(status, ct);
            if (clientStream.IsConnected)
                await clientStream.WriteAsync(b, 0, b.Length, ct);
            return;
        }
    }
}