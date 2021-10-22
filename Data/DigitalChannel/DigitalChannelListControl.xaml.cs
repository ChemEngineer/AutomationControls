using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using AutomationControls.WPF;
using System.Linq;
using System.Windows.Controls;

namespace AutomationControls.Controllers.DataClasses
{

    public partial class DigitalChannelListControl : UserControl
    {

        public DigitalChannelListControl() : base()
        {
            InitializeComponent();


            dg.SelectionChanged += (sender, e) =>
            {
                DigitalChannelList data = DataContext as DigitalChannelList;
                if (data == null) return;

                dp.Children.Clear();
                if (dg.SelectedItem != null) { dp.Children.Add(DataBinding.Generate_UserControl(dg.SelectedItem)); }
            };


            db.DataReadyEvent += (sender, e) =>
            {
                DigitalChannelList data = DataContext as DigitalChannelList;
                if (data == null) return;

                foreach (ISerializationSurrogate surr in db.sscdata.lstSurrogates)
                {
                    foreach (var v in surr.getList)
                    {
                        var r = v.GetType().CustomAttributes.Where(x => x.AttributeType == typeof(DigitalChannelsAttribute));
                        if (r.Count() > 0)
                        {
                            if (!data.digitalChannelNames.Contains(v.GetType().Name))
                            {
                                data.digitalChannelNames.Add(v.GetType().Name);
                            }
                        }
                    }
                }
            };
        }



    }
}