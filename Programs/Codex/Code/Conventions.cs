using AutomationControls.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutomationControls.Programs.Codex.Code
{
    public class Conventions
    {
        public static string NameSpace = "*NS*";
        public static string ClassName = "*CL*";
        public static string Properties = "*PROP*";
        public static string Enums = "*ENUM*";
        public static string Using = "*USING*";

        internal static string FillTemplate(string pattern, CodexData data, string serializePath)
        {
            string ret = "";

            ret = pattern.Replace(NameSpace, data.csNamespaceName)
                .Replace(ClassName, data.className)
                .Replace(ClassName, data.className.ToLower());
            ret.ToFile(serializePath);

            return ret;
        }
    }
}
