﻿using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data.Transpose
{
    public class TableConverter
    {

        public static System.Data.DataTable ForkSystemDataTable<T>(List<T> list, string tableName)
        {
            System.Data.DataTable resultTable = new System.Data.DataTable(tableName);

            //create columns
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                System.Data.DataColumn column = new System.Data.DataColumn();
                column.DataType = property.PropertyType;//String.Format("{0}",);
                column.ColumnName = property.Name;
                column.ReadOnly = true;
                resultTable.Columns.Add(column);
            }

            //all rows
            System.Data.DataRow row;
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
        public static DataTableFeather CreateTableFeatherFromDataObjectList<T>(List<T> list)
        {
            if (list == null)
            {
                System.Diagnostics.Trace.WriteLine("warning 2323898989  list is null");
                return null;
            }

            int amountRows = list.Count;
            if (amountRows == 0)
            {
                System.Diagnostics.Trace.WriteLine("warning 235235 list is empty");
                return null;
            }

            DataTableFeather tab = new DataTableFeather();
            T headElement = list.First();
            tab.AddDataClassAsColumns(headElement, amountRows);
            foreach (T p in list) //we add all rows 
            {
                tab.AddRow(p);
            }
            //TODO check some things

            return tab;
        }


    }
}
