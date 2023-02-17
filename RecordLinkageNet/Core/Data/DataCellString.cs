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
        public new string Value { get; set; } = ""; 

        public bool Equals(DataCellString x, DataCellString y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] DataCellString obj)
        {
            throw new NotImplementedException();
        }
    }
}
