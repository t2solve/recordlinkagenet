using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScoreSum : IScore
    {
        //                 index, value
        private Dictionary<byte, byte> matchScoreColumnByConditionIndex = new Dictionary<byte, byte>();
        private float scoreTotal = -1.0f;
        private float scoreTempProgress = -1.0f; 
        private IScore.AcceptanceLevel acceptanceLvl = IScore.AcceptanceLevel.Unknown;
        private MatchCandidate mCandidate = null; 

        public WeightedScoreSum(MatchCandidate m)
        {
            this.mCandidate = m; 
        }
        public float Calculate(MatchCandidate x)
        {
            throw new NotImplementedException();
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
