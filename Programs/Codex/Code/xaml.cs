using AutomationControls.Codex.Data;
using AutomationControls.Extensions;
using System;
using System.Linq;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class xaml
    {
        public static string Generate_XAML_Custom_Properties(CodexData data)
        {
            string s = ""; int row = 0;

            foreach (PropertiesData x in data.lstProperties)
            {
                if (x.type.Contains("ObservableColl", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("[]", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("List", StringComparison.OrdinalIgnoreCase)
                    || x.type.Contains("Observ", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("bool", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_Checkbox(x);
                else
                {
                    s += Generate_XAML_TextBoxInGrid(x, row.ToString());
                    row++;
                    //  s += Generate_XAML_TextBoxInDockPanel(x);
                }
                // s += Generate_XAML_EnumComboBox(x);
                s += Environment.NewLine;
            };
            return s;
        }

        public static string Generate_XAML_DatgGrid_Properties(CodexData data)
        {
            StringBuilder ret = new StringBuilder();
            int RowCount = 0; int ColumnCount = 0;
            if (data.lstProperties != null && data.lstProperties.Count() > 0)
            {
                RowCount = data.lstProperties.Count;
                ColumnCount = data.lstProperties[0].GetType().GetProperties().Count();
                for (int j = 0; j <= RowCount - 1; j++)
                {
                    PropertiesData p = data.lstProperties[j];
                    if (p.IsEnum)
                    {
                        ret.AppendLine(Tab(1) + Tab(1) + Tab(1) + "<DataGridComboBoxColumn  Header=" + p.name + " ItemsSource=\"{conv:EnumBindingSource {x:Type local:p.name}}\" SelectedItemBinding=\"{Binding p.name}\" />");
                    }
                    else
                    {
                        ret.AppendLine(Tab(1) + Tab(1) + Tab(1) + "<DataGridTextColumn Header=\"" + p.name + "\" Binding=\"{Binding " + p.name + " , UpdateSourceTrigger=PropertyChanged}\" />");
                    }
                }

            }
            return ret.ToString();
        }

        public static string Generate_XAML_TextBoxInGrid(PropertiesData p, string row)
        {
            string s = @"<TextBox Text=""{Binding " + p.name + "" + "}\" Grid.Row = \"" + row + "\" Grid.Column=\"1\" />";
            s += "<TextBlock Text=\"" + p.name + "\"" + " Grid.Row = \"" + row + "\" Grid.Column=\"1\" />";
            return s;
        }


        public static string Generate_XAML_ComboBox(PropertiesData p)
        {
            string s = "<DockPanel >" + Environment.NewLine;
            s += "<ComboBox Width=\"100\" ItemsSource=\"{Binding " + p.name + " , UpdateSourceTrigger=PropertyChanged}\" />" + Environment.NewLine;
            s += "<TextBlock Text=\"" + p.name + "\" />" + Environment.NewLine; ;
            s += "</DockPanel>" + Environment.NewLine;
            return s;
        }

        public static string Generate_XAML_Custom_Properties(PropertiesDataList lstPropertyData)
        {
            string s = ""; //int row = 0;

            lstPropertyData.ForEach(x =>
            {
                if (x.type.Contains("ObservableColl", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("[]", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("List", StringComparison.OrdinalIgnoreCase)
                    || x.type.Contains("Observ", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_ComboBox(x);
                else if (x.type.Contains("bool", StringComparison.OrdinalIgnoreCase))
                    s += Generate_XAML_Checkbox(x);
                else
                {
                    //  s +=  Generate_XAML_TextBoxInGrid(x, row.ToString());
                    //  row++;
                    s += Generate_XAML_TextBoxInDockPanel(x);
                }
                // s += Generate_XAML_EnumComboBox(x);
                s += Environment.NewLine;
            });
            return s;
        }

        private static string Generate_XAML_TextBoxInDockPanel(PropertiesData p)
        {
            string s = "<DockPanel DockPanel.Dock=\"Top\">" + Environment.NewLine;
            s += "<TextBlock Text=\"" + p.name + "\" />";
            s += "<TextBox Text=\"{Binding " + p.name + "}\" />";
            s += "</DockPanel>" + Environment.NewLine;
            return s;
        }


        public static string Generate_XAML_Checkbox(PropertiesData p)
        {
            return "<CheckBox Content=\"" + p.name + "\" IsChecked=\"{Binding " + p.name + " , UpdateSourceTrigger=PropertyChanged}\" />";
        }

        public static string Generate_XAML_DatgGrid_Properties(PropertiesDataList lstPropertyData)
        {
            StringBuilder ret = new StringBuilder();
            int RowCount = 0; int ColumnCount = 0;
            if (lstPropertyData != null)
            {
                if (lstPropertyData.Count > 0)
                {
                    RowCount = lstPropertyData.Count;
                    ColumnCount = lstPropertyData[0].GetType().GetProperties().Count();
                    for (int j = 0; j <= RowCount - 1; j++)
                    {
                        PropertiesData p = lstPropertyData[j];
                        if (p.IsEnum)
                        {
                            ret.AppendLine(Tab(1) + Tab(1) + Tab(1) + "<DataGridComboBoxColumn  Header=\"" + p.name + "\" ItemsSource=\"{convert:EnumBindingSource {x:Type local:" + p.type + "}}\" SelectedItemBinding=\"{Binding " + p.name + "}\" />");
                        }
                        else
                        {
                            ret.AppendLine(Tab(1) + Tab(1) + Tab(1) + "<DataGridTextColumn Header=\"" + p.name + "\" Binding=\"{Binding " + p.name + " , UpdateSourceTrigger=PropertyChanged}\" />");
                        }
                    }
                }
            }
            return ret.ToString();
        }


        public static string Tab(int NumOfTabs)
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
