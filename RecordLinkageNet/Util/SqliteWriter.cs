using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Util
{
    public class SqliteWriter
    {
        public static bool WriteObjectListToSqliteStringBulkFile<T>(List<T> objectList, string tablename, string fileName, bool removeIfExists = false)
        {
            //TODO check allowed table names
            //TODO check file name exist and so on 
            //TODO implement ovveride 
            if(removeIfExists)
            {
               if(File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            if(objectList == null)
            {
                Trace.WriteLine("error 238293898 objectList is null object");
                return false; 
            }
            if(objectList.Count == 0)
            {
                Trace.WriteLine("error 2354235 empty objectList");
                return false;
            }
            try
            {
                using (var conn = new SqliteConnection("Data Source=" + fileName))
                {
                    conn.Open();
                    StringBuilder createBuilder = new StringBuilder();
                    createBuilder.Append("CREATE TABLE IF NOT EXISTS ");
                    createBuilder.Append(tablename);
                    createBuilder.Append("(");
                    //createBuilder.Append(" (Id INTEGER PRIMARY KEY AUTOINCREMENT, ");
                    //we add all properties 
                    foreach (var item in objectList.First().GetType().GetProperties())
                    {
                        createBuilder.Append(item.Name);
                        createBuilder.Append(" TEXT");
                        createBuilder.Append(",");
                    }
                    //remove last 
                    createBuilder.Remove(createBuilder.Length - 1, 1);
                    createBuilder.Append(")");
                    //do create
                    string createString = createBuilder.ToString();
                    using (var cmd = new SqliteCommand(createString, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    //do bulk insert 
                    //  https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/bulk-insert
                    using (var transaction = conn.BeginTransaction())
                    {
                        var command = conn.CreateCommand();
                        StringBuilder insertCommandBuilder = new StringBuilder();
                        insertCommandBuilder.Append("INSERT INTO ");
                        insertCommandBuilder.Append(tablename);
                        insertCommandBuilder.Append(" VALUES (");
                        foreach (var item in objectList.First().GetType().GetProperties())
                        {
                            insertCommandBuilder.Append("$");
                            insertCommandBuilder.Append(item.Name);
                            insertCommandBuilder.Append(",");

                        }
                        insertCommandBuilder.Remove(insertCommandBuilder.Length - 1, 1);
                        insertCommandBuilder.Append(")");
                        string insertString = insertCommandBuilder.ToString();

                        command.CommandText = insertCommandBuilder.ToString();

                        //we add all parameter
                        Dictionary<string, SqliteParameter> paraDictio = new Dictionary<string, SqliteParameter>();

                        foreach (var item in objectList.First().GetType().GetProperties())
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = "$" + item.Name;
                            command.Parameters.Add(parameter);
                            paraDictio.Add(item.Name, parameter);
                        }

                        foreach (var obj in objectList)
                        {
                            foreach (var item in obj.GetType().GetProperties())
                            {
                                //get paramter 
                                var para = paraDictio[item.Name];
                                para.Value = item.GetValue(obj).ToString();
                            }
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }//end transaction
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 239892389 during write test db to " + fileName);
                Trace.WriteLine("error 239892389  " + e.ToString());
                return false; 
            }

            return true; 
        }
    }
}
