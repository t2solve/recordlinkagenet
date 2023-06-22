using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core.Compare.State
{

    public class StateProcess : CompareState
    {
        private MatchCandidateList listOfMachtes = null; 

        public StateProcess() : base()
        {
            this.Name = "Process";
            this.type = Type.Process;
        }

        public MatchCandidateList ListOfMachtes { get => listOfMachtes; set => listOfMachtes = value; }

        public override bool Load()
        {
            return LoadDefaultDataMemeber(out listOfMachtes);
        }

        public override bool Save()
        {        
            return SaveDefaultDataMemeber(listOfMachtes); ; 
        }
    }
}
