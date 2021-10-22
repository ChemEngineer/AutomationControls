using AutomationControls.Codex.Data;
using AutomationControls.Extensions;
using System;
using System.Diagnostics;
using System.Linq;

namespace AutomationControls.Codex.Code
{
    public class Android
    {
        static string serializePath = @"C:\SerializedData\Generated\Android\";

        private static int tabCount = 0;



        public static String NamespaceKey = "*Namespace*";
        public static String classKey = "*ClassName*";
        public static String propNameKey = "*propName*"; // property names
        public static String propTypeKey = "*propType*"; // property type

        public static String constructor = "*constructor*";
        public static String listConstructor = "*listConstructor*";

        public static string Properties(CodexData data)
        {
            String pattern = AutomationControls.Properties.Resources.Android_Property_Pattern;

            String ret = GenerateEnums(data);

            foreach (PropertiesData p in data.lstProperties)
            {
                var type = p.type.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                type = Converters.JavaCSharpConverter.ToJava(type);
                ret += (pattern.Replace(propNameKey, p.name)).Replace(propTypeKey, type);
                ret += Environment.NewLine;
            }
            return FormatPattern(ret);
        }

        #region Kotlin

        public static string KotlinFragment(CodexData data)
        {
            string path = GeneratePath(data);

            string activity = AutomationControls.Properties.Resources.KotlinFragment;
            activity = activity.Replace("*PROP*", AutomationControls.Codex.Code.Android.KotlinControlDeclarations(data));
            activity = activity.Replace("*CL*", data.className);
            activity = activity.Replace("*classNameLower*", data.className.ToLower());
            activity = activity.Replace("*namespaceName*", data.androidNamespace);
            activity = activity.Replace("*layoutXmlName*", data.className.ToLower() + "_fragment");
            activity = activity.Replace("*propInit*", AutomationControls.Codex.Code.Android.KotlinControlsInitialization(data));
            activity = activity.Replace("*gson*", data.className + " data");
            activity.ToFile(path + "\\" + data.className + "Fragment.kt");
            return activity;
        }

        public static string KotlinControlDeclarations(CodexData data)
        {
            string s = "";
            foreach (var v in data.lstProperties)
            {
                if (v.type == "bool")
                    s += KotlinControlDeclaration("CheckBox", "cb" + v.name);
                else
                    s += KotlinControlDeclaration("EditText", "et" + v.name);

                s += Environment.NewLine;
            }
            return s;
        }

        public static string KotlinControlDeclaration(string controlType, string controlName)
        {
            return "lateinit var " + controlName + ": " + controlType;
        }

        public static string KotlinControlsInitialization(CodexData data)
        {
            string s = "";
            foreach (var v in data.lstProperties)
            {
                if (v.type == "bool")
                    s += KotlinControlInitialization("CheckBox", "cb" + v.name);
                else
                    s += KotlinControlInitialization("EditText", "et" + v.name);
            }
            return s;
        }

        public static string KotlinControlInitialization(string controlType, string controlName)
        {
            return controlName + " = v.findViewById<" + controlType + ">(R.id." + controlName + ")" + Environment.NewLine;
        }

        public static string GenerateFragmentXml(CodexData data)
        {
            string path = GeneratePath(data);
            path = System.IO.Path.Combine(path, "res", "layout", data.className.ToLower() + "_fragment.xml");
            string s = AutomationControls.Codex.Code.Android.BasicLayoutXML(data).Replace("*PROP*", AutomationControls.Codex.Code.Android.XmlControls(data));

            s.ToFile(path);
            return s;
        }

        public static string BasicLayoutXML(CodexData data)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
                <LinearLayout xmlns:android=""http://schemas.android.com/apk/res/android""
    android:orientation=""vertical""
    android:layout_width=""match_parent""
    android:layout_height=""match_parent"">
 *PROP*
</LinearLayout>";
        }

        #endregion
        #region GSON Class

