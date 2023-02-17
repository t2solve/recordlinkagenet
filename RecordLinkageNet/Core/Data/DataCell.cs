using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data
{
    public class DataCell : IEqualityComparer<DataCell> //, IComparable<DataCell>
    {
        public uint Id { get; set; } = uint.MaxValue;

        //public byte Value { get; set; }

        //public virtual int CompareTo(DataCell other)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Equals(DataCell x, DataCell y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] DataCell obj)
        {
            throw new NotImplementedException();
        }
    }
}
