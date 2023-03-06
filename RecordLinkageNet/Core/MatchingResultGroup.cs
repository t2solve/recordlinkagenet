using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class MatchingResultGroup
    {
        public enum GroupingDirection
        {
            Unknown,
            IndexAIsKeyForGroup,
            IndexBIsKeyGorGroup,
        }

        public GroupingDirection MyGroupingDirection = GroupingDirection.Unknown;
        public uint IndexKey = uint.MaxValue;
        public List<MatchingScore> CandidateList = new List<MatchingScore>();
        public List<float> CandidateListDistancesToTopScore = new List<float>(); //TODO refactor to dictio is dangerous, when resort 
        public MatchingScore GetTopScore()
        {
            if (CandidateList.Count > 0)
                return CandidateList.First();
            return null;
        }
    }
}
