using Microsoft.Data.Sqlite;
using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RecordLinkageNet.Util
{
    public static class SqliteReader
    {
        private static int DoACountStatement(SqliteConnection conn, string stm)
        {
            int retVal = -1;
            try
            {
                using (var cmd = new SqliteCommand(stm, conn))
                {
                    retVal = Convert.ToInt32(cmd.ExecuteScalar());
                    return retVal;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 324349584598 during perform stm: " + stm + "\n" + e.ToString());
                return retVal;
            }
            //return retVal; 
        }

        private static List<string> GetColumnList(SqliteConnection conn, string tableName)
        {
            List<string> columnNames = new List<string>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("PRAGMA table_info(");
            cmdText.Append(tableName);
            cmdText.Append(")");
            using (var cmd = new SqliteCommand(cmdText.ToString(), conn))
            {
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //column 1 from the result contains the column names
                    string columnName = dr.GetValue(1).ToString();
                    columnNames.Add(columnName);
                }

                dr.Close();
            }
            return columnNames;
        }

        private static string GetFieldFromReader(SqliteDataReader reader, string name)
        {
            string ret = string.Empty;
            if (reader[name] != null)
                ret = Convert.ToString(reader[name]);
            return ret;
        }

        public static DataTableFeather ReadTableFromSqliteFile(string fileName, string tablename)
        {
            DataTableFeather result = null;

            if (!File.Exists(fileName))
            {
                Trace.WriteLine("error 2398989 file " +
                     fileName + " not found");
                return result;
            }

            //try to open and check what is included 
            try
            {
                using (var conn = new SqliteConnection("Data Source=" + fileName))
                {
                    conn.Open();
                    //do a check table exists statememt
                    StringBuilder checkForTableName = new StringBuilder();
                    checkForTableName.Append("SELECT COUNT(*) name FROM sqlite_master WHERE type = 'table' AND name ='");
                    checkForTableName.Append(tablename);
                    checkForTableName.Append("'");
                    int tableExistsCounter = DoACountStatement(conn, checkForTableName.ToString());
                    if (tableExistsCounter != 1)
                    {
                        Trace.WriteLine("error 1239892849 table not found in sqlite db: " + fileName);
                        return result;
                    }
                    //we do a count how many rows 
                    StringBuilder checkAmountRows = new StringBuilder();
                    checkAmountRows.Append("SELECT COUNT(*) FROM ");
                    checkAmountRows.Append(tablename);
                    int amountRows = DoACountStatement(conn, checkAmountRows.ToString());
                    if (amountRows > 0)
                    {
                        //we get the column Names 
                        List<string> columNames = GetColumnList(conn, tablename);
                        //add all columns
                        if (columNames.Count > 0)
                        {
                            result = new DataTableFeather();

                            foreach (string colName in columNames)
                            {
                                DataColumn datCol = new DataColumn(amountRows, "".GetType());
                                datCol.Name = colName;
                                result.AddColumn(colName, datCol);
                            }
                            //add all rows batched
                            string cmdGetRows = "SELECT * FROM " + tablename;
                            using (var cmd = new SqliteCommand(cmdGetRows.ToString(), conn))
                            {
                                var reader = cmd.ExecuteReader();

                                while (reader.Read())
                                {
                                    foreach (string colName in columNames)
                                    {
                                        DataColumn col = result.GetColumnByName(colName);
                                        if (col != null)
                                        {
                                            var cell = new DataCellString();
                                            cell.Value = GetFieldFromReader(reader, colName);
                                            if (!col.AppendCell(cell))
                                            {
                                                Trace.WriteLine("warning 2349823989898 ");
                                            }
                                        }
                                    }

                                }
                                reader.Close();

                            }
                        }
                        else
                        {
                            Trace.WriteLine("warning 566752342 no columns in table: " + tablename);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("warning 238928398 no rows in table: " + tablename);
                        return result;
                    }
                    conn.Close();
                    conn.Dispose();
                }//end using
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 23989898 89787 during open sqlite db: " + e.ToString());
                return result;
            }


            return result;
        }
    }
}
