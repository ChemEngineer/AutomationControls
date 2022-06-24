using AutomationControls.Codex.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationControls.Codex.Code
{
    public class WPF
    {

        #region Data Control 
       

        

        private static string Generate_XAML_TextBoxInGrid(PropertiesData p, string row)
        {
            string s = @"<TextBox Text=""{Binding " + p.name + "" + "}\" Grid.Row = \"" + row + "\" Grid.Column=\"1\" />";
            s += "<TextBlock Text=\"" + p.name + "\"" + " Grid.Row = \"" + row + "\" Grid.Column=\"1\" />";
            return s;
        }

        private static string Generate_XAML_ComboBox(PropertiesData p)
        {
            return "<ComboBox Text=\"" + p.name + "\" ItemsSource =\"{Binding " + p.name + " }\" />" + Environment.NewLine;
        }

        private static string Generate_XAML_TextBoxInDockPanel(PropertiesData p)
        {
            string s = "<DockPanel DockPanel.Dock=\"Top\">" + Environment.NewLine;
            s += "<Label Text=\"" + p.name + "\" />";
            s += "<TextBox Text=\"{Binding " + p.name + "}\" />";
            s += "</DockPanel>" + Environment.NewLine;
            return s;
        }


       

        public static string Generate_XAML_DataGrid_Properties(PropertiesDataList lstPropertyData)
        {
            StringBuilder ret = new StringBuilder();
            int RowCount = 0; int ColumnCount = 0;
            if (lstPropertyData != null)
            {
                if (lstPropertyData.Count > 0)
                {
                    RowCount = lstPropertyData.Count;
                    ColumnCount = lstPropertyData[0].GetType().GetProperties().Length;
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


        public static string Generate_XAML_Checkbox(PropertiesData p)
        {
            return "<CheckBox Content=\"" + p.name + "\" IsChecked=\"{Binding " + p.name + " , UpdateSourceTrigger=PropertyChanged}\" />";
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


        #endregion
    }
}
