using AutomationControls.Codex.Data;
using AutomationControls.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class Xamarin
    {
        static string serializePath = @"C:\SerializedData\Xamarin";
        
        public static string GenerateXamlControl(CodexData data)
        {
            string path = serializePath;
            data.csNamespaceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => path = System.IO.Path.Combine(path, x));
            path = System.IO.Path.Combine(path, data.className);

            string ret = "";
            string xaml = Properties.Resources.XamarinXamlBase.Replace("*namespace*", data.csNamespaceName).Replace("*class*", data.className); 

            string controls = GenerateControls(data);
            xaml = xaml.Replace("*content*", controls);

            string xamlcs = GenerateXamlCs(data);

            string dataClass = Generate_Data_Class(data);

            ret += xaml + Environment.NewLine;
            ret += dataClass;

            xaml.ToFile(path + "\\" + data.className + "Control.xaml");
            xamlcs.ToFile(path + "\\" + data.className + "Control.xaml.cs");

            dataClass.ToFile(path + "\\" + data.className + ".cs");
            //Process.Start(path);

            ret = GenerateListControl(data);
            return ret;
        }

        public static string GenerateListControl(CodexData data)
        {
            string path = serializePath;
            data.csNamespaceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => path = System.IO.Path.Combine(path, x));
            path = System.IO.Path.Combine(path, data.className);

            string ret = "";
            string xaml = Properties.Resources.XamarinXamlListBase.Replace("*namespace*", data.csNamespaceName)
                .Replace("*class*", data.className + "List")
                .Replace("*propname*", data.lstProperties[0].name)
                .Replace("*properties*", GenerateControls(data));

            string xamlcs = GenerateListXamlCs(data);

            ret += xaml + Environment.NewLine + Environment.NewLine + xamlcs;

            xaml.ToFile(path + "\\" + data.className + "ListControl.xaml");
            xamlcs.ToFile(path + "\\" + data.className + "ListControl.xaml.cs");

            return ret;
        }
        private static string GenerateXamlCs(CodexData data)
        {
            return Properties.Resources.XamarinXamlCs.Replace("*ClassName*", data.className).Replace("*NS*", data.csNamespaceName).Replace("*methods*", "");
        }

        private static string GenerateListXamlCs(CodexData data)
        {
            string s=  Properties.Resources.XamarinXamlCs.Replace("*ClassName*", data.className + "List").Replace("*NS*", data.csNamespaceName);

            string methods = @"
            private void miEdit_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new " + data.className  + @"Control() { BindingContext = lv" + data.className  + @"List.SelectedItem });
        }

        private void miDelete_Clicked(object sender, System.EventArgs e)
        {
            var mi = ((MenuItem)sender);
            ((" + data.className + @"List)lv" + data.className + @"List.ItemsSource).Remove((" + data.className + @")lv" + data.className + @"List.SelectedItem);
        }

        private void btnAdd_Clicked(object sender, System.EventArgs e)
        {
            ((" + data.className + @"List)lv" + data.className + @"List.ItemsSource).Add(new " + data.className + @"());
        }";
             
            return s.Replace("*methods*", methods);
        }


        private static string GenerateControls(CodexData data)
        {
            int i = 0;
            string s = "";
            foreach (PropertiesData x in data.lstProperties)
            {
                if (x.type.Contains("bool", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_Grid_Switch(x, i);
                else
                {
                    s += Generate_XAML_Grid_TextEntry(x, i);
                }
                s += Environment.NewLine;
                i++;
            };
            return s;
        }

        #region Grid

        private static string Generate_XAML_Grid_Switch(PropertiesData x, int row)
        {
            string s = "<Label Text = \"" + x.name + "\" Grid.Column=\"0\" Grid.Row=\"" + row + "\"  HorizontalTextAlignment=\"End\" VerticalTextAlignment=\"Center\" />" + Environment.NewLine;
            s += "<Switch IsToggled=\"{Binding  " + x.name + "}\" Grid.Column=\"1\" Grid.Row=\"" + row + "\" />" + Environment.NewLine;
            return s;
        }

        private static string Generate_XAML_Grid_TextEntry(PropertiesData x, int row)
        {
            string s = "<Label Text = \"" + x.name + "\" Grid.Column=\"0\" Grid.Row=\"" + row + "\"  HorizontalTextAlignment=\"End\" VerticalTextAlignment=\"Center\" />" + Environment.NewLine;
            s += "<Entry Text=\"{ Binding " + x.name + "}\" Grid.Column=\"1\" Grid.Row=\"" + row + "\" />" + Environment.NewLine;
            return s;
        }

        #endregion

        #region StackLayout

        private static string Generate_XAML_StackLayout_Switch(PropertiesData x)
        {
            string s = "<StackLayout Orientation=\"Horizontal\" >" + Environment.NewLine;
            s += "<Label Text = \"" + x.name + "\"  HorizontalOptions = \"Start\" VerticalOptions = \"Center\" />" + Environment.NewLine;
            s += "<Switch IsToggled=\"{Binding  " + x.name + "}\" HorizontalOptions = \"Start\" VerticalOptions = \"Center\" />" + Environment.NewLine;
            s += "</StackLayout>" +Environment.NewLine;
            return s;
        }

        private static string Generate_XAML_StackLayout_TextEntry(PropertiesData x)
        {
            string s = "<StackLayout Orientation=\"Horizontal\" >" + Environment.NewLine;
            s += "<Label Text = \"" + x.name + "\"  HorizontalOptions = \"Start\" VerticalOptions = \"Center\" />" + Environment.NewLine;
            s += "<Entry Text=\"{ Binding " + x.name + "}\" HorizontalOptions = \"Center\"  VerticalOptions = \"Center\" />";
            s += "</StackLayout>" + Environment.NewLine;
            return s;
        }

        #endregion

        private static string Generate_Data_Class(CodexData data)
        {
           

            StringBuilder s = new StringBuilder();

            string ret = "";
            data.IsISerializable = true;
            data.IsNotifyPropertyChanged = true;

            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);

            if (data.IsNotifyPropertyChanged)
            {
                implement += " INotifyPropertyChanged";
                props = AutomationControls.Codex.Code.CS.PropertyChangedPattern(data);
            }
            if (data.IsISerializable) { implement += ", ISerializable"; }

            props += Environment.NewLine;
            props += AutomationControls.Codex.Code.CS.GenerateProperties(data);
            enums += AutomationControls.Codex.Code.CS.Enums(data);

            string ns = data.csNamespaceName;
            if (!string.IsNullOrEmpty(data.extendedNamespace))
                ns += "." + data.extendedNamespace;

            ret = AutomationControls.Properties.Resources.XamarinDataClass
                 .Replace("*CL*", data.className)
                 .Replace("*ENUM*", enums)
                 .Replace("*init*", init)
                 .Replace("*implement*", implement)
                 .Replace("*NS*", ns)
                 .Replace("*PROP*", props)
                 .Replace("*ISerializable*", AutomationControls.Codex.Code.CS.ISerialization(data));
            s.Append(ret);

           
            return ret;
        }

    }
}
