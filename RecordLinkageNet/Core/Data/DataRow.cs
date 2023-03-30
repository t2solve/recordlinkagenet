using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data
{
    public class DataRow
    {
        private DataTableFeather parent = null; 
        public Dictionary<string, DataCell> Data = new Dictionary<string, DataCell>();
        
        public DataRow(DataTableFeather dataTableFeatherParent)
        {
            parent = dataTableFeatherParent;
        }



    }
}
