﻿using RecordLinkageNet.Core.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RecordLinkageNet.Core.Transpose
{
    public class DataTableWriter
    {
        public static bool WriteAsCSV(string file, DataTableFeather data, string sep = ";")
        {
            bool success = false;
            if (data == null)
            {
                Trace.WriteLine("error 29389283 data object null");
                return false;
            }
            if (data.GetAmountRows() == 0)
            {
                Trace.WriteLine("warning 2398983  data object has NO rows");
            }
            try
            {
                string newLine = Environment.NewLine;

                if (!Directory.Exists(Path.GetDirectoryName(file))) Directory.CreateDirectory(Path.GetDirectoryName(file));

                //if (!File.Exists(file)) File.Create(file);

                Encoding ourEncoding = Encoding.GetEncoding("ISO-8859-1");  //is iso 8859-1 
                bool appendFlag = false;
                using (var sw = new StreamWriter(file, appendFlag, ourEncoding))
                {
                    //we write the column header 
                    //this is the header row with display name
                    sw.Write("\"" + string.Join("\"" + sep + "\"", data.GetColumnNames().ToArray()) + "\"" + newLine);
                    int rowAmount = data.GetAmountRows();
                    for (int i = 0; i < rowAmount; i++)
                    {
                        DataRow row = data.GetRow(i);
                        if (row == null)
                        {
                            Trace.WriteLine("error 29839389 during write row with index: " + i + " is null, row will be ignored");
                            throw new NullReferenceException("error 29839389 row is null in DataTableFeather");
                        }
                        else
                        {
                            StringBuilder rowBuilder = new StringBuilder();
                            foreach (DataCell dataCell in row.Data.Values)
                            {
                                if (dataCell == null)
                                {
                                    Trace.WriteLine("warning 23425 datacell is null, will be ignored");
                                    rowBuilder.Append(String.Empty);
                                }
                                else
                                    rowBuilder.Append(dataCell.Value);
                                rowBuilder.Append(sep);
                            }
                            rowBuilder.Append(newLine);
                            //this acts as datacolumn
                            sw.Write(rowBuilder.ToString());
                        }
                    }
                    success = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 20983928398 during write DataTable to csv: " + e.ToString());
                success = false;
            }
            return success;
        }

    }
}
