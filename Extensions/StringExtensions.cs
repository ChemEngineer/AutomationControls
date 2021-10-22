using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AutomationControls.Extensions
{
    public static partial class Extensions
    {



        public static List<PropertyInfo> GetPropertiesWithCustomAttribute<T>(this Type t) where T : Attribute
        {
            var res = t.GetProperties().Where(x => x.GetCustomAttribute<T>() != null).Select(y => y);
            return res.ToList();
        }



        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (action == null) throw new ArgumentNullException("action");
            foreach (T item in sequence)
                action(item);
        }

       

        public static bool IsNull(this string s) { return String.IsNullOrEmpty(s); }

      
        public static void ToFile(this string s, string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Directory.Exists) fi.Directory.Create();
                using (FileStream fs = File.Create(path))
                {
                    byte[] b = Encoding.ASCII.GetBytes(s);
                    fs.Write(b, 0, b.Count());
                    fs.Flush();
                }
            }
            catch { }
        }

        

       

        public enum ContainsComparison
        {
            AND,
            OR
        }




        public static string GetNumbers(this string InputString)
        {
            //
            //      AUTHOR: Paul Delmonico
            //        DATE: 1/15/12
            //
            // DESCRIPTION: Specify characters to be extracted from a string.
            //              Extracted chars remain in the same relative positions
            //
            //              The first argument is a string of characters to be extracted.
            //              The Second argument is the string to be searched for the characters.
            //
            //      USEAGE: GetThese("01234567890","a1b2c3") will return "123'
            //
            string TempStr = string.Empty;
            string CurrChar = string.Empty;
            string These = string.Empty;
            string ThisStr = string.Empty;

            These = "0123456789.";

            ThisStr = InputString.Trim();


            for (int i = 0; i < ThisStr.Length; i++)
            {
                CurrChar = ThisStr.Substring(i, 1);
                for (int j = 0; j < These.Length; j++)
                {
                    if (CurrChar == These.Substring(j, 1))
                    {
                        TempStr += ThisStr.Substring(i, 1);
                        break;
                    }
                }

            }
            return TempStr;
        }

        public static string Invert(this string s, string delimiter)
        {
            string ret = "";
            string[] st = s.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = st.Count() - 1; i >= 0; i--)
            {
                ret += st[i] + Environment.NewLine;
            }
            return ret;
        }

        public static string Invert(this string s, string[] delimiters)
        {
            string ret = "";
            string[] st = s.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            for (int i = st.Count() - 1; i >= 0; i--)
            {
                ret += st[i] + Environment.NewLine;
            }
            return ret;
        }
    }
}