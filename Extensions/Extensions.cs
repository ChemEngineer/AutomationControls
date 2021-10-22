using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Reflection;
using swt = System.Windows.Threading;
using System.IO;
using System.Windows.Documents;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO.Pipes;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class Extensions
        {
            //public static async Task<string> GetStringAsync(this NamedPipeServerStream stream, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
            //{
            //    string ret = "";
            //    await Task.Factory.StartNew(() =>
            //    {
            //        var received = new byte[4096];
            //        var bytesRead = 0;
            //        bytesRead = stream.Read(received, 0, 4096);
            //        System.Array.Resize(ref received, bytesRead);
            //        ret = Encoding.ASCII.GetString(received);
            //        if (status != null) status.Report(ret);
            //    });
            //    return ret;
            //}

            //public static async Task WaitForConnectionAsync(this NamedPipeServerStream stream, IProgress<string> status = default(Progress<string>), CancellationToken ct = default(CancellationToken))
            //{
            //    var tcs = new TaskCompletionSource<bool>();
            //    var cancelRegistration = ct.Register(() => tcs.SetCanceled());
            //    var iar = stream.BeginWaitForConnection(null, null);
            //    var rwh = ThreadPool.RegisterWaitForSingleObject(iar.AsyncWaitHandle, delegate { tcs.TrySetResult(true); }, null, -1, true);
            //    try
            //    {
            //        await tcs.Task.ConfigureAwait(false);
            //        if (iar.IsCompleted)
            //        {
            //            stream.EndWaitForConnection(iar);
            //        }
            //    }
            //    finally
            //    {
            //        cancelRegistration.Dispose();
            //        rwh.Unregister(null);
            //    }
            //}


            public static T GetItemByType<T>(this  IList<object> lst) where T : new()
            {
                var ret = lst.Where(x => x.GetType() == typeof(T));
                if (ret.Count() > 0)
                    return (T)ret.ToArray()[0];

                return default(T);
            }


            public static string ToSqlTableString(this Type t)
            {
                string sqlsc;
                sqlsc = "CREATE TABLE " + t.Name + "(";
                var props = t.GetProperties();
                  foreach(var prop in props)
                  {
                      sqlsc += "\n [" + prop.Name + "] ";
                      string columnType = prop.PropertyType.ToString();
                      switch (columnType)
                      {
                          case "System.Int32":
                              sqlsc += " int ";
                              break;
                          case "System.Int64":
                              sqlsc += " bigint ";
                              break;
                          case "System.Int16":
                              sqlsc += " smallint";
                              break;
                          case "System.Byte":
                              sqlsc += " tinyint";
                              break;
                          case "System.Decimal":
                              sqlsc += " decimal ";
                              break;
                          case "System.DateTime":
                              sqlsc += " datetime ";
                              break;
                          case "System.String":
                          default:
                              sqlsc += string.Format(" nvarchar(256) ");
                              break;
                      }
                      if (prop != props.Last())
                          sqlsc += ",";
                  }
                  return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
            }

            public static void ToSqlTableString(this DataTable dt)
            {
                string sqlsc;
                sqlsc = "CREATE TABLE " + dt.TableName + "(";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sqlsc += "\n [" + dt.Columns[i].ColumnName + "] ";
                    string columnType = dt.Columns[i].DataType.ToString();
                    switch (columnType)
                    {
                        case "System.Int32":
                            sqlsc += " int ";
                            break;
                        case "System.Int64":
                            sqlsc += " bigint ";
                            break;
                        case "System.Int16":
                            sqlsc += " smallint";
                            break;
                        case "System.Byte":
                            sqlsc += " tinyint";
                            break;
                        case "System.Decimal":
                            sqlsc += " decimal ";
                            break;
                        case "System.DateTime":
                            sqlsc += " datetime ";
                            break;
                        case "System.String":
                        default:
                            sqlsc += string.Format(" nvarchar({0}) ", dt.Columns[i].MaxLength == -1 ? "max" : dt.Columns[i].MaxLength.ToString());
                            break;
                    }
                    if (dt.Columns[i].AutoIncrement)
                        sqlsc += " IDENTITY(" + dt.Columns[i].AutoIncrementSeed.ToString() + "," + dt.Columns[i].AutoIncrementStep.ToString() + ") ";
                    if (!dt.Columns[i].AllowDBNull)
                        sqlsc += " NOT NULL ";
                    sqlsc += ",";
                }
                //return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
            }

            public static string ToJsonString<T>(this T t) where T :ISerializable, new()
            {
                AutomationControls.Serialization.Serializer<T> ser = new Serialization.Serializer<T>(t);
                 ser.ToJSON("C:\\j.json");
                 return File.ReadAllText("C:\\j.json");
            }

            public static T FromJsonString<T>(this T t, string json) where T : ISerializable, new()
            {
                AutomationControls.Serialization.Serializer<T> ser = new Serialization.Serializer<T>(t);
                return ser.FromJSONString(json);
            }

            public static DataTable ToDataTable<T>(this Type t, string tableName = "")
            {
                Type type = typeof(T);
                if (tableName == "") tableName = type.Name;
                var properties = type.GetProperties();

                DataTable dt = new DataTable() { TableName = tableName };
                foreach (PropertyInfo info in properties)
                {
                    dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                }
                return dt;
            }

            public static DataTable ToDataTable<T>(this IEnumerable<T> list)
            {
                Type type = typeof(T);
                var properties = type.GetProperties();

                DataTable dataTable = new DataTable();
                foreach (PropertyInfo info in properties)
                {
                    dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                }

                foreach (T entity in list)
                {
                    object[] values = new object[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        values[i] = properties[i].GetValue(entity);
                    }

                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }

           
          
            /// <summary>
                          /// 
                          /// </summary>
                          /// <typeparam name="T"></typeparam>
                          /// <param name="array"></param>
                          /// <param name="size">Size of each array</param>
                          /// <returns></returns>
            public static IEnumerable<IEnumerable<T>> SplitBySize<T>(this T[] array, int size)
            {
                for(var i = 0; i < (float)array.Length / size; i++)
                {
                    yield return array.Skip(i * size).Take(size);
                }
            }

            public static Collection<T> ToCollection<T>(this T[] array)
            {
                      Collection<T> lst = new Collection<T>();
                      array.ForEach(x => lst.Add(x));
                      return lst;
            }

            public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> array)
            {
                ObservableCollection<T> lst = new ObservableCollection<T>();
                array.ForEach(x => lst.Add(x));
                return lst;
            }

            public static void AddRange<T>(this Collection<T> coll, IEnumerable<T> lst)
            {
                foreach(var v in lst) { coll.Add(v); }
            }

            public static  Task<TcpClient> AcceptTcpClientAsync(this TcpListener listener, CancellationToken token)
    {
        try
        {
            return  listener.AcceptTcpClientAsync();
        }
        catch (Exception ex) 
        { 
           return null;
        }
    }
            public static T[] SubArray<T>(this T[] data, int index, int length)
            {
                T[] result = new T[length];
                System.Array.Copy(data, index, result, 0, length);
                return result;
            }

            public static void ToFile(this byte[] b, string path)
            {
                 FileStream Fs = new FileStream (path, FileMode.Create, FileAccess.Write);          
                 Fs.Write(b, 0, b.Count());
                 Fs.Close();
            }

            //public static List<PropertyInfo> GetPropertiesWithCustomAttribute<T>(this Type t) where   T: Attribute
            //{
            //   var res =  t.GetProperties().Where(x => x.GetCustomAttribute<T>() != null).Select(y => y);
            //   return res.ToList();
            //}

            //public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
            //{
            //    if(sequence == null) throw new ArgumentNullException("sequence");
            //    if(action == null) throw new ArgumentNullException("action");
            //    foreach(T item in sequence)
            //        action(item);
            //}

            public static void ToCSV(this Type type, string path = null)
            {
                String s = ""; String p = "";
                List<PropertyInfo> pi = type.GetProperties().ToList();
                foreach(var v in pi)
                {
                    if(v != pi.Last())
                    {
                        s += v.Name + ",";
                        p += v.PropertyType.ToString().Split(new[] { "." }, StringSplitOptions.None).Last() + ",";
                    }
                    else
                    {
                        s += v.Name;
                        p += v.PropertyType.ToString().Split(new[] { "." }, StringSplitOptions.None).Last();
                    }
                }
                if(string.IsNullOrEmpty(path))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + type.Name + ".csv";
                }
                try
                {
                    FileInfo fi = new FileInfo(path);
                    if(!fi.Directory.Exists) fi.Directory.Create();
                    File.WriteAllText(path, s + Environment.NewLine + p);
                } catch { }

            }

            public static TOutput[,] ConvertAll<TInput, TOutput>(this TInput[,] array, Converter<TInput, TOutput> converter)
            {
                if(array == null)
                {
                    throw new ArgumentNullException("array");
                }
                TOutput[,] output = new TOutput[array.GetLength(0), array.GetLength(1)];
                System.Array.Copy(array, output, array.Length);
                return output;
            }

            #region anonomyous type xml serialization
            private static readonly Type[] WriteTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };
            public static bool IsSimpleType(this Type type) { return type.IsPrimitive || WriteTypes.Contains(type); }
            public static XElement ToXml(this object input) { return input.ToXml(null); }
            public static XElement ToXml(this object input, string element)
            {
                if(input == null)
                    return null;

                if(string.IsNullOrEmpty(element))
                    element = "object";
                element = XmlConvert.EncodeName(element);
                var ret = new XElement(element);

                if(input != null)
                {
                    var type = input.GetType();
                    var props = type.GetProperties();

                    var elements = from prop in props
                                   let name = XmlConvert.EncodeName(prop.Name)
                                   let val = prop.GetValue(input, null)
                                   let value = prop.PropertyType.IsSimpleType()
                                        ? new XElement(name, val)
                                        : val.ToXml(name)
                                   where value != null
                                   select value;

                    ret.Add(elements);
                }

                return ret;
            }
            #endregion
            //not tested

            public static T DeepCopy<T>(this T objectToCopy)
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);

                memoryStream.Position = 0;
                T returnValue = (T)binaryFormatter.Deserialize(memoryStream);

                memoryStream.Close();
                memoryStream.Dispose();

                return returnValue;
            }


            public static int AsPercent(this byte number) { return Convert.ToInt32((double)number / 255 * 100); }

            public static TextRange FindWordFromPosition(this TextPointer position, string word)
            {
                while(position != null)
                {
                    if(position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string textRun = position.GetTextInRun(LogicalDirection.Forward);

                        // Find the starting index of any substring that matches "word".
                        int indexInRun = textRun.IndexOf(word);
                        if(indexInRun >= 0)
                        {
                            TextPointer start = position.GetPositionAtOffset(indexInRun);
                            TextPointer end = start.GetPositionAtOffset(word.Length);
                            return new TextRange(start, end);
                        }
                    }
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }
                // position will be null if "word" is not found.
                return null;
            }


            public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            {
                HashSet<TKey> seenKeys = new HashSet<TKey>();
                foreach(TSource element in source)
                {
                    if(seenKeys.Add(keySelector(element)))
                    {
                        yield return element;
                    }
                }
            }


            public static String[] GetValues(this Enum sender)
            {
                List<String> ret = new List<string>();
                var vals = Enum.GetValues(sender.GetType());
                return ret.ToArray();
            }

            

            #region "Depreciated"


            public static void DoEvents(this Application source)
            {
                swt.DispatcherFrame frame = new swt.DispatcherFrame();
                swt.Dispatcher.CurrentDispatcher.BeginInvoke(swt.DispatcherPriority.Background, new swt.DispatcherOperationCallback(ExitFrame), frame);
                swt.Dispatcher.PushFrame(frame);
            }

            private static object ExitFrame(object f)
            {
                (f as swt.DispatcherFrame).Continue = false;
                return null;
            }

            #endregion




            // [DllImport("user32.dll")]
            // public static extern bool GetCursorPos(out POINT lpPoint);

            //public  static Point RealPixelsToWpf(this Window w, Point p)
            // {
            //     var t = PresentationSource.FromVisual(w).CompositionTarget.TransformFromDevice;
            //     return t.Transform(p);
            // }


            //public static string[] Get_Exif_Data(ExifLib.ExifReader source, string JpgPath)
            //{

            //    List<string> ret = new List<string>();

            //    ExifLib.ExifReader exifRead = new ExifLib.ExifReader(JpgPath);
            //    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            //    dynamic val = null;
            //    ExifLib.ExifTags exifEnum = new ExifLib.ExifTags();
            //    ExifLib.ExifTags[] en = (ExifLib.ExifTags[])Enum.GetValues(exifEnum.GetType());
            //    string[] ev = Enum.GetNames(exifEnum.GetType());

            //    for (int i = 0; i <= en.GetUpperBound(0); i++)
            //    {
            //        exifRead.GetTagValue(Convert.ToUInt16(en[i]), val);
            //        if (val == null)
            //            continue;
            //        string valType = val.GetType().ToString();
            //        switch (valType)
            //        {
            //            case  // ERROR: Case labels with binary operators are unsupported : Equality
            //            "System.Byte[]":
            //                val = encoder.GetString(val);
            //                break;
            //        }
            //        ret.Add(ev[i] + ": " + val);
            //        val = null;
            //    }

            //    return ret.ToArray();
            //}

            #region "String"

            //public static bool Contains(this string source, string value, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
            //{
            //    if(source == null || value == null) return false;
            //    return source.IndexOf(value, comparisonType) >= 0;
            //}

          


            #endregion


            public static string ToString(this byte[] bytes)
            {
                string s = string.Empty;
                foreach(byte b in bytes)
                {
                    s += (char)b;
                }
                return s;
            }

          

        }
    }
}