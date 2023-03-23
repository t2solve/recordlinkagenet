using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class which defines what and how to compare datacolumns
    /// </summary>
    [Serializable]
    public class Condition : IEquatable<Condition> , IComparable<Condition>
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
        //TODO: min max threshold
        //public float threshold = -1.0f; //default Non ? 
        public float ScoreWeight { get; set; } = 1.0f; //default neutral one

        public StringMethod MyStringMethod { get; set; } = StringMethod.Unknown;
        public string NameColNewLabel { get; set; } = null;

        //an exlusive list we have implemted 
        public void SetNewColName()//TODO call during set NameColA 
        {
            if (string.IsNullOrEmpty(NameColNewLabel))
            {
                //we set by nameA
                NameColNewLabel = NameColA;
            }
        }

        public bool Equals(Condition other)
        {
            if (other == null) return false;
            return (this.ScoreWeight.Equals(other.ScoreWeight));
        }

        public int CompareTo(Condition other)
        {
            if (ScoreWeight == -1.0f)
                return 1;
            else
                return other.ScoreWeight.CompareTo(this.ScoreWeight);
        }
    }
}
