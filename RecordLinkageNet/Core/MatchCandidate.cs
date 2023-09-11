using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Score;
using System;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "MatchCandidate", Namespace = "RecordLinkageNet")]
    [KnownType(typeof(WeightedScore))]//need to declare all implementation https://stackoverflow.com/a/11800139
    public class MatchCandidate : IEquatable<MatchCandidate>, IComparable<MatchCandidate>
    {
        public enum AcceptanceLevel
        {
            Unknown,
            MatchAccepted,
            MatchRejected,
        }

        [DataMember(Name = "IndexPair")]
        private IndexPair idxPair = new IndexPair();
        [DataMember(Name = "Score")]
        private IScore score = null;
        [DataMember(Name = "AcceptanceLevel")]
        private AcceptanceLevel acceptanceLvl = AcceptanceLevel.Unknown;


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

        public AcceptanceLevel AcceptanceLvL { get => acceptanceLvl; set => acceptanceLvl = value; }

    }
}
