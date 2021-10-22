using System;
using System.Windows.Controls;

namespace AutomationControls.Devices.LED
{
    /// <summary>
    /// Interaction logic for ucArduinoCommandsListBox.xaml
    /// </summary>
    public partial class WS2812CommandsListBox : UserControl
    {
        public WS2812CommandsListBox()
        {
            InitializeComponent();
            this.lb.ItemsSource = Commands.EnumerateCommands();
            this.lb.SelectionChanged += (sender, e) =>
            {
                String sel = lb.SelectedItem.ToString();
                int cmd = Commands.ParseCommandName(sel);
                OnRaiseCommandChangedEvent(new CommandChangedEventArgs() { command = cmd });
            };
        }


        public static class Commands
        {

            public enum CommandEnum
            {
                SolidFill = 1,
                StreakUp = 2,
                StreakDown = 3,
                RainbowDown = 4,
                RainbowUp = 5,
                ColorFlowDown = 6,
                ColorFlowUp = 7,
                CycleHalves = 8,
                Cycle = 9,
                FromMiddle = 10,
                FromEnds = 11,
                BandUp = 12,
                BandDown = 13,
                BouncingBall = 14,
                FadeToBlack = 15,
                FadeRandom = 16,
                Fire = 17,
                RandomTransition = 18,
                Fade2 = 19,
                Random = 20,
                Individual = 21,
                WarpCore = 22,
                Reset = 200,
                Reset2 = 201,
            }

            //public enum CommandEnum
            //{
            //    SolidFill = 1,
            //    StreakUp = 2,
            //    StreakDown = 3,
            //    RainbowDown = 4,
            //    RainbowUp = 5,
            //    ColorFlowDown = 6,
            //    ColorFlowUp = 7,
            //    CycleHalves = 8,
            //    Cycle = 9,
            //    FromMiddle = 10,
            //    FromEnds = 11,
            //    BandUp = 12,
            //    BandDown = 13,
            //    BouncingBall = 14,
            //    Fire = 16,
            //    Ice = 17,
            //    Dim = 18,
            //    Dim2 = 19,
            //    Random = 20,
            //    Reset = 200,
            //    Reset2 = 201,
            //}


            public static string[] EnumerateCommands() { return Enum.GetNames(typeof(CommandEnum)); }

            public static int ParseCommandName(String cmd)
            {
                CommandEnum res;
                if (Enum.TryParse(cmd, out res)) 
                    return (int)res;
                return 1;
            }
        }

        public class CommandChangedEventArgs : EventArgs
        {
            public CommandChangedEventArgs() { }
            private int _command;
            public int command
            {
                get { return _command; }
                set { _command = value; }
            }
        }
        public event EventHandler<CommandChangedEventArgs> RaiseCommandChangedEvent;
        protected virtual void OnRaiseCommandChangedEvent(CommandChangedEventArgs e)   // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior
        {
            EventHandler<CommandChangedEventArgs> handler = RaiseCommandChangedEvent; // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            if (handler != null) { handler(this, e); }
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WS2812LED data = DataContext as WS2812LED;
            if (data != null)
            {
                if (lb.SelectedItem != null)
                {
                    data.command = Commands.ParseCommandName(lb.SelectedItem.ToString());
                    data.SendCommand();
                }
            }
        }

    }
}
