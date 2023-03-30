using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace RecordLinkageNet.Core.Data
{
    public class DataTableFeather
    {
        //private List<DataColumn> columns = new List<DataColumn>();
        private Dictionary<string, DataColumn> namedColumnsMap = new Dictionary<string, DataColumn>();
        private Dictionary<int, DataColumn> indexedColumnsMap = new Dictionary<int, DataColumn>();
        private int columnIndexMax = 0;
        private int rowIndexMax = -1;

        private bool CheckColumnIndexIsInRange(int colIndex)
        {
            if (colIndex < 0 || colIndex >= columnIndexMax)
            {
                Trace.WriteLine("error 2938928398 wrong col index :" + colIndex);
                throw new IndexOutOfRangeException("GetColumnByIndex wrong index");
                return false;
            }
            return true;
        }

        private bool CheckRowIndexIsInRange(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex > rowIndexMax)
            {
                Trace.WriteLine("error 1242343546234 wrong row index :" + rowIndex);
                throw new IndexOutOfRangeException("CheckRowIndexIsInRange wrong index");
                return false;
            }
            return true;
        }
        public DataRow GetRow(int rowIndex)
        {
            DataRow row = new DataRow(this);

            //Dictionary<string, DataCell> row = new Dictionary<string, DataCell>();
            try
            {

                if (CheckRowIndexIsInRange(rowIndex))
                {
                    foreach (string name in namedColumnsMap.Keys)
                    {
                        //get column 
                        DataColumn col = namedColumnsMap[name];
                        row.Data[name] = col.At(rowIndex);
                    }
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 203902390 during GetRow: " + e.ToString());
                return null;
            }
            return row;
        }
        public DataColumn GetColumnByName(string name)
        {
            if (namedColumnsMap.TryGetValue(name, out DataColumn column))
                return column;

            return null;
        }
        public DataColumn GetColumnByIndex(int index)
        {
            if (!CheckColumnIndexIsInRange(index))
            {
                return null;
            }
            DataColumn col = null;
            if (indexedColumnsMap.TryGetValue(index, out col))
            {
                return col;
            }
            else
                Trace.WriteLine("error 2324252523 during indexexing data column");

            return null;
        }

        public int GetAmountRows()
        {
            return rowIndexMax;
        }
        public int GetAmountColumns()
        {
            return namedColumnsMap.Keys.Count();
        }

        public List<string> GetColumnNames()
        {
            return namedColumnsMap.Keys.ToList();
        }

        private bool CheckRowAmountViolationIsPresent(int amountRows)
        {
            if (this.rowIndexMax == -1)
            {
                this.rowIndexMax = amountRows;
            }
            else if (this.rowIndexMax != amountRows)
            {
                Trace.WriteLine("warning 29389832 misshaped rowSizes, old rowAmount is: " + this.rowIndexMax);
                throw new ArrayTypeMismatchException("wrong size");
                return true;
            }//wed add all columns
            return false;
        }

        public DataTableFeather AddDataClassAsColumns(object data, int amountRows)
        {
            //TODO type check data
            if (data == null)
                return null;
            if (CheckRowAmountViolationIsPresent(amountRows))
                Trace.WriteLine("warning 29389832 misshaped 2D datatable");

            PropertyInfo[] propertyInfoArr = data.GetType().GetProperties();
            foreach (PropertyInfo i in propertyInfoArr)
            {
                string name = i.Name;
                Type type = i.PropertyType;
                //Trace.WriteLine("debug:  "+ name+ " "+ type);

                DataColumn col = new DataColumn(amountRows, type);
                col.Name = name;
                col.DataType = type;

                this.AddColumn(col.Name, col);
            }
            return this;
        }

        public DataTableFeather AddColumn(string name, DataColumn c)
        {
            //TODO check if the names exists !! 

            namedColumnsMap.Add(name, c);
            c.ParentTable = this;

            indexedColumnsMap.Add(columnIndexMax, c);
            columnIndexMax += 1;

            //we update the amount of rows
            rowIndexMax = c.Rows.Length;

            if (CheckRowAmountViolationIsPresent(rowIndexMax))
                Trace.WriteLine("error 2983298398");

            return this;
        }

        public DataTableFeather AddRow(object a)
        {
            //TODO rename 
            //TODO check type parameter fits !! 

            //we try to find names and add them 
            PropertyInfo[] propertyInfoArr = a.GetType().GetProperties();
            foreach (PropertyInfo i in propertyInfoArr)
            {
                string colName = i.Name;
                Type type = i.PropertyType;
                DataColumn col = null;
                if (namedColumnsMap.TryGetValue(colName, out col))
                {

                    if (type == typeof(string))
                    {
                        DataCellString cell = new DataCellString();
                        cell.Value = (string)i.GetValue(a);
                        col.AppendCell(cell);

                    }
                }
                else Trace.WriteLine("error 9289289 no column named " + colName + " found, please add");

            }
            return this;
        }

        public DataTableFeather AddRow(int rowIndex, DataRow row)
        {
            if (rowIndex>=0&& rowIndex < rowIndexMax)
            {
                foreach(var colName in row.Data.Keys)
                {
                    DataColumn col = null; 
                    if(namedColumnsMap.TryGetValue(colName, out col))
                    {
                        //TODO we do a type check
                        //we get the cell 
                         DataCell cell = col.At(rowIndex);
                        //we replace
                        if(cell!=null)
                        {
                            cell.Value = row.Data[colName].Value;
                        }
                        else Trace.WriteLine("error 342349898398 cell not found in table at col: " + colName + " with rowIndex: " + rowIndex);

                    }
                    else Trace.WriteLine("error 23253459 col name not found int table :" + colName);

                }
            }
            else Trace.WriteLine("error 23536546 wrong index " + rowIndex );

            return this; 
        }
    }
}
