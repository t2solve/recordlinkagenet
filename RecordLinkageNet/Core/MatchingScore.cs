using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class MatchingScore : IEquatable<MatchingScore>, IComparable<MatchingScore>
    {
        public IndexPair Pair = new IndexPair(uint.MaxValue, uint.MaxValue);
        public float ScoreTotal = -1.0f;
        public Dictionary<string, byte> MatchScoreColumnByName = null;
        private ScoreProducer scoProducer = null;
        public MatchingScore(ScoreProducer scoreProducer, IndexPair pair)
        {
            this.Pair = pair;
            this.scoProducer = scoreProducer;
            MatchScoreColumnByName = new Dictionary<string, byte>();
        }

        public void AddScore(string columnName, float score)
        {
            //we do a conversion

            MatchScoreColumnByName.Add(columnName, scoProducer.TransposeComparisonResult(score));
        }

        public int CompareTo(MatchingScore other)
        {
            if (ScoreTotal == -1.0f)
                return 1;
            else
                return other.ScoreTotal.CompareTo(this.ScoreTotal);
        }

        public bool Equals(MatchingScore other)
        {
            if (other == null) return false;
            bool test1 = this.ScoreTotal.Equals(other.ScoreTotal);
            bool test2 = this.Pair.aIdx == other.Pair.aIdx;
            bool test3 = this.Pair.bIdx == other.Pair.bIdx;

            return (test1 && test2 && test3);
        }

        public float CalcEuclidianDistanceOverAllMatchScores(MatchingScore ms)
        {
            //TODO move this to score producer ??? 
            float result = -1;
            //we do a completeness check 
            foreach (string name in ms.MatchScoreColumnByName.Keys)
            {
                if (!this.MatchScoreColumnByName.ContainsKey(name))
                {
                    Trace.WriteLine("warning 29892389 column name not found result: " + name);
                    return result;
                }
            }
            result = EuclidianDistanceMultiDim(this.MatchScoreColumnByName.Values, ms.MatchScoreColumnByName.Values);

            return result;
        }

        private float EuclidianDistanceMultiDim(IEnumerable<byte> one, IEnumerable<byte> two)
        {
            //TODO move to own class 
            //TODO move this to score producer ??? 
            var sum = one.Select((x, i) => (x - two.ElementAt(i)) * (x - two.ElementAt(i))).Sum();
            return (float)Math.Sqrt(sum);
        }
    }
}