        public static string GSONDataClass(CodexData data)
        {
            string path = GeneratePath(data);
            //AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(data);

            string name = data.className;
            var lst = data.lstProperties;

            string ret = AutomationControls.Properties.Resources.Android_GSON_Pattern;
            ret = ret.Replace(classKey, name).Replace("*0-0*", Properties(data));

            String tmp = "";
            tmp += GSONSerializer(data) + Environment.NewLine;
            tmp += GSONDeserializer(data) + Environment.NewLine;
            tmp += GSONSerialize(data) + Environment.NewLine;
            tmp += GSONDeserialize(data) + Environment.NewLine;
            tmp += ReadFromFile() + Environment.NewLine;
            tmp += WriteToFile() + Environment.NewLine;

            string s = ret.Replace("*1-1*", tmp).Replace("*packageName*", data.androidNamespace) + Environment.NewLine;

            s.ToFile(path + "\\java\\" + data.className + ".java");

            Process.Start(path);

            return s;
        }

        private static string GenerateEnums(CodexData data)
        {
            var s = "";

            var lst = data.lstProperties.Where(x => x.IsEnum);
            foreach (var v in lst)
            {
                s += "enum " + v.type + @"
                     {
                        ";
                v.lstEnum.ForEach(x => s += "@SerializedName(\"" + x.position + "\")" + Environment.NewLine + x.value + "," + Environment.NewLine);
                s += "}" + Environment.NewLine + Environment.NewLine;
            }
            return s;
        }

        private static string GeneratePath(CodexData data)
        {
            string path = serializePath;
            data.csNamespaceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ForEach(x => path = System.IO.Path.Combine(path, x));
            return System.IO.Path.Combine(path, data.className);
        }

        public static string GSONSerializer(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Serializer;
            string pattern = "jsonObj.add(\"*propName*\", context.serialize(value.get*propName*()));";
            ret = ret.Replace(classKey, data.className);

            String tmp = "";
            foreach (PropertiesData p in data.lstProperties)
            {
                tmp += (pattern.Replace(propNameKey, p.name)).Replace(propTypeKey, p.type);
                tmp += Environment.NewLine;
            }
            return FormatPattern(ret.Replace("*1-1*", tmp)) + Environment.NewLine;
        }

        public static string GSONDeserializer(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Deserializer;
            // string pattern = "ret.*propName* = jobj.get(\"*propName*\").getAsString();";
            string pattern = @"Type " + propNameKey + @"Type = new TypeToken<" + propTypeKey + @">() { }.getType();
                            ret." + propNameKey + " = new Gson().fromJson(jobj.get(\"" + propNameKey + "\").getAsJsonObject(), " + propNameKey + "Type);";
            ret = ret.Replace(classKey, data.className);

            String tmp = "";
            foreach (PropertiesData p in data.lstProperties)
            {
                tmp += (pattern.Replace(propNameKey, p.name)).Replace(propTypeKey, AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(data.className));
                tmp += Environment.NewLine;
            }
            return FormatPattern(ret.Replace("*1-1*", tmp)) + Environment.NewLine;
        }

        public static string GSONTypeAdapter(string name, PropertiesDataList lst)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_TypeAdapter;
            ret = ret.Replace(classKey, name);

            String pattern1 = "if( s == \"*propName*\") {  data.set*propName*(in.nextString()); }";
            String pattern2 = "if( s == \"*propName*\") {  data.*propName*(in.nextString()); }";
            string tmp1 = "", tmp2 = "";

            foreach (PropertiesData p in lst)
            {
                if (p == lst.First())
                {
                    tmp1 += (pattern1.Replace(propNameKey, p.name)).Replace(propTypeKey, AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(p.type));
                    tmp1 += Environment.NewLine;

                    tmp2 += (pattern2.Replace(propNameKey, p.name)).Replace(propTypeKey, AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(p.type));
                    tmp2 += Environment.NewLine;
                }
                else
                {
                    tmp1 += "else " + (pattern1.Replace(propNameKey, p.name)).Replace(propTypeKey, AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(p.type));
                    tmp1 += Environment.NewLine;

                    tmp2 += "else " + (pattern2.Replace(propNameKey, p.name)).Replace(propTypeKey, AutomationControls.Codex.Converters.JavaCSharpConverter.ToJava(p.type));
                    tmp2 += Environment.NewLine;
                }
            }
            ret = ret.Replace("*1-1*", tmp1);
            ret = ret.Replace("*3-3*", tmp2);

            return FormatPattern(ret) + Environment.NewLine;
        }

        public static string GSONSerialize(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Serialize;
            return FormatPattern(ret.Replace(classKey, data.className)) + Environment.NewLine;
        }

        public static string GSONDeserialize(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Deserialize;
            return FormatPattern(ret.Replace(classKey, data.className)) + Environment.NewLine;
        }

        #endregion

        #region Utilities

        private static String FormatPattern(String st)
        {
            String ret = String.Empty;
            tabCount = 0;

            String[] ss = st.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (String s in ss)
            {
                ret += Tab(tabCount) + s.Trim() + Environment.NewLine;

                int leftBraceCount = (from a in s
                                      where a == '{'
                                      select a).Count();

                int rightBraceCount = (from a in s
                                       where a == '}'
                                       select a).Count();

                tabCount += leftBraceCount - rightBraceCount;
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

        private static string ReadFromFile() { return FormatPattern(AutomationControls.Properties.Resources.Android_Read_from_File); }

        private static string WriteToFile() { return FormatPattern(AutomationControls.Properties.Resources.Android_Write_to_File); }

        #endregion

        #region ObservableGson

        internal static string ObservableGSONDataClass(CodexData data)
        {
            string ret = ObservableObject(data);

            String tmp = "";
            tmp += ObservableGSONSerializer(data) + Environment.NewLine;
            tmp += ObservableGSONDeserializer(data) + Environment.NewLine;
            tmp += GSONSerialize(data) + Environment.NewLine;
            tmp += GSONDeserialize(data) + Environment.NewLine;
            tmp += ReadFromFile() + Environment.NewLine;
            tmp += WriteToFile() + Environment.NewLine;

            return ret.Replace("//*Add*", tmp) + Environment.NewLine;

        }

        public static string ObservableGSONSerializer(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Serializer;
            string pattern = ""; // 
            ret = ret.Replace(classKey, data.className);

            String tmp = "";
            foreach (PropertiesData p in data.lstProperties)
            {
                if (p.type.Contains("string", StringComparison.OrdinalIgnoreCase) || p.IsEnum) pattern = "jsonObj.add(\"*propName*\", context.serialize(value.*propName*));";
                else pattern = "jsonObj.add(\"*propName*\", context.serialize(value.*propName*));";
                tmp += (pattern.Replace(propNameKey, p.name)).Replace(propTypeKey, p.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last());
                tmp += Environment.NewLine;
            }
            return FormatPattern(ret.Replace("*1-1*", tmp)) + Environment.NewLine;
        }

        public static string ObservableGSONDeserializer(CodexData data)
        {
            string ret = AutomationControls.Properties.Resources.Android_GSON_Deserializer;
            string pattern = "";
            ret = ret.Replace(classKey, data.className);

            String tmp = "";
            foreach (PropertiesData p in data.lstProperties)
            {
                if (p.type.Contains("string", StringComparison.OrdinalIgnoreCase)) pattern = "ret.*propName* = jobj.get(\"*propName*\").getAsString();";
                else if (p.IsEnum) pattern = "ret.*propName* = *propType*.valueOf(jobj.get(\"*propName*\").getAsString());";
                else pattern = @"Type *propName*Type = new TypeToken<EbayConditionData>(){}.getType();
            ret.*propName* =new Gson().fromJson(jobj.get(""*propName*"").getAsJsonObject(),*propName*Type);";
                tmp += (pattern.Replace(propNameKey, p.name)).Replace(propTypeKey, p.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last());
                tmp += Environment.NewLine;
            }
            return FormatPattern(ret.Replace("*1-1*", tmp)) + Environment.NewLine;
        }

        public static string PropertiesBindingXML(PropertiesDataList lst, CodexData data)
        {
            string pattern = @"<variable name=""dataBinding"" type=""*Namespace*.*ClassName*"" />";
            string pattern2 = @" <TextView
      android:layout_width=""wrap_content""
      android:layout_height=""wrap_content""
      android:text=""@{dataBinding.*propName*}""
      android:layout_centerVertical=""true""
      android:layout_centerHorizontal=""true"" />";


            String ret = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <layout xmlns:android=""http://schemas.android.com/apk/res/android"">
                             <data>
                                ";
            ret += pattern.Replace(NamespaceKey, data.csNamespaceName).Replace(classKey, data.className);
            ret += "</data>";
            ret += @"<LinearLayout 
                    xmlns:tools=""http://schemas.android.com/tools""
                    android:layout_width=""match_parent""
                    android:layout_height=""match_parent""
                    tools:context="".MainActivity"" >";
            foreach (PropertiesData p in lst)
            {
                ret += pattern2.Replace(propNameKey, p.name);
                ret += Environment.NewLine;
            }
            ret += @"</LinearLayout>
                    </layout>";
            return ret;
        }

        public static string PropertiesBindingTable(CodexData data)
        {
            string s = "";
            string java = Android.Properties(data);
            for (int i = 0; i < data.lstProperties.Count(); i++) { s += AutomationControls.Properties.Resources.AndroidTableRowBinding.Replace("*name*", data.lstProperties[i].name) + Environment.NewLine; };

            string ret = AutomationControls.Properties.Resources.AndroidTableLayout.Replace("*rows*", s);
            string path = @"C:\SerializedData\" + data.className;
            ret.ToFile(path + "\\" + data.className.ToLower() + "bindingtable.xml");
            Process.Start(path);
            return ret;
        }

        public static string GSONDataClassLayoutXML(CodexData data)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<android.support.v4.widget.DrawerLayout xmlns:android=""http://schemas.android.com/apk/res/android""
    xmlns:app=""http://schemas.android.com/apk/res-auto""
    xmlns:tools=""http://schemas.android.com/tools""
    android:id=""@+id/drawer_layout" + data.className + @"""
    android:layout_width=""match_parent""
    android:layout_height=""match_parent""
    android:fitsSystemWindows=""true""
    tools:openDrawer=""start"">

    <RelativeLayout xmlns:android=""http://schemas.android.com/apk/res/android""
        xmlns:tools=""http://schemas.android.com/tools""
        android:layout_width=""match_parent""
        android:layout_height=""match_parent"">
        <TextView android:id=""@+id/messageTextView" + data.className + @""" 
            android:layout_width=""wrap_content""
            android:layout_height=""wrap_content""
            android:text=""Simple DrawerLayout example."" />
    </RelativeLayout>
   <android.support.v7.widget.LinearLayoutCompat xmlns:android=""http://schemas.android.com/apk/res/android""
    android:orientation=""vertical""
    android:layout_width=""match_parent""
    android:layout_height=""match_parent"">
                                   *PROP*
                    </android.support.v7.widget.LinearLayoutCompat>
</android.support.v4.widget.DrawerLayout>
                    ";
        }

        public static string ObservableObject(CodexData data)
        {
            string ret = @"public class *ClassName* {";
            ret = ret.Replace(classKey, data.className);
            //string s = @"private *propType* *propName*; @Bindable public *propType* get*propName*() { return this.*propName*;  } public void set*propName*(*propType* *propName*) { this.*propName* = *propName*;  notifyPropertyChanged(BR.*propName*); }";

            foreach (var v in data.lstProperties)
            {
                string s = "";
                s += Environment.NewLine;
                //if(v.type.Contains("string", StringComparison.OrdinalIgnoreCase)) 
                s += @"private *propType* *propName*;  public *propType* get*propName*() { return this.*propName*;  } public void set*propName*(*propType* *propName*) { this.*propName* = *propName*;  notifyPropertyChanged(BR.*propName*); }";
                ret += s.Replace(propNameKey, v.name).Replace(propTypeKey, v.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last());
            }
            ret += @"//*Add*
            }";


            return ret;
        }

        public static string ObservableObject2(PropertiesDataList lst, string name)
        {
            string ret = @"public class *ClassName* extends BaseObservable {";
            ret = ret.Replace(classKey, name);
            //string s = @"private *propType* *propName*; @Bindable public *propType* get*propName*() { return this.*propName*;  } public void set*propName*(*propType* *propName*) { this.*propName* = *propName*;  notifyPropertyChanged(BR.*propName*); }";

            foreach (var v in lst)
            {
                string s = "";
                s += Environment.NewLine;
                //if(v.type.Contains("string", StringComparison.OrdinalIgnoreCase)) 
                s += @"private *propType* *propName*; @Bindable public *propType* get*propName*() { return this.*propName*;  } public void set*propName*(*propType* *propName*) { this.*propName* = *propName*;  notifyPropertyChanged(BR.*propName*); }";
                ret += s.Replace(propNameKey, v.name).Replace(propTypeKey, v.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last());
            }
            ret += @"//*Add*
            }";

            ret += @"//Put in onCreate of UI class    ActivityBindingTestBinding binding = DataBindingUtil.setContentView(this, R.layout.activity_binding_test);";

            return ret;
        }

        public static string ObservableFields(PropertiesDataList lst, string name)
        {
            string ret = @"public class *ClassName* {";
            ret = ret.Replace(classKey, name);

            foreach (var v in lst)
            {
                string s = "";
                s += Environment.NewLine;
                if (v.type.Contains("string", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableField<String> *propName* =  new ObservableField<>();";
                else if (v.type.Contains("int", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableInt *propName* =  new ObservableInt();";
                else if (v.type.Contains("bool", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableBoolean *propName* =  new ObservableBoolean();";
                else if (v.type.Contains("byte", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableByte *propName* =  new ObservableByte();";
                else if (v.type.Contains("char", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableChar *propName* =  new ObservableChar();";
                else if (v.type.Contains("double", StringComparison.OrdinalIgnoreCase)) s += @"public final ObservableDouble *propName* =  new ObservableDouble();";
                else s += @"public final ObservableField<*propType*> *propName* =  new  ObservableField<>();";
                ret += s.Replace(propNameKey, v.name).Replace(propTypeKey, v.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last());
            }
            ret += @"//*Add*
            }";

            ret += @"//Put in onCreate of UI class    ActivityBindingTestBinding binding = DataBindingUtil.setContentView(this, R.layout.activity_binding_test);";

            return ret;
        }


        #endregion

        public static string GenerateActivity(CodexData data)
        {
            string path = GeneratePath(data);

            string activity = AutomationControls.Codex.Code.Android.Activity(data);
            activity = activity.Replace("*PROP*", AutomationControls.Codex.Code.Android.JavaControlDeclarations(data));
            activity = activity.Replace("*propInit*", AutomationControls.Codex.Code.Android.JavaControlsInitialization(data));
            activity = activity.Replace("*gson*", data.className + " data;");
            activity.ToFile(path + "\\" + data.className + "Activity.java");
            return activity;
            // 
        }
        public static string XmlControls(CodexData data)
        {
            string etpattern = @"<EditText android:id=""@+id/et*prop*""
                                         android:layout_height=""wrap_content""
                                         android:layout_width=""match_parent"">
                              </EditText>";
            string cbpattern = @"<CheckBox android:id=""@+id/cb*prop*""
                                         android:layout_height=""wrap_content""
                                         android:layout_width=""match_parent"">
                              </EditText>";

            string ret = "";
            foreach (var v in data.lstProperties)
            {
                if (v.type == "bool")
                    ret += cbpattern.Replace("*prop*", v.name.Trim());
                else
                    ret += etpattern.Replace("*prop*", v.name.Trim());

                if (v != data.lstProperties.Last()) ret += Environment.NewLine;
            }
            return ret;
        }

        internal static string Activity(CodexData codexData)
        {
            return @"package com.example.delmonic.ebayposter;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.AttributeSet;
import android.util.Log;
import android.view.ContextMenu;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.*;

public class " + codexData.className + @"Activity extends AppCompatActivity {
    
    *PROP*

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_" + codexData.className + @");

        *propInit*   

        }


    *gson*
}";
        }

        public static string JavaControlInitialization(string controlType, string controlName)
        {
            string s = controlName + " = (" + controlType + ") this.findViewById(R.id." + controlName + ");" + Environment.NewLine;

            return s;
        }
        public static string JavaControlsInitialization(CodexData data)
        {
            string s = "";
            foreach (var v in data.lstProperties)
            {
                s += JavaControlInitialization("EditText", "et" + v.name);
                s += "et" + v.name + ".setText(data." + v.name + ");" + Environment.NewLine;
            }

            return s;
        }

        public static string JavaControlDeclarations(CodexData data)
        {
            string s = "";
            data.lstProperties.ForEach(x => s += JavaControlDeclaration("EditText", "et" + x.name) + Environment.NewLine);
            return s;
        }

        public static string JavaControlDeclaration(string controlType, string controlName)
        {
            return controlType + " " + controlName + ";";
        }

        internal static string DataEditorLayoutXml(string p, PropertiesDataList propertiesDataList)
        {
            string ret = @"<LinearLayout xmlns:android=\""http://schemas.android.com/apk/res/android\""
    android:layout_width=\""match_parent\""
    android:layout_height=\""match_parent\""
    android:orientation=\""vertical\"" >";

            string unit = @"<LinearLayout
        android:layout_width=\""match_parent\""
        android:layout_height=\""wrap_content\""
        android:orientation=\""horizontal\"" >
        <TextView
            android:layout_width=\""match_parent\""
            android:layout_height=\""wrap_content\""
            android:layout_gravity=\""left\""
            android:layout_weight=\""50\""
            android:text=\""*name*:\""
            android:textSize=\""24sp\"" />
        <EditText
            android:id=\""@+id/tv_*name*\""
            android:layout_width=\""match_parent\""
            android:layout_height=\""wrap_content\""
            android:layout_gravity=\""right\""
            android:layout_weight=\""50\""
            android:background=\""#AAFF2A\""
            android:textSize=\""24sp\"" />
    </LinearLayout>";

            foreach (var v in propertiesDataList)
            {
                ret += unit.Replace("*name*", v.name);
            }

            ret += @"<Button
        android:id=\""@+id/btn_Commit_" + p + @"\""
        android:layout_width=\""match_parent\""
        android:layout_height=\""match_parent\""
        android:onClick=\""btnCommitClick\""
        android:text=\""Commit\"" />    
    </LinearLayout>";

            return ret.Replace(@"\", "");
        }

        internal static string DataEditorFragment(string p, PropertiesDataList propertiesDataList)
        {
            string ret = AutomationControls.Properties.Resources.DataEditorFragment;
            string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";

            foreach (var v in propertiesDataList)
            {
                s1 += "TextView tv" + v.name + " = null;";
                s2 += "if(tv" + v.name + " != null) tv" + v.name + ".setText(data." + v.name + ");";
                s3 += "tv" + v.name + " = (EditText)v.findViewById(R.id.tv" + v.name + ");";
                s4 += "if(tv" + v.name + " != null) data." + v.name + " = tv" + v.name + ".getText().toString();";
                s5 += " tv" + v.name + ".setOnFocusChangeListener(onFocusChangeListener);";
            }

            var res = ret.Replace("*s1*", s1).Replace("*s2*", s2).Replace("*s3*", s3).Replace("*s4*", s4).Replace("*s5*", s5).Replace("*class*", p).Replace("*datatype*", p);
            return res;
        }

        public static String Event(String name)
        {
            String s = AutomationControls.Properties.Resources.Android_Event_Pattern;
            string firstLetter = name[0].ToString();
            string upper = firstLetter.ToUpper() + name.Remove(0, 1);
            string lower = firstLetter.ToLower() + name.Remove(0, 1);
            s = s.Replace("BtDataReceive", upper);
            s = s.Replace("btDataReceive", lower);
            return FormatPattern(s) + Environment.NewLine;
        }

        public static string GenerateActivityXml(CodexData data)
        {
            string path = GeneratePath(data);
            path = System.IO.Path.Combine(path, "res", "layout", data.className.ToLower() + "_activity" + ".xml");
            string s = AutomationControls.Codex.Code.Android.GSONDataClassLayoutXML(data).Replace("*PROP*", AutomationControls.Codex.Code.Android.XmlControls(data));

            s.ToFile(path);
            return s;
        }

        internal static string GSONDataListClass(CodexData data)
        {
            string path = GeneratePath(data);
            path = System.IO.Path.Combine(path, "java", data.className + "List.java");

            string s = @"package " + data.androidNamespace + @"

import java.util.ArrayList;

            public class " + data.className + @"List  extends ArrayList<" + data.className + @">
            {
                
            }";
            s.ToFile(path);

            return s;
            {

            }

        }
    }
}