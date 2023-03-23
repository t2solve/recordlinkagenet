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
        private int indexCounter = 0; 
        private int amountRows = -1; 

        public DataColumn GetColumnByName(string name)
        {
            if( namedColumnsMap.TryGetValue(name, out DataColumn column))
                return column;

            return null; 
        }
        public DataColumn GetColumnByIndex(int index)
        {
            if ( index<0|| index>= indexCounter)
            {
                Trace.WriteLine("error 2938928398 wrong index ");
                throw new IndexOutOfRangeException("GetColumnByIndex wrong index");
                return null; 
            }
            DataColumn col = null; 
            if( indexedColumnsMap.TryGetValue(index, out col))
            {
                return col; 
            }
            else
                Trace.WriteLine("error 2324252523 during indexexing data column");
            
            return null;
        }

        public int GetAmountRows()
        {
            return amountRows;
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
            if (this.amountRows == -1)
            {
                this.amountRows = amountRows;
            }
            else if (this.amountRows != amountRows)
            {
                Trace.WriteLine("warning 29389832 misshaped rowSizes, old rowAmount is: " + this.amountRows);
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
                Trace.WriteLine("warning 29389832 misshaped ");
            
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

            indexedColumnsMap.Add(indexCounter, c);
            indexCounter += 1;

            //we update the amount of rows
            amountRows = c.Rows.Length;

            if (CheckRowAmountViolationIsPresent(amountRows))
                Trace.WriteLine("error 2983298398"); 

            return this; 
        }

        public DataTableFeather AddRow(object a)
        {
            //we try to find names and add them 
            PropertyInfo[] propertyInfoArr = a.GetType().GetProperties();
            foreach (PropertyInfo i in propertyInfoArr)
            {
                string colName = i.Name;
                Type type = i.PropertyType;
                DataColumn col = null;
                if (namedColumnsMap.TryGetValue(colName, out col))
                {
                   
                    if (type ==typeof(string))
                    {
                        DataCellString cell = new DataCellString();
                        cell.Value = (string)  i.GetValue(a);
                        col.AddCell(cell); 

                    }
                }
                else Trace.WriteLine("error 9289289 no column named " + colName + " found, please add"); 

            }
                return this; 
        }
    }
}
