using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationControls.Codex.Converters
{
    public class JavaCSharpConverter
    {
        private static ConverterDataList lst = new ConverterDataList() {
           new ConverterData() { csType = "ObservableCollection" , javaType = "ArrayList" } ,
           new ConverterData() { csType = "bool" , javaType = "Boolean" } ,
           new ConverterData() { csType = "bool?" , javaType = "Boolean" } ,
           new ConverterData() { csType = "Boolean?" , javaType = "Boolean" } ,
           new ConverterData() { csType = "sbyte" , javaType = "Byte" } ,
           new ConverterData() { csType = "sbyte?" , javaType = "Byte" } ,
           new ConverterData() { csType = "short" , javaType = "Short" } ,
           new ConverterData() { csType = "DateTime" , javaType = "String" } ,
           new ConverterData() { csType = "int" , javaType = "Integer" } ,
           new ConverterData() { csType = "Int32" , javaType = "Integer" } ,
           new ConverterData() { csType = "long" , javaType = "Long" } ,
           new ConverterData() { csType = "float" , javaType = "Float" } ,
           new ConverterData() { csType = "double" , javaType = "Double" } ,
           new ConverterData() { csType = "string" , javaType = "String" } ,
           new ConverterData() { csType = "List" , javaType = "ArrayList" }
       };


        public static string ToJava(string type)
        {
            string s = type.Split(new[] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries).First();
            var res = lst.Where(x => s.ToLower() == x.csType.ToLower());
            if (res.Count() > 0)
            {
                var v = res.First();
                s = type.Replace(v.csType, v.javaType);
            }
            return s;
        }

        //public static void ToJava(CodexData data)
        //{
        //    foreach(var v in data.lstProperties)
        //    {
        //        var type = JavaCSharpConverter.ToJava(v.type);
        //    }
        //}


        class ConverterData
        {
            public string javaType;
            public string csType;
        }

        class ConverterDataList : List<ConverterData>
        {

        }

    }
}
