using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateEvalute : CompareState
    {
        private MatchGroupOrderedList matchGroupOrderedList = null; 
        public StateEvalute() : base()
        {
            this.Name = "Evaluate";
            this.type = Type.Evaluate;
        }

        public MatchGroupOrderedList MatchGroupOrderedList { get => matchGroupOrderedList; set => matchGroupOrderedList = value; }

        public override bool Load()
        {
            return LoadDefaultDataMemeber(out matchGroupOrderedList);
        }

        public override bool Save()
        {
            return SaveDefaultDataMemeber(matchGroupOrderedList);
        }
    }
}