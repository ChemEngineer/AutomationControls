using AutomationControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationControls.Programs.Codex.Code
{
     public class Utilities
    {
        public static string GenerateClassCS(CodexData data, string template, string enums ="" , string props = "", string implement = "", string init = "")
        {
            StringBuilder s = new StringBuilder();


            string ret = template
                .Replace("*CL*", data.className)
                .Replace("*CLLOWER*", data.className.ToLower())
                .Replace("*ENUM*", enums)
                .Replace("*INIT*", init)
                .Replace("*IMPLEMENT*", implement)
                .Replace("*NS*", data.csNamespaceName)
                .Replace("*PROPS*", props)
                .Replace("*ISerializable*", AutomationControls.Codex.Code.CS.ISerialization(data));
            s.Append(ret);

            return s.ToString();
        }

        internal static string GenerateMvcCshtml(CodexData data, string mvcUpdateView, IControlGenerator generator)
        {
            throw new NotImplementedException();
        }
    }
}
