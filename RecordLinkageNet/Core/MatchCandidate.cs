using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class MatchCandidate : IEquatable<MatchCandidate>, IComparable<MatchCandidate>
    {
        private IndexPair idxPair = new IndexPair(); 
        private IScore score = null;

        public MatchCandidate(IndexPair idxPair)
        {
            this.idxPair = idxPair;
        }


        //TODO why by name ? , is ram inefficient
        //private Dictionary<string, byte> MatchScoreColumnByName = new Dictionary<string, byte>();
        //index 
        //private Dictionary<byte, byte> MatchScoreColumnByConditionIndex = new Dictionary<byte, byte>();
        public IScore GetScore()
        {
            return score; 
        }

        public void SetScore(IScore score)
        {
            this.score = score;
        }


        public IndexPair GetIndexPair()
        {
            return idxPair;
        }

        public bool Equals(MatchCandidate other)
        {
            if (this.idxPair.Equals(other.idxPair))
            {
                if (this.score.Equals(other.score))
                    return true; 
            }
            return false;
        }

        public int CompareTo(MatchCandidate other)
        {
            //more or less redirect
            return this.score.CompareTo(other.score);
        }
    }
}
