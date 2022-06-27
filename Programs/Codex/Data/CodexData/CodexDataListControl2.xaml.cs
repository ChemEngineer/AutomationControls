using AutomationControls.Codex.Code;
using AutomationControls.Codex.Data;
using AutomationControls.Communication.Pipes;
using AutomationControls.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static AutomationControls.Codex.Code.Blazor;
using static AutomationControls.Codex.Code.MvcCoreEF;

namespace AutomationControls
{
    /// <summary>
    /// Interaction logic for CodexDataListControl2.xaml
    /// </summary>
    public partial class CodexDataListControl2 : UserControl
    {

        public CodexDataListControl2()
        { 
            PipeServer.StartListeningAsync("Codex", s =>
            {
                CodexDataList lst = (DataContext as CodexDataList);

                CodexData cd = new CodexData();
                //PropertiesDataList pd = new PropertiesDataList();

                string[] ss = s.Split(new string[] { "|", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                cd.csNamespaceName = ss[1];
                cd.className = ss[3];
                cd.profileName = cd.className;
                //  db.host.AddProfile(cd.profileName);
                for (int i = 4; i < ss.Count(); i += 2)
                    cd.lstProperties.Add(new PropertiesData() { type = ss[i], name = ss[i + 1] });

                lst.Add(cd);

            });

            InitializeComponent();

            Loaded += (sender, e) =>
                {
                    CodexDataList lst = (DataContext as CodexDataList);
                    lb.ItemsSource = lst;
                    if (lst.Count() > 0)
                        lb.SelectedItem = lst.First();

                    //  lst.Add(db.FromType(typeof(AutomationControls.BaseClasses.DeviceBase)));  /// add on the fly

                };

            lb.SelectionChanged += (sender2, e2) =>
            {
                dpmain.Children.Clear();
                CodexData data = lb.SelectedItem as CodexData;
                if (data == null) return;
                dpmain.Children.Add(data.GetUserControls().First());
                if (data.lstProperties.Select(x => x.position).Sum() == 0)  //Add positions 
                {
                    int i = 0; data.lstProperties.ForEach(x => x.position = (i += 1));
                }
                data.lstProperties = new PropertiesDataList(data.lstProperties.OrderBy(x => x.position));

                dgProperties.ItemsSource = data.lstProperties;
                dgProperties.SelectionChanged += (sender3, e3) =>
                {
                    dpProp.Children.Clear();
                    var v = (dgProperties.SelectedItem) as PropertiesData;
                    if (v == null || v.type == null)
                        return;
                    if (v.IsEnum)
                        dpProp.Children.Add(new DataGrid() { ItemsSource = v.lstEnum });
                    else
                    {
                        string type = v.type.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                        if (v.type.EndsWith("List"))
                            type = v.type.Replace("List", "");
                        CodexDataList lst = lb.ItemsSource as CodexDataList;
                        var res = lst.Where(x => x.className == type);
                        if (res.Count() > 0)
                            dpProp.Children.Add(AutomationControls.WPF.DataBinding.Generate_UserControl(res.First()));
                    }
                };
            };
        }

        private void miSurrogate_Click(object sender, RoutedEventArgs e)
        {
            CodexDataList lst = (DataContext as CodexDataList);
            foreach (CodexData data in lb.SelectedItems)
            {
                string s = AutomationControls.Codex.Code.CS.GenerateSurrogate(data);

                List<string> lst3 = new List<string>();
                // Generate types within selected data class
                data.lstProperties.ForEach(x =>
                {
                    if (x.type != null)
                    {
                        var type = x.type.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                        foreach (var v in lst)
                        {
                            var classname = v.className.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                            if (type.Contains(classname))
                            {
                                lst3.Add(v.className);
                                s += "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
                                s += AutomationControls.Codex.Code.CS.GenerateSurrogate(v);
                            }
                        }
                    }
                });
            }
        }

        private void miWPFControl_Click(object sender, RoutedEventArgs e)
        {
            CodexDataList lst = (DataContext as CodexDataList);
            foreach (CodexData data in lb.SelectedItems)
            {
                List<string> lst3 = new List<string>();
                tb.Text += AutomationControls.Codex.Code.CS.GenerateWPFDataClass(data);
            }
        }
        
        private void Gson_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
                if (data == null) continue;

                string dataClass = AutomationControls.Codex.Code.Android.GSONDataClass(data);
                string s = dataClass;
                s += Environment.NewLine + Environment.NewLine + "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine + Environment.NewLine;

                string activity = AutomationControls.Codex.Code.Android.Activity(data);
                s += activity + Environment.NewLine + Environment.NewLine + "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine + Environment.NewLine;

                s += AutomationControls.Codex.Code.Android.GenerateActivityXml(data);
                s += Environment.NewLine + Environment.NewLine + "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine + Environment.NewLine;

                s += AutomationControls.Codex.Code.Android.GSONDataListClass(data);
                s += Environment.NewLine + Environment.NewLine + "\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine + Environment.NewLine;

                tb.Text = s;


            }
        }

