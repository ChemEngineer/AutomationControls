using AutomationControls.Codex.Data;
using AutomationControls.Extensions;
using AutomationControls.Interfaces;
using AutomationControls.Programs.Codex.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class BlazorEF
    {
        static string serializePath = @"C:\SerializedData\BlazorEF";

     
        public static string Generate_Config(CodexData data)
        {
            string basePath = Path.Combine(serializePath, data.className + "config.txt");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorEFCoreConfig);
            ret.ToFile(basePath);
            return ret;
        }

        public static string Generate_DbContext(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Server", "Models", "AppDbContext.cs");

            string ret = Properties.Resources.BlazorAppDBContext.Replace("*PROP*", "public virtual DbSet<" + data.className + "Data> lst" + data.className + " { get; set; }").Replace("*NS*", data.csNamespaceName);
            ret.ToFile(basePath);
            return ret;
        }

        public static string Generate_Service(CodexData data)
        {
            string basePath = Path.Combine(serializePath, data.className, "Components", data.className, data.className + "Service.cs");


            string ret = Utilities.GenerateClassCS(data, Properties.Resources.EFCoreService);
            ret.ToFile(basePath);
            return ret;
        }

        public static string GenerateRazorTableComponent(CodexData data)
        {
            string basePath = Path.Combine(serializePath, data.className, "Components", data.className, data.className + "ListComponent.razor");

            string ret = "";
            string pattern = @"
@page ""/*CLlower*List""

@using *NS*.Components.*CL*
@inject *CL*Service service
@inject NavigationManager nav

<button  @onclick=""Add"" >Add</button>
@if(lst != null)
{
    <p><em>*CL*</em></p>
    <table>
        <thead>
            <tr>
                *Head*
            </tr>
        </thead>
        <tbody>
            @foreach (var v in lst)
            {
                <tr>
                *Data*
                 <th><button  @onclick=""() => Delete(v.Id)"" >Delete</button></th>
                 <th><button  @onclick=""() => Edit(v.Id)""> Edit </button></th>
                 </tr>
            }
        </tbody>
    </table>
}

@code {

     public List<*CL*> lst {get; set;} = new List<*CL*>();
    
    protected override void OnInitialized()
    {
         lst =  repository.ReadAll().ToList();
        return;
    }

     public async Task Add()
    {
        var dat = new *CL*();
        lst.Add(dat);
            repository.Update(dat);
            nav.NavigateTo(""/*CLlower*edit"");
            await InvokeAsync(StateHasChanged);
        }

         public void Delete(int id)
        {
            var v = lst.Where(x => x.Id == id).First();
            lst.Remove(v);
            repository.Delete(v);
        }

         public void Edit(int id)
        {
            var data = lst.Where(x => x.Id == id).First();
            nav.NavigateTo(""/dockeredit"");          
        }
    }";

            var names = data.lstProperties.Select(x => x.name);
            string head = "";
            string dat = "";
            foreach (var name in names)
            {
                head += "<th>" + name + "</th>" + Environment.NewLine;
                dat += "<th>@v." + name + "</th>" + Environment.NewLine;
            }
            ret = pattern.Replace("*NS*", data.csNamespaceName)
                .Replace("*CL*", data.className)
                .Replace("*CLlower*", data.className.ToLower())
                .Replace("*Head*", head)
                .Replace("*Data*", dat);
            ret.ToFile(basePath);

            return ret;
        }

        public static string Generate_EditView(CodexData data)
        {
            string basePath = Path.Combine(serializePath, data.className, "Components", data.className, data.className + "Update.cshtml");

            string ctrls = new BlazorEFControlGenerator().GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcUpdateView, ctrls);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_EFCoreExtension(CodexData data)
        {
            string basePath = Path.Combine(serializePath, data.className, "Components", data.className, "SerializationExtensions.cs");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcSeedExtension);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_IDataRepository(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Server", "Interfaces", "IDataRepository.cs");
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorIDataRepository);
            ret.ToFile(basePath);

            basePath = Path.Combine(serializePath, "Server", "Interfaces", "I" + data.className + "Repository.cs");
            string s = @"
using *NS*.Data;
using *NS*.Server.Interfaces;

namespace *NS*.Server.Interfaces
{
    public interface I*CL*Repository : IDataRepository<*CL*Data>
    {
       
    }
}
";
            s = Conventions.FillTemplate(s, data, basePath);
            ret += s;
            return ret;
        }

        internal static string Generate_DataAccessLayer(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Server", "DataAccess", data.className + "DataAccessLayer.cs");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorDataAccessLayer);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_Controller(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Server", "Controllers", data.className + "Controller.cs");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorController);
            ret.ToFile(basePath);
            return ret;
        }

        #region Data Control

        internal static string Generate_DataControl(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "TableComponent.razor.cs");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorDataComponent);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_DataControlRazor(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "TableComponent.razor");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorDataComponentRazor);
            string v = "";
            data.lstProperties.ForEach(x => v += "<td>" + x.name + "</td>\n");
            string w = "";
            data.lstProperties.ForEach(x => w += "<td>@v." + x.name + "</td>\n");
            string z = "";
            data.lstProperties.ForEach(x => z += @"<tr>
                            <td>" + x.name + @"</td>
                            <td>@data." + x.name+ @"</td>
                        </tr>
");
            ret = ret.Replace("*PROPNAMES*", v).Replace("*PROPVALUES*", w).Replace("*PROPVALUES2*", z);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_DataControlCss(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "TableComponent.razor.css");

            var ret = @"
thead {
    font-size: 15px;
    font-weight: bold;
}

h1 {
    font-size: 15px;
    font-weight: bold;
}
";
            ret.ToFile(basePath);
            return ret;
        }

        #endregion

        #region Add Edit Control

        internal static string Generate_AddEditControl(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "AddEditComponent.razor.cs");

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.BlazorAddEditForm);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_AddEditControlRazor(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "AddEditComponent.razor");

            string ret =  Properties.Resources.BlazorAddEditRazor;
            var v = new BlazorEFControlGenerator().GenerateAll(data);
            ret = ret.Replace("*PROPS*", v);
            ret = Utilities.GenerateClassCS(data, ret);
            ret.ToFile(basePath);
            return ret;
        }

        internal static string Generate_AddEditControlCss(CodexData data)
        {
            string basePath = Path.Combine(serializePath, "Client", "Components", data.className, data.className + "AddEditComponent.razor.css");

            var ret = @"
h1 {
    font-size: 20px;
}
";
            ret.ToFile(basePath);
            return ret;
        }

        #endregion

        public class BlazorEFControlGenerator : IControlGenerator
        {
            static PropertiesData _data = null;
            public static string GenerateDefaultControl(PropertiesData data)
            {
                _data = data;
                string ret = "";
                string pattern = "";
                if (data.IsEnum)
                {
                    pattern = @"
<EditForm Model=""data"">
        <InputSelect @bind-Value=""data.*PName*"">
             @foreach(var v in Enum.GetValues(typeof(*PType*)))
                {
                <option> @v </option>
                }
        </InputSelect>
    </EditForm>";
                }
                else if (data.IsList)
                {
                    pattern = @"
 <select>

            @foreach(var v in service.lst)
            {
                @*<option value = @v> @v </option> *@
            }
        </select>";
                }
                else if (data.type.Contains("bool"))
                {
                    pattern = @"<input type=""checkbox"" @bind=""@data." + data.name + @""" />";
                }
                else
                {
                    pattern = @"<input @bind=""@data." + data.name + @""" />";
                }
                ret = pattern.Replace("*PType*", data.type)
                   .Replace("*PName*", data.name);
                return ret;
            }

            private string WrapWithBsGridRow(string s) { return "<div class=\"row\">\n" + s + "\n </div>"; }
            private string WrapWithBsGridCol(string s) { return "<div class=\"col-md-1\">\n" + s + "\n </div>"; }
            private string WrapWithBsFluidContainer(string s) { return "<div class=\"container-fluid\">\n" + s + "\n </div>"; }
            private string WrapWithDiv(string s) { return @"
                    <div class=""form-group row"">
        <div class=""col-md-4"">
            " + s + @"
        </div>
        <ValidationMessage For = ""@(() => data." + _data.name + @")"" />
    </div> "; }
            //        private string WrapWithDiv(string s) { return @"
            //                <div class=""form-group row"">
            //    <label class=""control-label col-md-12"">Name</label>
            //    <div class=""col-md-4"">
            //        "+ s+ @"
            //    </div>
            //    <ValidationMessage For = ""@(() => data."+ _data.name  +@")"" />
            //</div> "; }

            public string GenerateAll(CodexData data)
            {
                string ret = "";
                foreach (var v in data.lstProperties)
                {
                    string s = "";
                    if (v.type == "bool")
                    {
                        s += GenerateLabel(v);
                        s += GenerateCheckBox(v);
                        ret += WrapWithDiv(s);
                    }
                    else if (v.type == "DateTime")
                    {
                        s += GenerateLabel(v);
                        s += GenerateDateTimePicker(v);
                        ret += WrapWithDiv(s);
                    }
                    else if (v.IsEnum)
                    {
                        s += GenerateDropDownListFromEnum(v);
                        ret += WrapWithDiv(s);
                    }
                    else if (v.IsList)
                    {
                        s += GenerateDropDownList(v);
                        ret += WrapWithDiv(s);
                    }
                    else
                    {
                        s += GenerateLabel(v);
                        s += GenerateTextBox(v);
                        ret += WrapWithDiv(s);
                    }
                }
                return ret;
            }

            public string GenerateCheckBox(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateDateTimePicker(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateDropDownList(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateDropDownListFromEnum(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateLabel(PropertiesData data)
            {
                return "<h1>" + data.name + "</h1>";
            }

            public string GenerateRadioButton(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateTextBox(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }
        }


        public class BlazorEFFormControlGenerator : IControlGenerator
        {
            public static string GenerateDefaultControl(PropertiesData data)
            {
                string ret = "";
                string pattern = "";
                if (data.IsEnum)
                {
                    pattern = @"
        <InputSelect @bind-Value=""data.*PName*"">
             @foreach(var v in Enum.GetValues(typeof(*PType*)))
                {
                <option> @v </option>
                }
        </InputSelect>";
                }
                else if (data.IsList)
                {
                    pattern = @"
 <select>

            @foreach(var v in service.lst)
            {
                @*<option value = @v> @v </option> *@
            }
        </select>";
                }
                else if (data.type.Contains("bool"))
                {
                    pattern = @"<input type=""checkbox"" @bind=""@data." + data.name + @""" />";
                }
                else
                {
                    pattern = @"<input @bind=""@data." + data.name + @""" />";
                }
                ret = pattern.Replace("*PType*", data.type)
                   .Replace("*PName*", data.name);
                return ret;
            }

            private string WrapWithBsGridRow(string s) { return "<div class=\"row\">\n" + s + "\n </div>"; }
            private string WrapWithBsGridCol(string s) { return "<div class=\"col-md-1\">\n" + s + "\n </div>"; }
            private string WrapWithBsFluidContainer(string s) { return "<div class=\"container-fluid\">\n" + s + "\n </div>"; }

            public string GenerateAll(CodexData data)
            {
                string ret = "";
                foreach (var v in data.lstProperties)
                {
                    string s = "";
                    if (v.type == "bool")
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
                    else if (v.IsEnum)
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
                return GenerateDefaultControl(data);
            }

            public string GenerateDateTimePicker(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateDropDownList(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateDropDownListFromEnum(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateLabel(PropertiesData data)
            {
                return "<h1>" + data.name + "</h1>";
            }

            public string GenerateRadioButton(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }

            public string GenerateTextBox(PropertiesData data)
            {
                return GenerateDefaultControl(data);
            }
        }

       
    }
}