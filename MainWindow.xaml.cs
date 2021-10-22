using AutomationControls.BaseClasses;
using AutomationControls.Interfaces;
using System;
using System.Windows;

namespace AutomationControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            //var res =    Sql.StaticMethods.GetSinceDateUAutomationControlsated(DateTime.Now.AddDays(-7));
            //var d = "";

            //   SerializationSurrogate<AutomationControls.Servers.SqlData> server = new SerializationSurrogate<Servers.SqlData>();

            // var minfos = (typeof(SerializationSurrogate<ArduinoProMini>).GetMethods());
            // (new Window() { Content = new DataGrid() { ItemsSource = minfos } }).Show();

            // foreach(var mi in minfos)
            // {
            //     var a = mi.Name;
            //     var b = mi.GetGenericArguments();
            //     var c = mi.ReturnType;
            //     var d = mi.ReflectedType;
            // }
            // ObservableProfileDictionary<ArduinoProMini> dic = new ObservableProfileDictionary<ArduinoProMini>();
            // var lst = new ObservableProfileCollection<ArduinoProMini>() { profileName = "new profile" };
            // lst.Add(new ArduinoProMini() { Name = "Tardo" });
            // dic.Add("new profile",  lst);
            // dic.Add("old profile", new ObservableProfileCollection<ArduinoProMini>() { profileName = "old profile" });
            // Serializer<ObservableProfileDictionary<ArduinoProMini>> ser = new Serializer<ObservableProfileDictionary<ArduinoProMini>>(dic);
            // var sbin =  ser.ToBinary(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\dic.bin");
            //ser.ToJSON(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\dic.json");
            // var bin = ser.FromBinary(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\dic.bin");
            // var json = ser.FromJSON(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\dic.json"); 

            //WPF.Themes.UserControls.ThemePicker tp = new WPF.Themes.UserControls.ThemePicker();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sscctrl = db.ssc;
            this.Content = sscctrl;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            db.sscdata.Deserialize();
            db.ssc.GetSerializationSurrogateControl();

            foreach (ISerializationSurrogate v in db.sscdata.lstSurrogates)
            {
                try
                {
                    //var res = v.getList.Cast<object>().Where(x => x is DeviceBase).Select(y => y);
                    var dat = v.getData;
                    var interfaces = dat.GetType().GetInterfaces();
                    DeviceBase devbase = (v.getData) as DeviceBase;
                    if (devbase == null) return;
                    if (devbase.connectAtStartup)
                        await devbase.comm.OpenCommunicationChannelAsync();
                }
                catch { continue; }
            }
        }

    }
}
