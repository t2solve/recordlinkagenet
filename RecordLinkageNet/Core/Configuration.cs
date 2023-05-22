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
    //singelon class for config
    public sealed class Configuration
    {
        //TODO implement a computation modus, to not change config while calc
        
        
        //singeleton pattern
        //accoring to https://csharpindepth.com/Articles/Singleton
        private static readonly Lazy<Configuration> lazy =
        new Lazy<Configuration>(() => new Configuration());

        public static Configuration Instance { get { return lazy.Value; } }

        private Configuration()
        {
        }


        public List<string> ImportantIdList = new List<string>();

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
            if (list == null)
            {
                Trace.WriteLine("error 29389389 null ref list"); 
            }
            //clear when existing
            if (ConditionList != null)
            {
                if (ConditionList.GetAmountConditions() > 0)
                    ConditionList.Clear();
            }

        
            this.ConditionList = list;

            //TODO: remove here
            //we crerate our default score producer
            this.ScoreProducer = new ScoreProducer(this);

            return this; 
        }

        //public Configuration SetScoreProducer( ScoreProducer scoreProducer)
        //{
        //    this.ScoreProducer = scoreProducer;

        //    //we crerate our score producer

        //    return this;
        //}


    }
}
