using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class which defines what and how to compare datacolumns
    /// </summary>
    [DataContract(Name = "Condition", Namespace = "DataContracts")]
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
        [DataMember(Name = "Mode")]
        public CompareType Mode { get; set; } = CompareType.Unknown;
        [DataMember(Name = "NameColA")]
        public string NameColA { get; set; } = null;
        [DataMember(Name = "NameColB")]
        public string NameColB { get; set; } = null;
        //TODO: min max threshold
        //public float threshold = -1.0f; //default Non ? 
        [DataMember(Name = "ScoreWeight")]
        public float ScoreWeight { get; set; } = 1.0f; //default neutral one
        [DataMember(Name = "MyStringMethod")]
        public StringMethod MyStringMethod { get; set; } = StringMethod.Unknown;
        [DataMember(Name = "NameColNewLabel")]
        public string NameColNewLabel { get; set; } = null;

        public byte ConditionIndex { get; set; } = 0; 

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
            //TODO fix, this doesnt make sense here ??
            //is because of SortByScoreWeight

            if (other == null) return false;
            return (this.ScoreWeight.Equals(other.ScoreWeight));
        }

        public int CompareTo(Condition other)
        {
            //TODO fix, this doesnt make sense here ??
            //is because of SortByScoreWeight
            if (ScoreWeight == -1.0f)
                return 1;
            else
                return other.ScoreWeight.CompareTo(this.ScoreWeight);
        }
    }
}
