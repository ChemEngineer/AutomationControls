using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutomationControls.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for TextInputMessageBox.xaml
    /// </summary>
    public partial class TextInputMessageBox : UserControl
    {
        public TextInputMessageBox()
        {
            InitializeComponent();
            this.Visibility = System.Windows.Visibility.Hidden;
            this.DataContext = this;
        }

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

        private String _tbtext;
        public String tbtext
        {
            get { return _tbtext; }
            set
            {
                _tbtext = value;
                OnPropertyChanged("tbtext");
            }
        }

        private String _title;
        public String title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("title");
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            { OnRaiseResponseReadyEvent(new ResponseReadyEventArgs() { data = tbtext }); }
            base.OnKeyDown(e);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseResponseReadyEvent(new ResponseReadyEventArgs() { data = tbtext });
        }

        public void Show(string title)
        {
            this.title = title;
            this.Visibility = System.Windows.Visibility.Visible;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
        }

        public event EventHandler<ResponseReadyEventArgs> RaiseResponseReadyEvent;
        protected virtual void OnRaiseResponseReadyEvent(ResponseReadyEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior
        {
            EventHandler<ResponseReadyEventArgs> handler = RaiseResponseReadyEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            if (handler != null) { handler(this, e); }
        }
        public class ResponseReadyEventArgs : EventArgs
        {
            public ResponseReadyEventArgs() { }
            private string _data;
            public string data
            {
                get { return _data; }
                set { _data = value; }
            }
        }
    }
}
