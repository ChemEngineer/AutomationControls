using AutomationControls.Extensions;
using System;
using AutomationControls.Programs.Codex.Code;
using System.IO;
using System.Linq;
using System.Text;
using AutomationControls.Interfaces;
using AutomationControls.Codex.Data;

namespace AutomationControls.Codex.Code
{
    public class MvcCoreEF
    {
        static string serializePath = @"C:\SerializedData\MvcCoreEF";

        public static string GenerateIndex(CodexData data)
        {
            string s = "";

            return s;
        }

        public static string GenerateTable(CodexData data)
        {

            string btn = "<input type = \"button\" value = \"Create\" onclick = \"@(\"window.location.href = '\" + @Url.Action(\"Create\", \""+ data.className + "\") + \"'\"); \" />";
                     string pattern = @"
@model " + "List<" + data.csNamespaceName + ".Models." + data.className + @">" + Environment.NewLine +
btn + Environment.NewLine + "<form>\n" +
  @"<table>
         <thead>
             <tr>
                 *pth*
             </tr>
         </thead>
        <tbody>
            @foreach(var v in Model)
            {
                <tr>
                    *ptr*
                </tr>
            }
        </tbody>";
            string tr = "";
            string th = "";
            foreach (var v in data.lstProperties)
            {
                if (v.IsList)
                {
                    tr += @"<td>
                    <select>
                            @foreach (var vv in v." + v.name + @")
                            {
                                <option>@vv</option>
                            }
                    </select>
                    </td>";
                }
                else
                {
                    tr += "<td>@v." + v.name + "</td>\n";                  
                    th += "<th>" + v.name + "</th>\n";
                }
            }
            tr += "<td><button type = \"submit\" class=\"btn btn-primary\" asp-action=\"update\" asp-route-id=\"@v.Id\">Edit</button></td>";
            tr += "<td><button type = \"submit\" class=\"btn btn-primary\" asp-action=\"delete\" asp-route-id=\"@v.Id\">Delete</button></td>";
            string ret = pattern.Replace("*pth*", th).Replace("*ptr*", tr);
            ret += "</table>\n</form>";
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className,  "Index.cshtml"));

