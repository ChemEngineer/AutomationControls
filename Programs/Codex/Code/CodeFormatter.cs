using System;
using System.Linq;

namespace AutomationControls.Codex.Code
{
    public static class CodeFormatter
    {
        //public static String FormatPattern(String st)
        //{
        //    String ret = String.Empty;
        //    int tabCount = 0;

        //    String[] ss = st.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    foreach(String s in ss)
        //    {
        //        foreach(var split in s.Split(new string[] { "{" }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            if(split.Contains("}"))
        //            {
        //                string[] split2 = split.Split(new string[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
        //                for(int i = 0; i < split2.Count(); i++)
        //                {
        //                    if(i / 2 == 0 || i / 2 == 1)
        //                    {
        //                        ret += Environment.NewLine + "{" + Environment.NewLine;
        //                        tabCount++;
        //                        ret += Tab(tabCount) + split2[i] + Environment.NewLine;
        //                    }
        //                    else
        //                    {
        //                        tabCount--;
        //                        ret += "}" + Environment.NewLine;
        //                        ret += Tab(tabCount) + split2[i];
        //                    }
        //                }
        //            }
        //            else { ret += Tab(tabCount) + split; }
        //        }
        //    }
        //    return ret;
        //}

        public static String FormatPattern(String st)
        {
            String ret = String.Empty;
            int tabCount = 0;
            for (int i = 0; i < st.Count(); i++)
            {
                switch (st[i])
                {
                    case '{':
                        ret += Tab(tabCount) + Environment.NewLine + "{" + Environment.NewLine;
                        tabCount++;
                        break;
                    case '}':
                        tabCount--;
                        ret += Tab(tabCount) + Environment.NewLine + "}" + Environment.NewLine;
                        break;
                    case '\r':
                        break;
                    case '\n':
                        break;
                    default:
                        ret += st[i];
                        break;
                }
            }
            return ret;
        }


        private static string Tab(int NumOfTabs)
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
    }
}
