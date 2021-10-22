using System;
using System.Reflection;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class Extensions
        {


            public static void ToCodex(this string s)
            {
                AutomationControls.Communication.Pipes.NamedPipeClientStreamData data = new AutomationControls.Communication.Pipes.NamedPipeClientStreamData();
                data.Connect("Codex");
                data.Send(s);
            }



            public static void ToCodex(this Type t)
            {
                string ret = "";
                ret += "NameSpace" + "|" + t.Namespace + Environment.NewLine;
                ret += "ClassName" + "|" + t.Name + Environment.NewLine;

                TypeInfo ti = t.GetTypeInfo();
                var cn = ti.Name;

                var props = t.GetProperties();
                foreach (var v in props)
                {
                    try
                    {
                        ret += v.PropertyType + "|" + v.Name + "|" + Environment.NewLine;
                    }
                    catch
                    {
                        continue;
                    }
                }

                ret.ToCodex();
            }

        }
    }
}