            return ret;
        }

        public static string Generate_DbContext(CodexData data)
        {
            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);
            props += Environment.NewLine;
            props += AutomationControls.Codex.Code.CS.GenerateProperties(data);
            enums += AutomationControls.Codex.Code.CS.Enums(data);

            string ret =  Utilities.GenerateClassCS(data, Properties.Resources.MvcDbContext, enums,props,implement,init);
            ret.ToFile(Path.Combine(serializePath, data.className, "DbContext", data.className + "DbContext.cs"));
            return ret;
        }


        public static string Generate_Model(CodexData data)
        {
            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);

            props += Environment.NewLine;
            props += AutomationControls.Codex.Code.CS.PropertiesEFCore(data);
            enums += AutomationControls.Codex.Code.CS.Enums(data);

            string ret =  Utilities.GenerateClassCS(data, Properties.Resources.MvcModelEF,enums, props,implement,init);
            ret.ToFile(Path.Combine(serializePath, data.className, "Models", data.className + "Model.cs"));
            return ret;
        }

        public static string Generate_Config(CodexData data)
        {
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcConfig);
            ret.ToFile(Path.Combine(serializePath, data.className , "Config.txt"));
            return ret;
        }

        public static string Generate_Controller(CodexData data)
        {      
            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcControllerEF, enums, props, implement, init);
            ret.ToFile(Path.Combine(serializePath, data.className, "Controllers", data.className + "Controller.cs"));
            return ret;
        }

        public static string Generate_SeedExtension(CodexData data)
        {
           string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcSeedExtension, enums, props, implement, init);
            ret.ToFile(Path.Combine(serializePath, data.className,  "Extensions", data.className + "Extensions.cs"));
            return ret;
        }

        internal static string Generate_EFCoreExtension(CodexData data)
        {
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.EFCoreSerializationExtensions);
            ret.ToFile(Path.Combine(serializePath, data.className, "Extensions","SerializationExtensions.cs"));
            return ret;
        }

        internal static string Generate_IDataRepository(CodexData data)
        {
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcIDataRepository);
            ret.ToFile(Path.Combine(serializePath, data.className, "Repository", "IDataRepository.cs"));
            return ret;
        }

        internal static string Generate_DataRepository(CodexData data)
        {
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcDataRepository);
            ret.ToFile(Path.Combine(serializePath, data.className, "Repository", data.className + "DataRepository.cs"));
            return ret;
        }

        #region views

        internal static string Generate_MainListCshtml(CodexData data, IControlGenerator generator)
        {

            string ret = "";
            string model = Properties.Resources.MvcMainCshtml;
            string props = "";
            string props2 = "";

            foreach(var v in data.lstProperties)
            {
                props += "<th>" + v.name + "</th>";
                props2 += "<td>@v." + v.name + "</td>";
            }

            ret = ret.Replace("*CL*", data.className)
                .Replace("*NS*", data.csNamespaceName)
                .Replace("*PROP*", props)
                .Replace("*ISerializable*", AutomationControls.Codex.Code.CS.ISerialization(data));
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "List.cshtml"));
            return ret;
        }

        internal static string Generate_EditView(CodexData data, IControlGenerator generator)
        {
            string ctrls =  generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcUpdateView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Update.cshtml"));
            return ret;
        }

        internal static string Generate_DetailsView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcEditView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className,  "Read.cshtml"));
            return ret;
        }

        internal static string Generate_CreateView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcCreateView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Create.cshtml"));
            return ret;
        }

        internal static string Generate_UpdateView(CodexData data, MvcFormTagHelperControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcUpdateView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Update.cshtml"));
            return ret;
        }

        #endregion
        public class MvcTagHelperControlGenerator : IControlGenerator
        {
            private string WrapWithBsGridRow(string s) { return "<div class=\"row\">\n" + s + "\n </div>"; }
            private string WrapWithBsGridCol(string s) { return "<div class=\"col-md-1\">\n" + s + "\n </div>"; }
            private string WrapWithBsFluidContainer(string s) { return "<div class=\"container-fluid\">\n" + s + "\n </div>"; }

            public string GenerateAll(CodexData data)
            {
                string ret = "";
                foreach(var v in data.lstProperties)
                {
                    string s = "";
                     if(v.type == "bool")
                    {
                        s += GenerateLabel(v);
                        s += GenerateCheckBox(v);
                        ret += WrapWithBsGridCol(s);
                    }
                    else if (v.type == "DateTime")
                    {
                        s += GenerateLabel(v);
                        s += GenerateDateTimePicker(v);
                        ret += WrapWithBsGridCol(s);
                    }
                    else if(v.IsEnum)
                    {
                        s += GenerateDropDownListFromEnum(v);
                        ret += WrapWithBsGridCol(s);
                    }
                    else if (v.IsList)
                    {
                        s += GenerateDropDownList(v);
                        ret += WrapWithBsGridCol(s);
                    }
                    else
                    {
                        s += GenerateLabel(v);
                        s += GenerateTextBox(v);
                        ret += WrapWithBsGridCol(s);
                    }
                }
                return WrapWithBsFluidContainer(ret);
            }

            public string GenerateCheckBox(PropertiesData data)
            {
                throw new NotImplementedException();
            }

            public string GenerateDateTimePicker(PropertiesData data)
            {
                throw new NotImplementedException();
            }

            public string GenerateDropDownList(PropertiesData data)
            {
                return "@Html.DropDownList(\"" + data.name + "\", new SelectList(" + data.name + ")";
            }

            public string GenerateDropDownListFromEnum(PropertiesData data)
            {
                return "@Html.DropDownList(\"" + data.name  + "\", new SelectList(Enum.GetValues(typeof("+ data.name  +")))";
            }

            public string GenerateLabel(PropertiesData data)
            {
                return "@Html.LabelFor(x => x." + data.name + ")" + Environment.NewLine;
            }

            public string GenerateRadioButton(PropertiesData data)
            {
                string ret = "@Html.RadioButtonFor(x => x." + data.name + ", True)" + Environment.NewLine;
                ret += "@Html.RadioButtonFor(x => x." + data.name + ", False)" + Environment.NewLine;
                return ret;
            }

            public string GenerateTextBox(PropertiesData data)
            {
                return "@Html.TextBoxFor(x => x."+ data.name +")" + Environment.NewLine;
            }
        }

        public class MvcFormTagHelperControlGenerator : IControlGenerator
        {
            private string WrapWithForm(string s, CodexData data, string action) { return " <form asp-controller=\""+ data.className +"\" asp-action=\"" + action +"\" method=\"post\" >\n" + s + "\n </div>"; }
            private string WrapWithFormRow(string s) { return "<div class=\"form-group row\">\n" + s + "\n </div>"; }
            private string WrapWithDiv(string s) { return "<div class=\"col-sm-10\">\n" + s + "\n </div>"; }

            public string GenerateAll(CodexData data)
            {
                string ret = "";
                foreach (var v in data.lstProperties)
                {
                    string s = "";
                    if (v.type == "bool")
                    {
                        s += GenerateLabel(v);
                        s += WrapWithDiv(GenerateCheckBox(v));
                        ret += WrapWithFormRow(s);
                    }
                    else if (v.type == "DateTime")
                    {
                        s += GenerateLabel(v);
                        s += WrapWithDiv(GenerateDateTimePicker(v));
                        ret += WrapWithFormRow(s);
                    }
                    else if (v.IsEnum)
                    {
                        s += GenerateDropDownListFromEnum(v);
                        ret += WrapWithFormRow(s);
                    }
                    else if (v.IsList)
                    {
                        s += GenerateDropDownList(v);
                        ret += WrapWithFormRow(s);
                    }
                    else
                    {
                        s += GenerateLabel(v);
                        s += WrapWithDiv(GenerateTextBox(v));
                        ret += WrapWithFormRow(s);
                    }
                }
                return ret;
                //return WrapWithForm(ret,data, "ACTION");
            }

            public string GenerateCheckBox(PropertiesData data)
            {
                throw new NotImplementedException();
            }

            public string GenerateDateTimePicker(PropertiesData data)
            {
                throw new NotImplementedException();
            }

            public string GenerateDropDownList(PropertiesData data)
            {
                return "@Html.DropDownList(\"" + data.name + "\", new SelectList(" + data.name + ")";
            }

            public string GenerateDropDownListFromEnum(PropertiesData data)
            {
                return "@Html.DropDownList(\"" + data.name + "\", new SelectList(Enum.GetValues(typeof(" + data.name + ")))";
            }

            public string GenerateLabel(PropertiesData data)
            {
                return "<label asp-for=\"" +  data.name + "\" class=\"col-sm-2 col-form-label\">" + data.name +"</label>" + Environment.NewLine;
            }

            public string GenerateRadioButton(PropertiesData data)
            {
                string ret = "@Html.RadioButtonFor(x => x." + data.name + ", True)" + Environment.NewLine;
                ret += "@Html.RadioButtonFor(x => x." + data.name + ", False)" + Environment.NewLine;
                return ret;
            }

            public string GenerateTextBox(PropertiesData data)
            {
                return "<input asp-for=\"" +  data.name + "\" class=\"form-control\"/>" + Environment.NewLine;
            }
        }

      
    }
}