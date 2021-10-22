using AutomationControls.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomationControls.Communication
{
    /// <summary>
    /// Interaction bufferic for CommunicationMonitor.xaml
    /// </summary>
    public partial class CommunicationMonitor : UserControl, INotifyPropertyChanged
    {
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

        private IProgress<string> progressSend = new Progress<string>();
        private IProgress<string> progressReceive = new Progress<string>();

        private string _sendBuffer;
        public string sendBuffer
        {
            get { return _sendBuffer; }
            set
            {
                _sendBuffer = value;
                var v = _sendBuffer.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);

                if (v.Count() > 1)
                {
                    _sendBuffer = v.Last();
                    foreach (var vv in v)
                    {
                        if (String.IsNullOrEmpty(vv)) continue;
                        commTracker.Add(new CommunicationTracker() { buffer = vv, color = Colors.Green });
                    }

                    switch (cbInvert.IsChecked)
                    {
                        case true:
                            rtb.ClearText();
                            for (int i = commTracker.Count() - 1; i >= 0; i--) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                            break;
                        case false:
                            {
                                rtb.ClearText();
                                for (int i = 0; i < commTracker.Count(); i++) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                                break;
                            }
                    }
                }
                OnPropertyChanged("sendBuffer");
            }
        }

        private string _receiveBuffer;
        public string receiveBuffer
        {
            get { return _receiveBuffer; }
            set
            {
                _receiveBuffer = value;
                var v = _receiveBuffer.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (v.Count() >= 1)
                {
                    _sendBuffer = v.Last();
                    foreach (var vv in v)
                    {
                        if (String.IsNullOrEmpty(vv)) continue;
                        commTracker.Add(new CommunicationTracker() { buffer = vv, color = Colors.Red });
                    }
                    switch (cbInvert.IsChecked)
                    {
                        case true:
                            rtb.ClearText();
                            for (int i = commTracker.Count() - 1; i >= 0; i--) { rtb.TextAppend(commTracker[i].buffer, commTracker[i].color); }
                            break;
                        case false:
                            {
                                rtb.ClearText();
                                for (int i = 0; i < commTracker.Count(); i++) { rtb.TextAppend(commTracker[i].buffer, commTracker[i].color); }
                                break;
                            }
                    }
                    _receiveBuffer = v.Last();
                }
                OnPropertyChanged("receiveBuffer");
            }
        }

        private List<CommunicationTracker> _commTracker = new List<CommunicationTracker>();
        public List<CommunicationTracker> commTracker
        {
            get { return _commTracker; }
            set
            {
                _commTracker = value;
                OnPropertyChanged("commTracker");
            }
        }

        public CommunicationMonitor()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            ((Progress<String>)progressReceive).ProgressChanged += progressReceive_ProgressChanged;
            ((Progress<String>)progressSend).ProgressChanged += progressSend_ProgressChanged;
            this.DataContext = this;
        }

        public void Load(IProgress<string> send, IProgress<string> receive)
        {
            commTracker = new List<CommunicationTracker>();
            rtb.ClearText();
            progressSend = send;
            progressReceive = receive;
            Initialize();
        }

        private void progressSend_ProgressChanged(object sender, string e) { sendBuffer = e; }

        private void progressReceive_ProgressChanged(object sender, string e) { receiveBuffer = e; }

        private void cbReceive_Click(object sender, RoutedEventArgs e)
        {
            switch (cbInvert.IsChecked)
            {
                case true:
                    rtb.ClearText();
                    for (int i = commTracker.Count() - 1; i >= 0; i--) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                    scrollViewer.ScrollToTop();
                    break;
                case false:
                    {
                        rtb.ClearText();
                        for (int i = 0; i < commTracker.Count(); i++) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                        scrollViewer.ScrollToEnd();
                        break;
                    }
            }
        }

        private void cbInvert_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.IsChecked)
            {
                case true:
                    rtb.ClearText();
                    for (int i = commTracker.Count() - 1; i >= 0; i--) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                    ScrollViewer.ScrollToTop();
                    break;
                case false:
                    {
                        rtb.ClearText();
                        for (int i = 0; i < commTracker.Count(); i++) { rtb.TextAppend(commTracker[i].buffer + Environment.NewLine, commTracker[i].color); }
                        ScrollViewer.ScrollToEnd();
                        break;
                    }
            }
        }

        private ScrollViewer scrollViewer;
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (this.scrollViewer == null)
                {
                    DependencyObject obj = this.rtb;

                    do
                    {
                        if (VisualTreeHelper.GetChildrenCount(obj) > 0)
                            obj = VisualTreeHelper.GetChild(obj as Visual, 0);
                        else
                            return null;
                    }
                    while (!(obj is ScrollViewer));

                    this.scrollViewer = obj as ScrollViewer;
                }

                return this.scrollViewer;
            }
        }
    }

    public class CommunicationTracker : INotifyPropertyChanged
    {
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

        private String _buffer = "";
        public String buffer
        {
            get { return _buffer; }
            set
            {
                _buffer = value;
                OnPropertyChanged("buffer");
            }
        }

        private Color _color;
        public Color color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged("color");
            }
        }

        public CommunicationTracker()
        {

        }


    }
}
