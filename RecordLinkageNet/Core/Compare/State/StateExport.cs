using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateExport : CompareState
    {

        public StateExport() : base()
        {
            this.Name = "Export";
            this.type = Type.Export;
        }

        public override bool Load()
        {
            //throw new NotImplementedException();
            return true; 
        }

        public override bool Save()
        {
            //throw new NotImplementedException();
            return true; 
        }
    }
    
}
