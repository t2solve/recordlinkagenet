using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data
{
    public abstract class DataCell : IEqualityComparer<DataCell> //, IComparable<DataCell>
    {
        public uint Id { get; set; } = uint.MaxValue;

        //public virtual string Value { get; set; } = "";

        //private string _value;

        public virtual string Value
        {
            get {               
                throw new NotImplementedException();
            } 
            set
            {
                throw new NotImplementedException();  
            }     
        }

        public bool Equals(DataCell x, DataCell y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode( DataCell obj)
        {
            throw new NotImplementedException();
        }


    }
}
