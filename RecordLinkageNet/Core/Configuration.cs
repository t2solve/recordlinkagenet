using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexFeather = RecordLinkageNet.Core.Compare.IndexFeather;

namespace RecordLinkageNet.Core
{
    public class Configuration
    {

        public ConditionList ConditionList { get; private set; } = null;
        public IndexFeather Index { get; private set; } = null; 
        public NumberTransposeHelper.TransposeModus NumberTransposeModus { get; set;  } = NumberTransposeHelper.TransposeModus.LINEAR;
        
        //computational 
        public int AmountCPUtoUse { get; set; } = Environment.ProcessorCount;

        public ScoreProducer ScoreProducer { get; private set; } = null;

        public Configuration AddIndex( IndexFeather index)
        {
            this.Index = index;
            return this; 
        }

        public Configuration AddConditionList(ConditionList list)
        {
            this.ConditionList = list;

            //we crerate our default score producer
            this.ScoreProducer = new ScoreProducer(this.ConditionList, this.NumberTransposeModus);

            return this; 
        }

        public Configuration SetScoreProducer( ScoreProducer scoreProducer)
        {
            this.ScoreProducer = scoreProducer;

            //we crerate our score producer

            return this;
        }


    }
}
