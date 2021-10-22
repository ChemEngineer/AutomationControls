using AutomationControls.Attributes;
using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class GenerateExtensions
        {
            public static UserControl[] GenerateControl(this IProvideUserControls obj)
            {
                List<UserControl> lst = new List<UserControl>();
                UserControl cc = new UserControl();
                List<DataProfileAttribute> lstAttr = new List<DataProfileAttribute>();
                lstAttr.AddRange((DataProfileAttribute[])obj.GetType().GetCustomAttributes(typeof(DataProfileAttribute), false));
                var res = obj.GetType().GetProperties().Select(x => (DataProfileAttribute[])x.PropertyType.GetCustomAttributes(typeof(DataProfileAttribute), false));
                res.ForEach(x => lstAttr.AddRange(x));

                foreach (var attr in lstAttr)
                {
                    var type = ((DataProfileAttribute)attr).ClassName;
                    cc = (UserControl)Activator.CreateInstance(type);
                    if (cc == null) continue;
                    lst.Add(cc);
                    cc.DataContext = obj;
                }
                return lst.ToArray();
            }
        }
    }
}
