using AutomationControls.Codex.Data;
using AutomationControls.Extensions;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class CS
    {
        static string serializePath = @"C:\SerializedData\Generated\CSharp\";

        private static string eventNameKey = "*EventName*";

        static int tabCount = 0;
        public static string DefaultConstructor(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            /////////////'Default constructor/////////////////
            s.AppendLine("public " + data.className + "()");
            s.AppendLine("{");
            s.AppendLine("}");
            s.AppendLine();

            return s.ToString();
        }

        public static string StartConstructor(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("public " + data.className + "()");
            s.AppendLine("{");
            return s.ToString();
        }

        public static string EndConstructor(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("}");
            s.AppendLine();
            return s.ToString();
        }

        #region Properties
        public static string GenerateProperties(CodexData data)
        {
            string ret = "";

            if (data.IsNotifyPropertyChanged)
                ret = Properties(data);
            else
                ret = PropertiesSimple(data);
            return ret;
        }

        private static string Properties(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            int ctr = 1;

            if (data.IsNotifyPropertyChanged)
                s.AppendLine(PropertyChangedPattern(data));
            //////////////// Property Declarations ///////////////
            foreach (PropertiesData o in data.lstProperties)
            {

                s.AppendLine(Tab(ctr) + "private " + o.type + " " + "_" + o.name + ";");
                s.AppendLine(Tab(ctr) + "public " + o.type + " " + o.name);
                s.AppendLine(Tab(ctr) + "{"); ctr += 1;
                s.AppendLine(Tab(ctr) + "get { return _" + o.name + "; }");
                s.AppendLine(Tab(ctr) + "set");
                s.AppendLine(Tab(ctr) + "{");
                ctr += 1;
                s.AppendLine(Tab(ctr) + "_" + o.name + " = value;");

                if ((bool)data.IsNotifyPropertyChanged)
                    s.AppendLine(Tab(ctr) + "OnPropertyChanged(\"" + o.name + "\");");

                ctr -= 1;
                s.AppendLine(Tab(ctr) + "}");
                ctr -= 1;
                s.AppendLine(Tab(ctr) + "}");
                s.AppendLine();
            }
            return s.ToString();
        }

        private static string PropertiesSimple(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            int ctr = 1;
            //////////////// Property Declarations ///////////////
            foreach (PropertiesData o in data.lstProperties)
            {
                s.AppendLine(Tab(ctr) + "public " + o.type + " " + o.name + "{ get; set; }");
            }
            return s.ToString();
        }
        #endregion
        public static string PropertiesEFCore(CodexData data)   
        {
            StringBuilder s = new StringBuilder();
            int ctr = 1;
            //////////////// Property Declarations ///////////////
            foreach (PropertiesData o in data.lstProperties)
            {
                if (o.IsList)
                {
                    s.AppendLine(Tab(ctr) + "[NotMapped]");
                    s.AppendLine(Tab(ctr) + "public " + o.type + " " + o.name );
                    s.AppendLine(Tab(ctr) + "{");
                    s.AppendLine(Tab(ctr++) + "get { return lstStr.FromCSV(); }");
                    s.AppendLine(Tab(ctr) + "set { lstStr = value.ToCSV(); }");
                    s.AppendLine(Tab(ctr--) + "}");
                    s.AppendLine(Tab(ctr) + "public string "+ o.name + "Str { get; set; }");
                }
                else
                {
                    s.AppendLine(Tab(ctr) + "public " + o.type + " " + o.name + "{ get; set; }");
                }
            }
            return s.ToString();
        }

        public static string EFCoreSerializationExtensions(CodexData data)
        {
            return AutomationControls.Properties.Resources.EFCoreSerializationExtensions;
        }

        public static string Using(CodexData data)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("using System;");
            s.AppendLine("using System.ComponentModel;");
            s.AppendLine("using System.Collections.Generic;");
            s.AppendLine("using "+ data.csNamespaceName + ".Interfaces;");
            if ((bool)data.IsISerializable) s.AppendLine("using System.Runtime.Serialization;");

            return s.ToString();
        }

        public static string PropertyChangedPattern(CodexData data)
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine("#region PropertyChanged Pattern");
            s.AppendLine();
            s.AppendLine(Tab(tabCount) + "public event PropertyChangedEventHandler PropertyChanged;");
            s.AppendLine();
            s.AppendLine(Tab(tabCount) + "protected void OnPropertyChanged(string name)");
            s.AppendLine(Tab(tabCount) + "{"); tabCount += 1;
            s.AppendLine(Tab(tabCount) + "PropertyChangedEventHandler handler = PropertyChanged;");
            s.AppendLine(Tab(tabCount) + "if (handler != null)");
            s.AppendLine(Tab(tabCount) + "{"); tabCount += 1;
            s.AppendLine(Tab(tabCount) + "handler(this, new PropertyChangedEventArgs(name));"); tabCount -= 1;
            s.AppendLine(Tab(tabCount) + "}"); tabCount -= 1;
            s.AppendLine(Tab(tabCount) + "}");
            s.AppendLine();
            s.AppendLine("#endregion");
            s.AppendLine();
            return s.ToString();
        }

        public static string Tab(int NumOfTabs)
        {
            string ret = null;
            if (NumOfTabs > 0)
            {
                for (int i = 1; i <= NumOfTabs; i++)
                {
                    ret += ("   ");
                }
            }
            return ret;
        }

        public static string ISerialization(CodexData data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public void GetObjectData(SerializationInfo info, StreamingContext context)   ");
            sb.AppendLine("{");
            foreach (var v in data.lstProperties)
            {
                sb.AppendLine("info.AddValue(\"" + v.name + "\"," + "_" + v.name + ", typeof(" + v.type + "));");
            }
            sb.AppendLine("}");

            sb.AppendLine("public  " + data.className + "(SerializationInfo info, StreamingContext context)");
            sb.AppendLine("{");
            foreach (var v in data.lstProperties)
            {
                sb.AppendLine("_" + v.name + "= (" + v.type + ") info.GetValue(\"" + v.name + "\", typeof(" + v.type + "));");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string ISerializationSimple(CodexData data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public void GetObjectData(SerializationInfo info, StreamingContext context)   ");
            sb.AppendLine("{");
            foreach (var v in data.lstProperties)
            {
                sb.AppendLine("info.AddValue(\"" + v.name + "\","  + v.name + ", typeof(" + v.type + "));");
            }
            sb.AppendLine("}");

            sb.AppendLine("public  " + data.className + "(SerializationInfo info, StreamingContext context)");
            sb.AppendLine("{");
            foreach (var v in data.lstProperties)
            {
                sb.AppendLine( v.name + "= (" + v.type + ") info.GetValue(\"" + v.name + "\", typeof(" + v.type + "));");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }


        public static String Event(String eventName) { return CodeFormatter.FormatPattern(AutomationControls.Properties.Resources.CS_Event_Pattern.Replace(eventNameKey, eventName)); }

        internal static string Enums(CodexData data)
        {
            string ret = "";

            foreach (var v in data.lstProperties.Where(x => x.IsEnum))
            {
                ret += "public enum " + v.type + Environment.NewLine;
                ret += "{" + Environment.NewLine;
                foreach (var vv in v.lstEnum)
                {
                    ret += vv.value + " = " + vv.position;
                    if (vv != v.lstEnum.Last())
                        ret += ",";
                    ret += Environment.NewLine;
                }
                ret += "}" + Environment.NewLine + Environment.NewLine;
            }
            return ret;
        }

        internal static string Enums(PropertiesData data)
        {
            string ret = "";
            ret += "public enum " + data.type + Environment.NewLine;
            ret += "{" + Environment.NewLine;
            foreach (var vv in data.lstEnum)
            {
                ret += vv.value + " = " + vv.position;
                if (vv != data.lstEnum.Last())
                    ret += ",";
                ret += Environment.NewLine;
            }
            ret += "}" + Environment.NewLine + Environment.NewLine;
            return ret;
        }


        internal static string GenerateSurrogate(CodexData data)
        {
            string path = serializePath;
            data.csNamespaceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => path = System.IO.Path.Combine(path, x));
            path = System.IO.Path.Combine(path, data.className);

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
            props += AutomationControls.Codex.Code.CS.Properties(data);
            enums += AutomationControls.Codex.Code.CS.Enums(data);

            string ns = data.csNamespaceName;
            if (!string.IsNullOrEmpty(data.extendedNamespace))
                ns += "." + data.extendedNamespace;

            ret = AutomationControls.Properties.Resources.SerializationSurrogate
                 .Replace("*CL*", data.className)
                 .Replace("*ENUM*", enums)
                 .Replace("*init*", init)
                 .Replace("*implement*", implement)
                 .Replace("*NS*", ns)
                 .Replace("*PROP*", props)
                 .Replace("*ISerializable*", AutomationControls.Codex.Code.CS.ISerialization(data));
            s.Append(ret);
            ret.ToFile(path + "\\" + data.className + ".cs");

            ///
            ret = AutomationControls.Properties.Resources.UserControlTemplate;
            ret = ret.Replace("*ClassName*", data.className).Replace("*NS*", ns);
            s.Append(ret);
            ret.ToString().ToFile(path + "\\" + data.className + "Control.xaml.cs");

            ret = AutomationControls.Properties.Resources.DataControlXAML;
            ret = ret.Replace("*ClassName*", data.className).Replace("*NS*", ns).Replace("*DataGrid*", AutomationControls.Codex.Code.xaml.Generate_XAML_Custom_Properties(data.lstProperties));
            ret.ToString().ToFile(path + "\\" + data.className + "Control.xaml");
            s.Append(ret);

            ret = AutomationControls.Properties.Resources.UserControlListTemplate;
            ret = ret.Replace("*ClassName*", data.className).Replace("*NS*", ns);
            s.Append(ret);
            ret.ToString().ToFile(path + "\\" + data.className + "ListControl.xaml.cs");
            // tbCodexCodeWindow.Text = s.ToString();

            ret = AutomationControls.Properties.Resources.ListControl;
            ret = ret.Replace("*ClassName*", data.className).Replace("*NS*", ns).Replace("*DataGrid*", AutomationControls.Codex.Code.xaml.Generate_XAML_DatgGrid_Properties(data.lstProperties)).Replace("DockPanel", "DataGrid");
            ret.ToString().ToFile(path + "\\" + data.className + "ListControl.xaml");
            s.Append(ret);

            //Process.Start(serializePath);
            return s.ToString();
        }

        internal static string GenerateDataClass(CodexData data, string serializePath = "")
        {
            string originalClassName = data.className;
            if (!data.className.EndsWith("Data"))
                data.className += "Data";
            string path = "";
            data.csNamespaceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => path = System.IO.Path.Combine(path, x));
            path = System.IO.Path.Combine(path, data.className);

            StringBuilder s = new StringBuilder();

            string ret = "";
            string init = "";
            string implement = "";
            string ser = "";
            string properties = "";
            string enums = Code.CS.Enums(data);
           
            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);

            if (data.IsNotifyPropertyChanged)
            {
                implement += " INotifyPropertyChanged";
            }
            if (data.IsISerializable && data.IsNotifyPropertyChanged)
            {
                implement += ", ISerializable";
                ser = AutomationControls.Codex.Code.CS.ISerialization(data);
                properties = AutomationControls.Codex.Code.CS.Properties(data);
            }
            else if (data.IsISerializable && !data.IsNotifyPropertyChanged)
            {
                implement += " ISerializable";
                ser = AutomationControls.Codex.Code.CS.ISerializationSimple(data);
                properties = AutomationControls.Codex.Code.CS.PropertiesSimple(data);
            }
            else if (data.IsEntityFramework)
            {
                properties = Code.CS.GenerateProperties(data);
            }
            else { 
                properties = AutomationControls.Codex.Code.CS.PropertiesSimple(data);
            }

            //enums += AutomationControls.Codex.Code.CS.Enums(data);
           
            string ns = data.csNamespaceName;
            if (!string.IsNullOrEmpty(data.extendedNamespace))
                ns += "." + data.extendedNamespace;

            ret = AutomationControls.Properties.Resources.csDataTemplate
                 .Replace("*CL*", data.className)
                 .Replace("*ENUM*", enums)
                 .Replace("*INIT*", "")
                 .Replace("*IMPLEMENT*", implement)
                 .Replace("*NS*", ns)
                 .Replace("*PROP*",properties)
                 .Replace("*ISerializable*",ser);
            s.Append(ret);
            ret.ToFile(path + "\\" + data.className + ".cs");

            if (!string.IsNullOrEmpty(serializePath))
                ret.ToFile(serializePath);

            //Process.Start(serializePath);
            data.className = originalClassName;
            return s.ToString();
        }
    }
}