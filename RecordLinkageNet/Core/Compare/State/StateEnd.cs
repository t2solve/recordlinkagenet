using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateEnd : CompareState
    {

        public StateEnd() : base()
        {
            this.Name = "End";
            this.type = Type.End;
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
