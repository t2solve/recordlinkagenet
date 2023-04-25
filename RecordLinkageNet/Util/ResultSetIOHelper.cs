#define OLDDOTNETCOMPMODE

using Microsoft.ML;
using RecordLinkageNet.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;



namespace RecordLinkageNet.Util
{
    public class ResultSetIOHelper
    {

        public static DataTable ToDataTable(IDataView dataView)
        {
            DataTable dt = null;
            if (dataView != null)
            {
                dt = new DataTable();
                var preview = dataView.Preview();
                dt.Columns.AddRange(preview.Schema.Select(x => new DataColumn(x.Name)).ToArray());
                foreach (var row in preview.RowView)
                {
                    var r = dt.NewRow();
                    foreach (var col in row.Values)
                    {
                        r[col.Key] = col.Value;
                    }
                    dt.Rows.Add(r);

                }
            }
            return dt;
        }

        public static bool WriteAsCsv<T>(IDataView dataView, string filename)
        {
            bool success = false;

            DataTable datTab = ToDataTable(dataView);//ForkDataTable(tableList, tablename);
            if (datTab != null)
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = datTab.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(";", columnNames));

                foreach (DataRow row in datTab.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(";", fields));
                }

                File.WriteAllText(filename, sb.ToString());
            }

            return success;

        }
        public static DataTable ForkDataTable<T>(List<T> list, string tableName)
        {
            DataTable resultTable = new DataTable(tableName);

            //create columns
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                DataColumn column = new DataColumn();
                column.DataType = property.PropertyType;//String.Format("{0}",);
                column.ColumnName = property.Name;
                column.ReadOnly = true;
                resultTable.Columns.Add(column);
            }

            //all rows
            DataRow row;
            foreach (T x in list)
            {
                row = resultTable.NewRow();
                foreach (PropertyInfo property in properties)
                {
                    string name = property.Name;
                    var value = property.GetValue(x);
                    row[name] = value;
                }

                resultTable.Rows.Add(row);
            }

            return resultTable;
        }
#if !OLDDOTNETCOMPMODE

        public static bool WriteResultSetToFolder(string path, ResultSet rs)
        {
            bool success = false;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //Trace.WriteLine("error 28732873 folder " + path + " not existing"); 
                //return success;
            }
            try
            {

                string fullNameIndex = Path.Combine(path, "index.json");
                WriteAsJson(rs.indexList, fullNameIndex);

                string fullNameColNames = Path.Combine(path, "colnames.json");
                WriteAsJson(rs.colNames, fullNameColNames);

                string fullNameData = Path.Combine(path, "data.json");
                WriteAsJson(ConvertDataToJaggedArray(rs), fullNameData);

                success = true;

            }
            catch (Exception e)
            {
                Trace.WriteLine("error 928392839 during save resultset:" + e.ToString());
            }

            return success;
        }


        public static ResultSet ReadResultSetToFolder(string path)
        {
            ResultSet rs = null;

            if (!Directory.Exists(path))
            {
                Trace.WriteLine("error 29389283 path not found ");
                return rs;
            }


            string fullNameIndex = Path.Combine(path, "index.json");
            if (!File.Exists(fullNameIndex))
            {
                Trace.WriteLine("error 23423423 file " + fullNameIndex +
                     "not found ");
                return rs;
            }
            string fullNameColNames = Path.Combine(path, "colnames.json");
            if (!File.Exists(fullNameColNames))
            {
                Trace.WriteLine("error 2352355 file " + fullNameColNames +
                     "not found ");
                return rs;
            }
            string fullNameData = Path.Combine(path, "data.json");
            if (!File.Exists(fullNameData))
            {
                Trace.WriteLine("error 34343434 file " + fullNameData +
                     "not found ");
                return rs;


            }
            List<Tuple<int, int>> indexList = null;
            List<string> colNames = null;
            float[,] data = null;

            var text = File.ReadAllBytes(fullNameIndex);
            var readOnlySpan = new ReadOnlySpan<byte>(text);
            var utf8Reader = new Utf8JsonReader(readOnlySpan);
            indexList = JsonSerializer.Deserialize<List<Tuple<int, int>>>(ref utf8Reader);

            text = File.ReadAllBytes(fullNameColNames);
            readOnlySpan = new ReadOnlySpan<byte>(text);
            utf8Reader = new Utf8JsonReader(readOnlySpan);
            colNames = JsonSerializer.Deserialize<List<string>>(ref utf8Reader);

            text = File.ReadAllBytes(fullNameData);
            readOnlySpan = new ReadOnlySpan<byte>(text);
            utf8Reader = new Utf8JsonReader(readOnlySpan);
            float[][] mydata = JsonSerializer.Deserialize<float[][]>(ref utf8Reader);

            //we need to convert back again //TODO check better ways
            int x = indexList.Count;
            int y = colNames.Count;
            data = new float[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                    data[i, j] = mydata[i][j];
            }

            rs = new ResultSet(indexList, colNames, data);

            return rs;
        }
        private static float[][] ConvertDataToJaggedArray(ResultSet rs)
        {
            int x = rs.indexList.Count;
            int y = rs.colNames.Count;
            float[][] a = new float[x][];

            //we copy every thing
            for (int i = 0; i < x; i++)
            {
                a[i] = new float[y];
                for (int j = 0; j < y; j++)
                    a[i][j] = rs.data[i, j];
            }
            return a;
        }


        private static void WriteAsJson(object o, string fileNameAndPath)
        {
            byte[] bytesToWrite = JsonSerializer.SerializeToUtf8Bytes(o, new JsonSerializerOptions { WriteIndented = false });
            using (FileStream stream = new FileStream(fileNameAndPath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(bytesToWrite, 0, bytesToWrite.Length);
                stream.Close();
            }
        }
#endif
    }

}
