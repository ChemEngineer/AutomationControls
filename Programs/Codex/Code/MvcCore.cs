using AutomationControls.Extensions;
using AutomationControls.Programs.Codex.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class MvcCore
    {
        static string serializePath = @"C:\SerializedData\MvcCore";

        public static string Generate_Model(CodexData data)
        {
            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);

            props += Environment.NewLine;
            props += AutomationControls.Codex.Code.CS.GenerateProperties(data);
            enums += AutomationControls.Codex.Code.CS.Enums(data);

            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcModel, enums, props, implement, init);
            ret.ToFile(Path.Combine(serializePath, data.className, "Models", data.className + "Model.cs"));
            return ret;
        }

        public static string Generate_Controller(CodexData data)
        {
            string implement = "",
                   init = "",
                   props = "",
                   enums = "";

            data.lstProperties.Where(x => x.isObject).ForEach(x => init += x.name + " = new " + x.type + "();" + Environment.NewLine);
            string ret = Utilities.GenerateClassCS(data, Properties.Resources.MvcController, enums, props, implement, init);
            ret.ToFile(Path.Combine(serializePath, data.className, "Controllers", data.className + "Controller.cs"));
            return ret;
        }


    }
}