        private void miCSEvent_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.CodeFormatter.FormatPattern(AutomationControls.Codex.Code.CS.Event(data.className));
        }

        private void KotlinFragment_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.Android.KotlinFragment(data);
            tb.Text += "\n\n ////////////////////////////////////  \n\n";
            tb.Text += Codex.Code.Android.GenerateFragmentXml(data);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Convert Code

        private void miConvert_Click(object sender, RoutedEventArgs e)
        {
            CodexDataList data = DataContext as CodexDataList;
            if (data == null) return;

            Button btn = new Button() { Content = "Submit" };
            TextBox tbClassName = new TextBox() { Text = "ClassName" };
            TextBox ttb = new TextBox() { Text = "name : type \n name2 : type2", AcceptsReturn = true };
            DockPanel sp = new DockPanel(); sp.Children.Add(btn); sp.Children.Add(tbClassName); sp.Children.Add(ttb);
            btn.Click += (sender2, e2) =>
            {
                var res = ParseCode(ttb.Text);
                data.Add(new CodexData() { className = tbClassName.Text, lstProperties = res });
            };
            sp.ToWindow();
        }

        private PropertiesDataList ParseCode(string p)
        {
            PropertiesDataList lst = new PropertiesDataList();
            foreach (var v in p.Replace('"', ' ').Replace(',', ' ').Split('\n'))
            {
                var res = v.Split(':');
                lst.Add(new PropertiesData() { name = res[0].Trim(), type = res[1].Trim() });
            }
            return lst;
        }

