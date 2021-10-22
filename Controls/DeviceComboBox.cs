using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System.Linq;
using System.Windows.Controls;

namespace AutomationControls.Controls
{
    public class DeviceComboBox : ComboBox
    {
        public DeviceComboBox()
        {
            this.DropDownOpened += (sender, e) =>
            {
                SerializationSurrogateControlData data = DataContext as SerializationSurrogateControlData;
                if (data == null) return;
                if (this.Items.Count > 0) return;
                foreach (ISerializationSurrogate surr in data.lstSurrogates)
                {
                    foreach (var v in surr.getList)
                    {
                        var r = v.GetType().CustomAttributes.Where(x => x.AttributeType == typeof(DeviceAttribute));
                        if (r.Count() > 0)
                        {
                            this.AddChild(v.GetType().Name);
                        }
                    }
                }
            };
        }


    }
}
