using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutomationControls;
using AutomationControls.Extensions;
using System.IO;
using AutomationControls.Codex.Data;
using AutomationControls.Programs.Codex.Code;
using AutomationControls.Interfaces;
using AutomationControls.Codex;

namespace AutomationControls.Codex.Code
{
    public class Blazor
    {
        static string serializePath = @"C:\SerializedData\Blazor";

        #region Main Data Class


        public static string GenerateDataClass(CodexData data)
        {
            string ret = "";
            string pattern = @"
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace *NS*.Components.*CL*
    {
    public class *CL*
    {
        *Data*
    }

    public class *CL*List : List<*CL*> { }

    *Enum*
    
    *Extra*

}";


            string dat = "", enums = "", extra = "";
            //Generate Sub Classes
            data.lstProperties.Where(x => x.isObject).ForEach(y => extra += GenerateDataClassMinimal(y));
            dat += Code.CS.GenerateProperties(data);

            //Generate Enums
            data.lstProperties.Where(x => x.IsEnum).ForEach(y => enums += Code.CS.Enums(y));

            ret = pattern.Replace("*NS*", data.csNamespaceName)
                .Replace("*CL*", data.className)
                .Replace("*Data*", dat)
                .Replace("*Enum*", enums)
                .Replace("*Extra*", extra);
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + ".cs"));
            return ret;
        }


        public static string GenerateDataClassMinimal(PropertiesData data2)
        {
            string ret, enums = "";
            string pattern = @"
    public class *CL*
    {
        *Data*

        *Enum*
    }

    public class *CL*List : List<*CL*> { }";

            //Dig out class
            SerializationSurrogate<CodexDataList> lst = (SerializationSurrogate<CodexDataList>)db.sscdata.lstSurrogates.Where(x => x.GetType() == typeof(SerializationSurrogate<CodexDataList>)).First();
            CodexData data = (CodexData)lst.data.Where(x => x.className == data2.type).First();

            //Generate Enums
            data.lstProperties.Where(x => x.IsEnum).ForEach(y => enums += Code.CS.Enums(y));
            //Generate Properties
            string dat = Code.CS.GenerateProperties(data);
           
            ret = pattern.Replace("*CL*", data.className)
                .Replace("*Data*", dat)
                .Replace("*Enum*", enums);
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + ".cs"));
            return ret;
        }

        #endregion

        public static string GenerateService(CodexData data)
        {
            string ret = "";

            string pattern = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace *NS*.Components.*CL*
{
    public class *CL*Service
    {
         string basePath = ""https://entropyhome.asuscomm.com:6004/MySql"";
        string dbName = ""DB"";
        string tableName = ""*CL*"";
        string key = ""1"";

        public *CL*List lst {get; set;} = new *CL*List();       
        public *CL* data {get; set;} = new *CL*();

         public  async Task Serialize()
        {
            var lstjson = JsonConvert.SerializeObject(lst);
            HttpClient http = new HttpClient();
            var res = await http.GetStringAsync(basePath + "" / "" + dbName + "" / "" + tableName + "" / "" + key + "" / "" + lstjson) ;
        }

        public async Task Deserialize()
        {

            HttpClient http = new HttpClient();
            var res = await http.GetStringAsync(basePath + ""/"" + dbName + ""/"" + tableName + ""/"" + key);
            lst = JsonConvert.DeserializeObject<*CL*List>(res);
            if (lst == null)
                lst = new *CL*List();
        }
    }
}";

            ret = pattern.Replace("*NS*", data.csNamespaceName)
                .Replace("*CL*", data.className)
                .Replace("*CLlower*", data.className.ToLower());
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + "Service.cs"));

            return ret;
        }

        public static string GenerateRazorTableComponent(CodexData data)
        {
            string ret = "";
            string pattern = @"
@page ""/*CLlower*List""

@using *NS*.Components.*CL*
@inject *CL*Service service
@inject NavigationManager nav

<button  @onclick=""Add"" >Add</button>
@if(service.lst != null)
{
    <p><em>*CL*</em></p>
    <table>
        <thead>
            <tr>
                *Head*
            </tr>
        </thead>
        <tbody>
            @foreach (var v in service.lst)
            {
                <tr>
                *Data*
                 <th><button  @onclick=""() => Delete(v.ipAddress)"" >Delete</button></th>
                 <th><button  @onclick=""() => Edit(v.ipAddress)""> Edit </button></th>
                 </tr>
            }
        </tbody>
    </table>
}

@code {

     public async Task Add()
    {
        var dat = new *CL*();
        service.lst.Add(dat);
            service.data = dat;
            await service.Serialize();
            nav.NavigateTo(""/*CLlower*edit"");
            await InvokeAsync(StateHasChanged);
        }

        public void Delete(string ipAddress)
        {
            var v = service.lst.Where(x => x.ipAddress == ipAddress).First();
            service.lst.Remove(v);
            service.Serialize();
        }

        public void Edit(string ipAddress)
        {
            service.data = service.lst.Where(x => x.ipAddress == ipAddress).First();
            nav.NavigateTo(""/*CLlower*edit"");
            service.Serialize();
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
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + "ListComponent.razor"));

            return ret;
        }

        public static string GenerateRazorListComponent(CodexData data, PropertiesData pData)
        {
            string ret = "";

            string pattern = @"
@using *NS*.Components.*CL*
@inject *CL*Service service

@if (service.lst == null)
{
    <h1>Loading ...</h1>
}
else
{
    <body>
        <select @onchange=""OnChange"">

            @foreach(var v in *PName*)
            {
                @*<option value = @v > @v </option> *@
            }
        </select>
    </body>
}

        @code {
    private *PType* *PName*;

    protected override async Task OnInitializedAsync()
        {
            return Task.Factory.StartNew(() => { return; });
        }

        private void OnChange(ChangeEventArgs e)
        {
            string s = e.Value.ToString();
        }
    }";

            ret = pattern.Replace("*NS*", data.csNamespaceName)
               .Replace("*CL*", data.className)
               .Replace("*CLlower*", data.className.ToLower())
               .Replace("*PType*", pData.type)
               .Replace("*PName*", pData.name);
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + "ListComponent.razor"));

            return ret;
        }

        #region CRUD

        //internal static string Generate_ListView(CodexData data, IControlGenerator generator)
        //{

        //    string ret = "";
        //    string model = Properties.Resources.MvcMainCshtml;
        //    string props = "";
        //    string props2 = "";

        //    foreach (var v in data.lstProperties)
        //    {
        //        props += "<th>" + v.name + "</th>";
        //        props2 += "<td>@v." + v.name + "</td>";
        //    }

        //    ret = ret.Replace("*className*", data.className)
        //        .Replace("*NamespaceName*", data.csNamespaceName)
        //        .Replace("*props*", props)
        //        .Replace("*ISerializable*", AutomationControls.Codex.Code.CS.ISerialization(data));
        //    ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + "ListComponent2.cshtml"));
        //    return ret;
        //}

        public static string Generate_EditView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string pattern = Properties.Resources.BlazorEditView;
            var ret = pattern.Replace("*NS*", data.csNamespaceName)
               .Replace("*CL*", data.className)
               .Replace("*Data*", ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Components", data.className, data.className + "Edit.razor"));
            return ret;
        }

        public static string Generate_DetailsView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcEditView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Read.cshtml"));
            return ret;
        }

        public static string Generate_CreateView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcCreateView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Create.cshtml"));
            return ret;
        }

        public static string Generate_UpdateView(CodexData data, IControlGenerator generator)
        {
            string ctrls = generator.GenerateAll(data);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcUpdateView, ctrls);
            ret.ToFile(Path.Combine(serializePath, data.className, "Views", data.className, "Update.cshtml"));
            return ret;
        }


        #endregion

        #region Generate Controls

        public static string GenerateDefaultControl(PropertiesData data)
        {
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
                pattern = @"<input type=""checkbox"" @bind=""@service.data." + data.name + @""" />";
            }
            else
            {
                pattern = @"<input @bind=""@service.data." + data.name + @""" />";
            }
            ret = pattern.Replace("*PType*", data.type)
               .Replace("*PName*", data.name);
            return ret;
        }

        public class BlazorControlGenerator : IControlGenerator
        {
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

        #endregion
    }
}
