using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AutomationControls.Sql
{
    public class StaticMethods
    {//
        //static string dbName = "EbayPoster";
        //static string serverName = "MYDB";
        //static string userName = "Enthalpy";
        //static string Password = "En3123123";

        //static string serverName = "DELMONICOSERVER\\DB";
        //static string userName = "Entropy";
        //static string Password = "En3123123";


        public static string connectionString
        {
            get
            {
                // return "Server=tcp:" + serverName + ";DataBase=" + dbName + ";User Id=" + userName + ";Password=" + Password + ";";
                return "Server=GAMES-PC;DataBase=EbayPoster;User Id=Entropy;Password=En3123123;";
               // return "Server=DELTA-PC\\MYDB;DataBase=EbayPoster;User Id=Enthalpy;Password=En3123123;";
                //    return "Server=" + serverName + "Database=" + dbName + ";User Id=" + userName + ";password=" + Password ;
                //   return "Data Source = " +  serverName + "/" + dbName + "," + port + ";" +  "User id=" + userName + ";" + "password=" + Password + ";"  + "connection timeout=30;";
            }
        }


        public static string GetData(string tableName, string key)
        {
            string ret = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT data FROM " + tableName + " WHERE id = @id ;", connection);
                command.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = key;
                connection.Open();

                try
                {
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ret = reader.GetString(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
                catch
                {
                    ret = "";
                }
            }
            return ret;
        }

        //public static SqlDataList GetAllData(string tableName)
        //{
        //    SqlDataList lst = new SqlDataList();
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand("SELECT * FROM " + tableName + ";", connection);
        //        connection.Open();

        //        SqlDataReader reader = command.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                SqlData data = new SqlData() { data = (string)reader["data"], id = (string)reader["id"], lastUpdated = (DateTime)reader["lastUpdated"] };
        //                lst.Add(data);
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("No rows found.");
        //        }
        //        reader.Close();
        //    }
        //    return lst;
        //}

        public static SqlDataList GetAllData(string tableName)
        {
            SqlDataList lst = new SqlDataList();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM " + tableName + ";", connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        SqlData data = new SqlData() { data = (string)reader["data"], id = (string)reader["id"], lastUpdated = DateTime.Parse((string)reader["lastUpdated"]) };
                        lst.Add(data);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            return lst;
        }


        public static SqlDataList GetLastUpdated(string tableName)
        {
            string res = "";
            SqlDataList lst = new SqlDataList();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = @"Declare @val Varchar(MAX); 
                         Select @val = COALESCE(@val + '{id:' + [id] + ',' + 'lastUpdated:' + [lastUpdated] + '},','{id:' + [id] + ',' + 'lastUpdated:' + [lastUpdated] + '},') FROM " + tableName + ";";
                SqlCommand command = new SqlCommand(cmd, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        res += reader.ToString();
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            res = "[" + res + "]";
            lst = SqlDataList.Deserialize(res);
            return lst;
        }

        public static List<string> GetColumnData(string tableName, string columnName)
        {
            List<string> ret = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT TOP 1000000 " + columnName + " FROM PostingData;", connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ret.Add(reader.GetString(0));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            return ret;
        }


        public static bool SetData(string tableName, string key, string data, DateTime lastUpdated)
        {
            int ctr = 0;
            string cmd = "";
            SqlCommand command = new SqlCommand();
            command.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = key;
            command.Parameters.AddWithValue("@data", SqlDbType.VarChar).Value = data;
            command.Parameters.AddWithValue("@lastUpdated", SqlDbType.DateTime).Value = lastUpdated;

            var keys = Sql.StaticMethods.GetColumnData(tableName, "id").Where(x => x == key);
            if (keys.Count() == 0)
                cmd = "INSERT INTO " + tableName + "(id,data, lastUpdated) VALUES (@id, @data, @lastUpdated);";
            else
                cmd = "UPDATE " + tableName + " SET data = @data, lastUpdated = @lastUpdated WHERE id = @id;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = cmd;
                command.Connection = connection;
                ctr = command.ExecuteNonQuery();
            }

            return (ctr == 1) ? true : false;
        }

        public static bool SetList(string tableName, List<SqlData> lst)
        {
            int ctr = 0;
            string cmd = "";
            var keys = Sql.StaticMethods.GetColumnData("PostingData", "id");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var v in lst)
                {

                    SqlCommand command = new SqlCommand();
                    command.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = v.id;
                    command.Parameters.AddWithValue("@data", SqlDbType.VarChar).Value = v.data;
                    command.Parameters.AddWithValue("@lastUpdated", SqlDbType.DateTime).Value = v.lastUpdated;

                    var ret = keys.Where(x => x == v.id);
                    if (ret.Count() == 0)
                        cmd = "INSERT INTO " + tableName + "(id,data, lastUpdated) VALUES (@id, @data, @lastUpdated);";
                    else
                        cmd = "UPDATE " + tableName + " SET data = @data, lastUpdated = @lastUpdated WHERE id = @id;";

                    command.CommandText = cmd;
                    command.Connection = connection;
                    ctr = command.ExecuteNonQuery();
                }
            }
            return (ctr == 1) ? true : false;
        }

        public static List<string> GetSinceDateUpdated(string tableName, DateTime dt)
        {
            var res = Sql.StaticMethods.GetAllData("PostingData").Where(x => x.lastUpdated > dt && !string.IsNullOrEmpty(x.id)).OrderBy(x => Convert.ToInt32(x.id)).ToArray();
            return new List<string>(res.Select(x => x.id));
        }
    }
}