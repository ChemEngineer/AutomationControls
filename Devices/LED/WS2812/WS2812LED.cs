using AutomationControls.Attributes;
using AutomationControls.BaseClasses;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;


namespace AutomationControls.Devices.LED
{

    //[DataProfileAttribute(typeof(WS2812Control))]
    //[DataProfileAttribute(typeof(WS2812CommandsListBox))]
    //[DataProfileAttribute(typeof(WS2812ColorsListBox))]
    //[DataProfileAttribute(typeof(WS2812Sliders))]
    [Serializable]
    [DataProfileAttribute(typeof(WS2812CompositeControl))]
    [DataProfileAttribute(typeof(DeviceBaseControl))]
    public class WS2812LED : DeviceBase, System.Runtime.Serialization.ISerializable, IProvideUserControls
    {

        public WS2812LED()
        {

        }

        private int _command;
        public int command
        {
            get { return _command; }
            set
            {
                _command = value;
                OnPropertyChanged("command");
            }
        }

        private int _red;
        public int red
        {
            get { return _red; }
            set
            {
                _red = value;
                OnPropertyChanged("red");
            }
        }

        private int _green;
        public int green
        {
            get { return _green; }
            set
            {
                _green = value;
                OnPropertyChanged("green");
            }
        }

        private int _blue;
        public int blue
        {
            get { return _blue; }
            set
            {
                _blue = value;
                OnPropertyChanged("blue");
            }
        }

        private int _delay;
        public int delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                OnPropertyChanged("delay");
            }
        }

        private int _brightness;
        public int brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                OnPropertyChanged("brightness");
            }
        }

        private int _streakLength;
        public int streakLength
        {
            get { return _streakLength; }
            set
            {
                _streakLength = value;
                OnPropertyChanged("streakLength");
            }
        }

        private int _cooling;
        public int cooling
        {
            get { return _cooling; }
            set
            {
                _cooling = value;
                OnPropertyChanged("cooling");
            }
        }

        private int _heat;
        public int heat
        {
            get { return _heat; }
            set
            {
                _heat = value;
                OnPropertyChanged("heat");
            }
        }

        public byte[] Create_Serial_Command()
        {
            return new byte[] {
                (byte)this.command ,
                (byte)this.delay ,
                (byte)this.red ,
                (byte)this.green ,
                (byte)this.blue ,
                (byte)this.brightness ,
                (byte)this.streakLength ,
                (byte)this.cooling ,
                (byte)this.heat ,
                (byte)'|' ,
                (byte)'|' ,
                 (byte)'|'
            };
        }

        public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("command", _command, typeof(int));
            info.AddValue("red", _red, typeof(int));
            info.AddValue("green", _green, typeof(int));
            info.AddValue("blue", _blue, typeof(int));
            info.AddValue("delay", _delay, typeof(int));
            info.AddValue("brightness", _brightness, typeof(int));
            info.AddValue("streakLength", _streakLength, typeof(int));
            info.AddValue("cooling", _cooling, typeof(int));
            info.AddValue("heat", _heat, typeof(int));
        }

        public WS2812LED(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _command = (int)info.GetValue("command", typeof(int));
            _red = (int)info.GetValue("red", typeof(int));
            _green = (int)info.GetValue("green", typeof(int));
            _blue = (int)info.GetValue("blue", typeof(int));
            _delay = (int)info.GetValue("delay", typeof(int));
            _brightness = (int)info.GetValue("brightness", typeof(int));
            _streakLength = (int)info.GetValue("streakLength", typeof(int));
            _cooling = (int)info.GetValue("cooling", typeof(int));
            _heat = (int)info.GetValue("heat", typeof(int));
        }

        public new UserControl[] GetUserControls()
        {
            List<UserControl> lst = new List<UserControl>();

            UserControl uc = new UserControl();
            var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false);
            foreach (var attr in attrs)
            {
                var type = ((DataProfileAttribute)attr).ClassName;
                uc = (UserControl)Activator.CreateInstance(type);
                if (uc == null) continue;
                uc.DataContext = this;
                lst.Add(uc);
            }

            return lst.ToArray();
        }


        DateTime LastCommandTime = DateTime.Now;
        int MillisecondDelay = 25;
        public void SendCommand()
        {
            TimeSpan ts = DateTime.Now - LastCommandTime;
            if (ts > TimeSpan.FromMilliseconds(MillisecondDelay))
            {
                comm.SendBytesAsync(Create_Serial_Command());
                LastCommandTime = DateTime.Now;
                OnCommandExecutedEvent(new CommandExecutedEventArgs());
            }
        }

        public void SendCommand(WS2812LED dat)
        {
            this.blue = dat.blue;
            this.brightness = dat.brightness;
            this.command = dat.command;
            this.cooling = dat.cooling;
            this.delay = dat.delay;
            this.green = dat.green;
            this.heat = dat.heat;
            this.red = dat.red;
            this.streakLength = dat.streakLength;

            TimeSpan ts = DateTime.Now - LastCommandTime;
            if (ts > TimeSpan.FromMilliseconds(MillisecondDelay))
            {
                comm.SendBytesAsync(Create_Serial_Command());
                LastCommandTime = DateTime.Now;
            }
        }



        #region CommandExecutedEventHandler

        public delegate void CommandExecutedEventHandler(object sender, CommandExecutedEventArgs a);
        public event EventHandler<CommandExecutedEventArgs> CommandExecutedEvent;
        protected virtual void OnCommandExecutedEvent(CommandExecutedEventArgs e)
        {
            EventHandler<CommandExecutedEventArgs> handler = CommandExecutedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public class CommandExecutedEventArgs : EventArgs
        {
            public CommandExecutedEventArgs() { }
            private string _Message;
            public string Message { get { return _Message; } set { _Message = value; } }
        }

        #endregion


        public override string ToString()
        {
            return Encoding.ASCII.GetString(Create_Serial_Command());
        }
    }
}
