using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data
{
    public class DataCellString : DataCell, IEqualityComparer<DataCellString>
    {
        //TODO refactor eveything is a string here
        private string myvalue = "";
        public override string Value 
        { 
            get
            {
                return myvalue;
            }
            set 
            {
                myvalue = value;
            } 
        } 

        public bool Equals(DataCellString x, DataCellString y)
        {
            if ( ReferenceEquals(x, y)) return true;

            if(x !=null && y !=null)
            {
                if (x.GetHashCode() == y.GetHashCode())
                    return true; 
            }

            return false; 
        }

        public int GetHashCode( DataCellString obj)
        {
           return obj.myvalue.GetHashCode();
        }
    }
}
