using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Score;
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

        public enum CalculationStrategy
        {
            Unknown,
            WeightedConditionSum,
            DecisionTree
        }

        public CalculationStrategy Strategy { get; private set; } = CalculationStrategy.Unknown; 

        //public List<string> ImportantIdList = new List<string>();//TODO remove 

        public ConditionList ConditionList { get; private set; } = null;
        public IndexFeather Index { get; private set; } = null; 
        public NumberTransposeHelper.TransposeModus NumberTransposeModus { get; private set;  } = NumberTransposeHelper.TransposeModus.LINEAR;

        private bool modusDoCompareCalculation = false; 

        //computational 
        public int AmountCPUtoUse { get; private set; } = Environment.ProcessorCount;

        public Configuration AddStrategy(CalculationStrategy strategy)
        {
            if(modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoin"); 
                return null; 
            }
            this.Strategy = strategy;
            return this; 
        }

        public Configuration AddNumberTransposeModus(NumberTransposeHelper.TransposeModus modus)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoin");
                return null;
            }
            this.NumberTransposeModus = modus;
            return this;
        }

        public Configuration SetAmountCPUtoUse(int amount)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoin");
                return null;
            }
            if (amount<0 || amount > Environment.ProcessorCount )
            {
                throw new ArgumentException("error 203923090 wrong cpu amount selected"); 
            }
            AmountCPUtoUse = amount;

            return this; 
        }

        public Configuration AddIndex( IndexFeather index)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoin");
                return null;
            }
            this.Index = index;
            return this; 
        }

        public Configuration AddConditionList(ConditionList list)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoin");
                return null;
            }

            if (list == null)
            {
                Trace.WriteLine("error 29389389 null ref list");
                return null; 
            }
            //clear when existing
            if (ConditionList != null)
            {
                if (ConditionList.GetAmountConditions() > 0)
                    ConditionList.Clear();
            }
            this.ConditionList = list;

            return this; 
        }

        public void EnterDoCompareCalculationModus()
        {
            modusDoCompareCalculation = true; 
        }

        public void ExitDoCompareCalculationModus()
        {
            modusDoCompareCalculation= false;
        }





    }
}