        private void XamarinData_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
                tb.Text = Codex.Code.Xamarin.GenerateXamlControl(data);
            }
        }

        private void miRazorTable_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.GenerateTable(data);
        }

        private void miModel_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_Model(data);
        }

        private void miDbContext_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_DbContext(data);
        }

        private void miAllMvc_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            var id = data.lstProperties.Where(x => x.name == "Id");
            if (id.Count() == 0)
                data.lstProperties.Add(new PropertiesData() { name = "Id", type = "int" });
            tb.Text = Codex.Code.MvcCoreEF.Generate_Config(data);
            //  tb.Text += Codex.Code.MvcCore.GenerateConnectionString(data);
            tb.Text += Codex.Code.MvcCoreEF.GenerateTable(data);
            tb.Text += Codex.Code.MvcCore.Generate_Model(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DbContext(data);
            tb.Text += Codex.Code.MvcCore.Generate_Controller(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_SeedExtension(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_EFCoreExtension(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_IDataRepository(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DataRepository(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DetailsView(data, new MvcTagHelperControlGenerator());
            tb.Text += Codex.Code.MvcCoreEF.Generate_EditView(data, new MvcTagHelperControlGenerator());
            tb.Text += Codex.Code.MvcCoreEF.Generate_CreateView(data, new MvcFormTagHelperControlGenerator());

        }

        private void miMvcController_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCore.Generate_Controller(data);
        }

        private void miSeedExtension_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_SeedExtension(data);

        }

        private void miConnectionString_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            //tb.Text = Codex.Code.MvcCore.GenerateConnectionString(data);
        }

        private void miIDataRepository_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_IDataRepository(data);
        }

        private void miDataRepository_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_DataRepository(data);
        }

        private void miEditView_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_EditView(data, new MvcTagHelperControlGenerator());
        }

        private void miDetailsView_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_DetailsView(data, new MvcTagHelperControlGenerator());
        }

        private void miListView_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.GenerateTable(data);
        }

        private void miCreateView_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_MainListCshtml(data, new MvcFormTagHelperControlGenerator());
        }

        private void miUpdateView_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCoreEF.Generate_CreateView(data, new MvcFormTagHelperControlGenerator());
        }



        #endregion

        #region Drag and Drop

        public void DragOver2(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) { e.Effects = DragDropEffects.All; }
            else { e.Effects = DragDropEffects.None; }
            e.Handled = false;
        }

        public void Drop2(object sender, DragEventArgs e)
        {
            string s = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                var dataFormat = DataFormats.Text;
                if (System.IO.File.Exists(docPath[0]))
                {
                    //using (StreamReader sr = new StreamReader(docPath[0])) { s = sr.ReadToEnd(); }
                    if (docPath[0].EndsWith(".dll"))
                    {
                        var DLL = Assembly.LoadFile(docPath[0]);
                        foreach (Type type in DLL.GetExportedTypes())
                        {
                            try
                            {
                                // var c = Activator.CreateInstance(type);
                                CodexDataList lst = new CodexDataList();
                                var data = new CodexData() { className = type.Name };
                                lst.Add(data);
                                foreach (var v in type.GetProperties())
                                {
                                    data.lstProperties.Add(new PropertiesData()
                                    {
                                        type = v.PropertyType.ToString(),
                                        name = v.Name
                                    });
                                }
                                CodexDataList lst2 = (DataContext as CodexDataList);
                                lst.ForEach(x => lst2.Add(x));
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                //catch (System.Exception e2)
                //{
                //    MessageBox.Show(e2.Message);
                //}
            }
        }

        #endregion

        private void miIndividual_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            foreach (var v in data.lstProperties)
            {
                tb.Text += Codex.Code.Blazor.GenerateDefaultControl(v) + Environment.NewLine;
            }
        }

        private void miProperties_Click(object sender, RoutedEventArgs e)
        {
            CodexDataList lst = (DataContext as CodexDataList);
            tb.Text = "";
            foreach (CodexData data in lb.SelectedItems)
            {
                string s = AutomationControls.Codex.Code.CS.GenerateWPFDataClass(data);
                tb.Text += RemoveEmptyLines(s);
            }
        }

        #region MVC EFCore

        private void miAllMvcEF_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            var id = data.lstProperties.Where(x => x.name == "Id");
            if (id.Count() == 0)
                data.lstProperties.Add(new PropertiesData() { name = "Id", type = "int" });
            tb.Text = Codex.Code.MvcCoreEF.Generate_Config(data);
            //  tb.Text += Codex.Code.MvcCore.GenerateConnectionString(data);
            tb.Text += Codex.Code.MvcCoreEF.GenerateTable(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_Model(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DbContext(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_Controller(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_SeedExtension(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_EFCoreExtension(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_IDataRepository(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DataRepository(data);
            tb.Text += Codex.Code.MvcCoreEF.Generate_DetailsView(data, new MvcTagHelperControlGenerator());
            tb.Text += Codex.Code.MvcCoreEF.Generate_EditView(data, new MvcTagHelperControlGenerator());
            tb.Text += Codex.Code.MvcCoreEF.Generate_CreateView(data, new MvcFormTagHelperControlGenerator());
        }

        private void miModelEF_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCore.Generate_Model(data);
        }

        private void miMvcControllerEF_Click(object sender, RoutedEventArgs e)
        {

            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = Codex.Code.MvcCore.Generate_Controller(data);

        }
        #endregion


        #region Blazor

        private void miAllBlazor_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = "";

            tb.Text += Codex.Code.Blazor.GenerateService(data);

            var dataPath = Path.Combine(@"C:\SerializedData\Blazor", data.className, "Components", data.className, data.className + ".cs");
            tb.Text += Codex.Code.CS.GenerateWPFDataClass(data);

            tb.Text += Codex.Code.Blazor.GenerateRazorTableComponent(data);
            // tb.Text += Codex.Code.Blazor.Generate_CreateView(data, new BlazorControlGenerator());
            tb.Text += Codex.Code.Blazor.Generate_EditView(data, new BlazorControlGenerator());
            // tb.Text += Codex.Code.Blazor.Generate_UpdateView(data, new BlazorControlGenerator());
            // tb.Text += Codex.Code.Blazor.Generate_DetailsView(data, new BlazorControlGenerator());
            // tb.Text += Codex.Code.Blazor.GenerateRazorIndex(data);

            data.lstProperties.Where(x => x.IsList).ForEach(x => tb.Text += Codex.Code.Blazor.GenerateRazorListComponent(data, x));

        }

        private void miBlazorEditView_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
                tb.Text += Codex.Code.Blazor.Generate_EditView(data, new BlazorControlGenerator());

            }
        }

        private void miBlazorDetailsView_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
                tb.Text += Codex.Code.Blazor.Generate_DetailsView(data, new MvcTagHelperControlGenerator());
            }
        }



        private void miBlazorCreateView_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
                tb.Text += Codex.Code.Blazor.Generate_EditView(data, new MvcTagHelperControlGenerator());
            }
        }
        private void miBlazorUpdateView_Click(object sender, RoutedEventArgs e)
        {
            foreach (CodexData data in lb.SelectedItems)
            {
            }
        }
        #endregion

        private void miAllBlazorEF_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = "";


                if (data.lstProperties.Where(x => x.name == "Id").Count() == 0)
                    data.lstProperties.Add(new PropertiesData() { name = "Id", type = "int" });

            //Shared
            var dataPath = Path.Combine(@"C:\SerializedData\BlazorEF",  "Shared" , "Models", data.className + ".cs");
            tb.Text += Codex.Code.CS.GenerateWPFDataClass(data);

            //Server
            tb.Text += Codex.Code.BlazorEF.Generate_Config(data);
            tb.Text += Codex.Code.BlazorEF.Generate_DbContext(data);
            tb.Text += Codex.Code.BlazorEF.Generate_IDataRepository(data);
            tb.Text += Codex.Code.BlazorEF.Generate_DataAccessLayer(data);
            tb.Text += Codex.Code.BlazorEF.Generate_Controller(data);

            //Client
            tb.Text += Codex.Code.BlazorEF.Generate_DataControl(data);
            tb.Text += Codex.Code.BlazorEF.Generate_DataControlRazor(data);
            tb.Text += Codex.Code.BlazorEF.Generate_DataControlCss(data);


            tb.Text += Codex.Code.BlazorEF.Generate_AddEditControl(data);
            tb.Text += Codex.Code.BlazorEF.Generate_AddEditControlRazor(data);
            tb.Text += Codex.Code.BlazorEF.Generate_AddEditControlCss(data);


            tb.Text += Codex.Code.BlazorEF.GenerateRazorTableComponent(data);
            // tb.Text += Codex.Code.Blazor.Generate_CreateView(data, new BlazorControlGenerator());
            //tb.Text += Codex.Code.BlazorEF.Generate_EditView(data);
            // tb.Text += Codex.Code.Blazor.Generate_UpdateView(data, new BlazorControlGenerator());
            // tb.Text += Codex.Code.Blazor.Generate_DetailsView(data, new BlazorControlGenerator());
            // tb.Text += Codex.Code.Blazor.GenerateRazorIndex(data);

            data.lstProperties.Where(x => x.IsList).ForEach(x => tb.Text += Codex.Code.Blazor.GenerateRazorListComponent(data, x));
        }

        private void miEnumBindingSource_Click(object sender, RoutedEventArgs e)
        {
           tb.Text =  Properties.Resources.EnumBindingSource;
        }

        private void miXamlDataboundControls_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = RemoveEmptyLines(xaml.GenerateDataboundControls(data));
        }

        private void miXamlDataboundDataGridControls_Click(object sender, RoutedEventArgs e)
        {
            CodexData data = lb.SelectedItem as CodexData;
            if (data == null) return;
            tb.Text = xaml.GenerateDataboundDataGrid(data);

        }

        private string RemoveEmptyLines(string s)
        {
            string ret = "";
            var lst = s.Split(Environment.NewLine);
            lst.Where(x => x != string.Empty).ForEach(x => ret += x + Environment.NewLine );
            return ret;
        }

      
    }
}