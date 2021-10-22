using AutomationControls.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AutomationControls.Serialization
{
    public enum SerializationType
    {
        None,
        JSON,
        XML,
        CSV
    }

    public class Serializer<T> where T : new()
    {
        T t = new T();
        SerializationType serializationType = SerializationType.JSON;

        public Serializer(T t) { this.t = t; }
        public Serializer(T t, SerializationType type = SerializationType.JSON) { this.t = t; serializationType = type; }
        public Serializer() { }

        private string _defaultSerializationPath = String.Empty;
        public virtual string defaultSerializationPath
        {
            get
            {
                if (_defaultSerializationPath == String.Empty)
                {

                }
                return _defaultSerializationPath;
            }
            set
            {
                _defaultSerializationPath = value;
                // OnPropertyChanged("defaultSerializationPath");
            }
        }

        public void GenerateSerializationPath()
        {
            // string a = (t.GetType().FullName.ToString().Split(new string[] { ".", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Aggregate((i, j) => { return Path.Combine(i, j); }));
            string defaultBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Serialized Data");
            if (!Directory.Exists(defaultBasePath)) Directory.CreateDirectory(defaultBasePath);
            string defaultFileName = t.GetType().Name;
            string suffix = "";
            switch (serializationType)
            {
                case SerializationType.None:
                    break;
                case SerializationType.JSON:
                    suffix = ".json";
                    break;
                case SerializationType.XML:
                    suffix = ".xml";
                    break;
                case SerializationType.CSV:
                    suffix = ".csv";
                    break;
                default:
                    break;
            }
            defaultFileName += suffix;
            defaultSerializationPath = Path.Combine(defaultBasePath, defaultFileName);
        }

        public void Serialize(SerializationType type = SerializationType.None, string path = "")
        {
            if (path == "")
                GenerateSerializationPath();
            else
                defaultSerializationPath = path;

            if (type != SerializationType.None)
                serializationType = type;

            switch (serializationType)
            {
                case SerializationType.JSON:
                    this.ToJSON(defaultSerializationPath);
                    break;
                case SerializationType.XML:
                    this.ToXML(defaultSerializationPath);
                    break;
                case SerializationType.CSV:
                    this.ToCSV_Values();
                    break;
                default:
                    break;
            }
        }

        public T Deserialize(SerializationType type = SerializationType.None, string path = "")
        {
            T ret = new T();

            if (path == "")
                GenerateSerializationPath();
            else
                defaultSerializationPath = path;

            if (type != SerializationType.None)
                serializationType = type;

            switch (serializationType)
            {
                case SerializationType.JSON:
                    ret = this.FromJSON(defaultSerializationPath);
                    break;
                case SerializationType.XML:
                    this.FromXML(defaultSerializationPath);
                    break;
                case SerializationType.CSV:
                    this.FromCSV_Values(defaultSerializationPath);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public string ToCSV_Names()
        {
            String s = "";
            List<PropertyInfo> pi = t.GetType().GetProperties().ToList();

            foreach (var v in pi)
            {
                s += v.Name + ",";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        public string ToCSV_Types()
        {
            String s = "";
            List<PropertyInfo> pi = t.GetType().GetProperties().ToList();

            foreach (var v in pi)
            {
                s += v.PropertyType + ",";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        public string ToCSV_Values()
        {
            String s = "";
            List<PropertyInfo> pi = t.GetType().GetProperties().ToList();

            foreach (var v in pi)
            {
                if (v.DeclaringType != t.GetType()) continue;
                if (v != pi.Last()) { s += v.GetValue(t) + ","; }
                else { s += v.GetValue(t); }
            }
            s = s.Remove(s.Length - 1);
            return s;
        }


        public virtual T FromCSV_Values(string csv)
        {
            T ret = new T();
            List<PropertyInfo> pi = t.GetType().GetProperties().ToList();
            string[] str = csv.Split(new[] { "," }, StringSplitOptions.None);
            int i = 0;
            foreach (var v in pi)
            {
                if (v.DeclaringType != t.GetType()) continue;
                var type = v.PropertyType;
                var res = Convert.ChangeType(str[i], type);
                v.SetValue(ret, res);
                i++;
            }
            return ret;
        }


        public Task ToJSON_Async(string path)
        {
            return Task.Run(() => { ToJSON(path); }, CancellationToken.None);
        }

        public Task<T> FromJSON_Async(string path)
        {
            return Task.FromResult<T>(FromJSON(path));
        }

        public void ToJSON(string path)
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
            jsonData.ToFile(path);
        }

        //public string ToJSON()
        //{
        //    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented,
        //        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
        //   return   jsonData;
        //}

        public T FromJSON(string path)
        {
            if (!File.Exists(path)) return new T();
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
            return (T)jsonData;
        }
        public T FromJSONString(string s)
        {
            if (s.IsNull()) return new T();
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
            return (T)jsonData;
        }

        public T DeserializeJSONIgnoreMissing(string file)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }


        public string ToBinary(string path)
        {
            System.IO.FileStream s = new System.IO.FileStream(path, System.IO.FileMode.Create);
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(s, t);
            s.Close();
            return File.ReadAllText(path);
        }

        public T FromBinary(string path)
        {
            T ret = new T();
            FileStream fs = new FileStream(path, FileMode.Open);
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            ret = (T)formatter.Deserialize(fs);
            fs.Close();
            return ret;
        }



        public string GeneratePath(object o, string suffix = ".json")
        {
            string path = @"C:\Saved Data\";
            o.GetType().Namespace.Split(new[] { ".", "]", "[", ",", " ", "=", "~1" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => path += x + "\\");
            path += o.GetType().Name + "\\";
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            path += typeof(T).Name + suffix;
            return path;
        }

        #region XML

        private object lockSerialization = new object();
        public virtual bool ToXML(String path = null)
        {
            if (!String.IsNullOrEmpty(path))
                defaultSerializationPath = path;
            if (String.IsNullOrEmpty(defaultSerializationPath))
                GenerateSerializationPath();

            lock (lockSerialization)
            {
                if (this == null) return false;


                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);

                xmlWriter.Formatting = System.Xml.Formatting.Indented;

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path)) { xmlSerializer.Serialize(sw, this); }
                return true;
            }
        }

        public bool ToXML(Stream stream, String path = null)
        {
            if (!String.IsNullOrEmpty(path))
                defaultSerializationPath = path;
            if (String.IsNullOrEmpty(defaultSerializationPath))
                GenerateSerializationPath();

            lock (lockSerialization)
            {
                if (!String.IsNullOrEmpty(path))
                {
                    if (this == null) return false;
                    if (stream == null) return false;

                    XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

                    StringWriter stringWriter = new StringWriter();
                    XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);

                    xmlWriter.Formatting = System.Xml.Formatting.Indented;

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                    {
                        xmlSerializer.Serialize(sw, this);
                        sw.BaseStream.Close();
                    }
                    return true;
                }
                return false;
            }
        }

        public virtual T FromXML(String path = null)
        {
            if (!String.IsNullOrEmpty(path))
                defaultSerializationPath = path;
            if (String.IsNullOrEmpty(defaultSerializationPath))
                GenerateSerializationPath();

            lock (lockSerialization)
            {
                if (!String.IsNullOrEmpty(path))
                {
                    try
                    {
                        T ret = new T();
                        Type typeT = this.GetType();
                        if (path == default(string)) { return new T(); }

                        if (!File.Exists(path))
                        {
                            System.Windows.MessageBox.Show("Cannot Deserialize " + typeT.ToString() + Environment.NewLine + path + Environment.NewLine + "File does not exist.");
                            return new T();
                        }

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                        {
                            XmlSerializer xs = new XmlSerializer(typeT);
                            ret = (T)xs.Deserialize(sr);
                        }
                        return ret;
                    }
                    catch { return new T(); }
                }
                return default(T);
            }
        }

        public T FromXML(Stream stream)
        {
            if (String.IsNullOrEmpty(defaultSerializationPath))
                GenerateSerializationPath();

            lock (lockSerialization)
            {
                if (this == null) return default(T);
                if (stream == null) return default(T);

                try
                {
                    T ret = new T();

                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        ret = (T)xs.Deserialize(sr);
                    }
                    return ret;
                }
                catch { return new T(); }
            }
        }

        #endregion
    }

}
