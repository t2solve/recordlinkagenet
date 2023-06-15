﻿using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IndexFeather = RecordLinkageNet.Core.Compare.IndexFeather;

namespace RecordLinkageNet.Core
{
    //singelon class for config
    [DataContract(Name = "Configuration", Namespace = "RecordLinkageNet")]
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

        //data parts 
        [DataMember(Name = "Strategy")]
        public CalculationStrategy Strategy { get; private set; } = CalculationStrategy.WeightedConditionSum;
        [DataMember(Name = "ConditionList")]
        public ConditionList ConditionList { get; private set; } = null;
        [IgnoreDataMember]
        public IndexFeather Index { get; private set; } = null;
        [DataMember(Name = "NumberTransposeModus")]
        public NumberTransposeHelper.TransposeModus NumberTransposeModus { get; private set;  } = NumberTransposeHelper.TransposeModus.LINEAR;

        //filter parameter 
        [DataMember(Name = "FilterParameterThresholdRelativMinScore")]
        public float FilterParameterThresholdRelativMinScore { get; private set; } = 0.7f;//used by FilterRelativMinScore

        public float FilterParameterThresholdRelativMinDistance { get; private set; } = 0.2f;//used by FilterRelativMinScore

        //computational 
        public int AmountCPUtoUse { get; private set; } = Environment.ProcessorCount;

        //functional 
        private bool modusDoCompareCalculation = false;

        public bool IsValide()
        {
            bool success = false;
            if (NumberTransposeModus == NumberTransposeHelper.TransposeModus.UNKNOWN)
            {
                Trace.WriteLine("warning 235235 non valide configuration please AddNumberTransposeModus");
                return false;
            }
            if (Strategy == CalculationStrategy.Unknown)
            {
                Trace.WriteLine("warning 298398 non valide configuration please AddStrategy");
                return false; 
            }
            if(ConditionList == null)
            {
                Trace.WriteLine("warning 234235 non valide configuration please AddConditionList");
                return false;
            }
            if (ConditionList.Count() == 0)
            {
                Trace.WriteLine("warning 236 non valide configuration please add a condition to the list");
                return false;
            }
            if (Index == null)
            {
                Trace.WriteLine("warning 2455 non valide configuration please AddIndex");
                return false;
            }

            if (Index.dataTabA == null)
            {
                Trace.WriteLine("warning 5635 non valide configuration please add DatTable as dataTabA to Index");
                return false;
            }
            if (Index.dataTabB == null)
            {
                Trace.WriteLine("warning 5635 non valide configuration please add DatTable as dataTabB to Index");
                return false;
            }
            if (Index.dataTabA.GetAmountColumns() == 0|| Index.dataTabA.GetAmountColumns()==0)
            {
                Trace.WriteLine("warning 6436 non valide configuration please add Columns to DataTable in Index");
                return false;
            }
            if (Index.dataTabA.GetAmountRows() == 0 || Index.dataTabA.GetAmountRows() == 0)
            {
                Trace.WriteLine("warning 6436 non valide configuration please add Rows to DataTable in Index");
                return false;
            }

            success = true;
            return success; 
        }

        public void Reset()
        {
            Index = null; 
            ConditionList = null;
        }

        public Configuration AddStrategy(CalculationStrategy strategy)
        {
            if(modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoing"); 
                return null; 
            }
            this.Strategy = strategy;
            return this; 
        }


        public Configuration AddNumberTransposeModus(NumberTransposeHelper.TransposeModus modus)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoing");
                return null;
            }
            this.NumberTransposeModus = modus;
            return this;
        }

        public Configuration SetFilterParameterThresholdRelativMinScore(float minScore)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2353646 change will be ignored, computation is ongoing");
                return null;
            }
          
            this.FilterParameterThresholdRelativMinScore = minScore;

            return this;
        }

        public Configuration SetFilterParameterThresholdRelativMinDistance(float minDistance)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 234253 change will be ignored, computation is ongoing");
                return null;
            }

            this.FilterParameterThresholdRelativMinDistance = minDistance;

            return this;
        }
        

        public Configuration SetAmountCPUtoUse(int amount)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoing");
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
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoing");
                return null;
            }
            this.Index = index;
            return this; 
        }

        public Configuration AddConditionList(ConditionList list)
        {
            if (modusDoCompareCalculation)
            {
                Trace.WriteLine("warning 2938938 change will be ignored, computation is ongoing");
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
