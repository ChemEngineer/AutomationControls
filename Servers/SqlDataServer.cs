using AutomationControls.Attributes;
using AutomationControls.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AutomationControls.Servers
{


    [DataProfileAttribute(typeof(TcpServerControl))]
    public class SqlDataServer : AsyncTcpServer<SqlData>
    {
        public override string Identify()
        {
            return "SqlDataServer";
        }

        public override void ProcessCommand(TcpClient tcpClient)
        {
            try
            {
                GetPostingData(tcpClient);
            }
            catch { }

        }

        private SqlData GetPostingData(TcpClient client)
        {
            SqlData dat = new SqlData();
            Serializer<SqlData> ser = new Serializer<SqlData>(dat);
            var json = base.split[2];

            var postingdata = ser.FromJSONString(json);
            var ip = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            OnDataReadyEvent(new DataReadyEventArgs() { Message = postingdata, ipAddress = ip });

            return postingdata;
        }

        public override void ProcessClient(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.ReceiveTimeout = 2000;
                    tcpClient.SendTimeout = 2000;
                    stream = tcpClient.GetStream();
                    Task.Factory.StartNew(async () =>
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            if (!stream.DataAvailable)
                            {
                                await Task.Delay(100, cts.Token);
                                continue;
                            }
                            else
                            {
                                string command = "";
                                string ret = "";
                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    ret = await sr.ReadLineAsync();
                                    using (StreamWriter sw = new StreamWriter(stream))
                                    {
                                        split = ret.Split(new string[] { "_~_~" }, StringSplitOptions.RemoveEmptyEntries);
                                        command = split.First();

                                        if (command.Equals("1"))
                                        {
                                            string s = Identify() + "\r\n";
                                            sw.Write(s);
                                            sw.Flush();
                                            OnDataReadyEvent(new DataReadyEventArgs() { ipAddress = ipAddress, text = s });
                                        }
                                        else if (command.Equals("2")) //Get Data
                                        {
                                            //2_~_~PostingData_~_~I1000
                                            string tableName = split[1];
                                            string key = split[2];
                                            var dat = Sql.StaticMethods.GetData(tableName, key);
                                            sw.Write(dat + "\r\n");
                                            sw.Flush();
                                            OnDataReadyEvent(new DataReadyEventArgs() { ipAddress = ipAddress, text = "Data Received", Message = new SqlData() { id = key, data = dat } });
                                        }
                                        else if (command.Equals("3")) //Set Data
                                        {
                                            //3_~_~PostingData_~_~I1000_~_~data
                                            string tableName = split[1];
                                            string key = split[2];
                                            string data = split[3];
                                            Sql.StaticMethods.SetData(tableName, key, data, DateTime.Now);
                                        }
                                        else if (command.Equals("4")) //Get skus since date
                                        {
                                            //4_~_~PostingData_~_~Date
                                            string tableName = split[1];
                                            DateTime dt = DateTime.Parse(split[2]);
                                            var lst = Sql.StaticMethods.GetSinceDateUpdated(tableName, dt);
                                            var res = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                                            sw.Write(res);

                                        }
                                        else if (command.Equals("5")) //Get all skus 
                                        {
                                            //5_~_~PostingData_
                                            string tableName = split[1];
                                            var lst = Sql.StaticMethods.GetColumnData(tableName, "id");
                                            var res = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                                            sw.Write(res);

                                        }
                                        else if (command.Equals("6")) //Get all skus 
                                        {
                                            //6_~_~PostingData_
                                            string tableName = split[1];
                                            var lst = Sql.StaticMethods.GetAllData(tableName).OrderBy(x => x.id);
                                            var res = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                                            sw.Write(res);

                                        }
                                    }
                                }
                            }
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }
    }



    //public class SqlDataServerData : TcpServerData, INotifyPropertyChanged, ISerializable, IProvideUserControls
    //{

    //    public UserControl[] GetUserControls()
    //    {
    //        db.lstUserControls.Clear();
    //        UserControl cc = new UserControl();
    //        var attrs = (DataProfileAttribute[])this.GetType().GetCustomAttributes(typeof(DataProfileAttribute), true);
    //        foreach (var attr in attrs)
    //        {
    //            var type = ((DataProfileAttribute)attr).ClassName;
    //            //Check to see if we already have the control created - Avoid binding errors due to recreation
    //            if (db.lstUserControls.Where(x => x.GetType().Name == type.Name).Count() == 0)
    //            {
    //                cc = (UserControl)Activator.CreateInstance(type);
    //                if (cc == null) continue;
    //                db.lstUserControls.Add(cc);
    //                cc.DataContext = this;
    //            }
    //        }
    //        return db.lstUserControls.ToArray();
    //    }
    //}
}