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
        private float scoreTotal = -1.0f;
        private bool scoreIsComplete = false;
        private IScore.AcceptanceLevel acceptanceLvl = IScore.AcceptanceLevel.Unknown;
        private MatchCandidate mCandidate = null;

        public WeightedScore(MatchCandidate m)
        {
            this.mCandidate = m;
        }
        public float Calculate(MatchCandidate x)
        {
            if (matchScorePartByConditionIndex.Count() == 0)
            {
                Trace.WriteLine("error 394898349 empty list");
                return -1.0f;
            }
            throw new NotImplementedException();
        }
        public float GetTotalScoreValue()
        {
            return scoreTotal;
        }
        public bool IsCompleteBuilded()
        {
            return scoreIsComplete;
        }
        public int GetAmountScoreParts()
        {
            return matchScorePartByConditionIndex.Count(); 
        }
        public float AddScorePart(byte index, float scorePart)
        {
            if(!matchScorePartByConditionIndex.ContainsKey(index))
            {
                //TODO maybe change where to find 
                byte scoreTransformed = Configuration.Instance.ScoreProducer.TransposeComparisonResult(scorePart);
                //we calc 
                if (matchScorePartByConditionIndex.Count() == 0)
                {
                    scoreTotal = 0.0f;
                }
                //we add
                scoreTotal += scorePart;

                //we remember it 
                matchScorePartByConditionIndex.Add(index, scoreTransformed);

                //we check if we are full
                if (matchScorePartByConditionIndex.Count() == Configuration.Instance.ConditionList.Count())
                    scoreIsComplete = true; 

            }
            else
            {
                Trace.WriteLine("warning 293829839 double added key");
                throw new ArgumentException("error 293829839"); 
            }

            return scoreTotal; 
        }
        public int CompareTo(IScore other)
        {
            //if (other is WeightedScoreSum)
            //{
            //    if (scoreTotal == -1.0f)
            //        return 1;
            //    else
            //        return this.Equals(other);
            //}
            //else throw new ArgumentException("error 237872837 must be same class type");

            throw new NotImplementedException();
        }

        public bool Equals(IScore other)
        {
            throw new NotImplementedException();

            //    if (other == null) return false;
            //    bool test1 = this.scoreTotal == other.ScoreTotal);
            //    bool test2 = this.Pair.aIdx == other.Pair.aIdx;
            //    bool test3 = this.Pair.bIdx == other.Pair.bIdx;

            //    return (test1 && test2 && test3);
            //}
        }
    }
}