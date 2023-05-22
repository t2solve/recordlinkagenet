using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data.Transpose
{
    public class TableConverter
    {

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
