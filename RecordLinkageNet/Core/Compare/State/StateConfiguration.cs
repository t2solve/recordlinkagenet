using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateConfiguration : CompareState
    {

        public StateConfiguration():base()
        {
            this.Name = "LoadData";
        }
        public override bool Load()
        {
            throw new NotImplementedException();
        }

        public override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
