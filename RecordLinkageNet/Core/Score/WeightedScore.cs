using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScore : IScore
    {
        //                 index, value
        private Dictionary<byte, byte> matchScorePartByConditionIndex = new Dictionary<byte, byte>();
        private float scoreAbsTotal = -1.0f;
        //private bool scoreIsComplete = false;
        private IScore.AcceptanceLevel acceptanceLvl = IScore.AcceptanceLevel.Unknown;
        private MatchCandidate mCandidate = null;
        private List<byte> conIndexListWeNeedToAddScoreParts = new List<byte>();
        public WeightedScore(MatchCandidate m)
        {
            //link each other
            this.mCandidate = m;
            m.SetScore(this);

            //we fill our todo list
            foreach (Condition con in Configuration.Instance.ConditionList)
            {
                conIndexListWeNeedToAddScoreParts.Add(con.ConditionIndex);
            }

        }

        public Dictionary<byte, byte> GetScoreParts()
        {
            return matchScorePartByConditionIndex;
        }

        //public float Calculate(MatchCandidate x)
        //{
        //    if (matchScorePartByConditionIndex.Count() == 0)
        //    {
        //        Trace.WriteLine("error 394898349 empty list");
        //        return -1.0f;
        //    }
        //    throw new NotImplementedException();
        //}

        public bool IsScoreComplete()
        {
            return (matchScorePartByConditionIndex.Count() == Configuration.Instance.ConditionList.Count());
        }
        public int GetAmountScorePartsWeAdded()
        {
            return matchScorePartByConditionIndex.Count(); 
        }

        public List<byte> GetIndexListOfMissingParts()
        { 
            return conIndexListWeNeedToAddScoreParts; 
        }

        public float AddScorePart(byte index, float scorePartCompareWeigthed, byte scoreTransformed)
        {
            if(!matchScorePartByConditionIndex.ContainsKey(index))
            {
                //TODO maybe change where to find 
                
                if (matchScorePartByConditionIndex.Count() == 0) //init 
                {
                    scoreAbsTotal = 0.0f;
                }

                scoreAbsTotal += scorePartCompareWeigthed;

                //we remember it 
                matchScorePartByConditionIndex.Add(index, scoreTransformed);

                conIndexListWeNeedToAddScoreParts.Remove(index);

                ////we check if we are full
                //if (matchScorePartByConditionIndex.Count() == Configuration.Instance.ConditionList.Count())
                //    scoreIsComplete = true; 

            }
            else
            {
                Trace.WriteLine("warning 293829839 double added key");
                throw new ArgumentException("error 293829839"); 
            }

            return scoreAbsTotal; 
        }
        public int CompareTo(IScore other)
        {
            if (other == null) return -1;
            return (int) (other.GetScoreValue() - this.GetScoreValue());

            //if (other is WeightedScore)
            //{
            //    if (scoreTotal == -1.0f)
            //        return 1;
            //    else
            //        return this.Equals(other);
            //}
            ////else throw new ArgumentException("error 237872837 must be same class type");


            //throw new NotImplementedException();
        }

        public bool Equals(IScore other)
        {
            if (other == null) return false;

            if (other is WeightedScore)
            {
                WeightedScore wOther = (WeightedScore) other;
                bool test1 = this.GetScoreValue() == other.GetScoreValue();
                if (!test1)
                    return false;

                foreach(var a in this.matchScorePartByConditionIndex)
                {
                    if (wOther.GetScoreParts().ContainsKey(a.Key))
                    {
                        if (wOther.GetScoreParts()[a.Key] != this.matchScorePartByConditionIndex[a.Key])
                            return false;
                    }
                    else
                    { 
                        return false;
                    }
                }
                return true; 
            }
            return false;
        }

        public IScore.AcceptanceLevel GetAcceptanceLevel()
        {
            return acceptanceLvl;
        }

        public float GetScoreValue()
        {

            if (IsScoreComplete())
                return scoreAbsTotal;
            else
                return -1.0f;
        }
    }
}