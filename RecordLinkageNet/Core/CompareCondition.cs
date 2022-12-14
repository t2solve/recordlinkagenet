using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    /// <summary>
    /// Class which defines what and how to compare datacolumns
    /// </summary>
    public class CompareCondition
    {
        public enum StringMethod
        {
            Exact,
            HammingDistance,
            DamerauLevenshteinDistance,
            JaroDistance,
            JaroWinklerSimilarity,
            ShannonEntropyDistance,
            MyCustomizedDistance, 
            Unknown
        }
        public enum CompareType
        {
            Exact,
            String,
            Numeric,
            Unknown
        }

        public CompareType Mode { get; set; } = CompareType.Unknown;
        public string NameColA { get; set; } = null;
        public string NameColB { get; set; } = null;
        public float threshold = -1.0f; //default Non ? 
        //TODO: min max threshold

        public StringMethod MyStringMethod { get; set; } = StringMethod.Unknown;
        //foo
        public string NameColNewLabel { get; set; } = null;

        //an exlusive list we have implemted 
        public void SetNewColName()
        {
            if(String.IsNullOrEmpty(NameColNewLabel))
            {
                //we set by nameA
                NameColNewLabel = NameColA; 
            }
        }
    }
}
