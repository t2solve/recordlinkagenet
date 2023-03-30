using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "MatchingResultGroup", Namespace = "DataContracts")]
    public class MatchingResultGroup
    {
        public enum GroupingDirection
        {
            Unknown,
            IndexAIsKeyForGroup,
            IndexBIsKeyGorGroup,
        }

        [DataMember(Name = "MyGroupingDirection")]
        public GroupingDirection MyGroupingDirection = GroupingDirection.Unknown;
        [DataMember(Name = "IndexKey")]
        public uint IndexKey = uint.MaxValue;
        [DataMember(Name = "CandidateList")]
        public List<MatchingScore> CandidateList = new List<MatchingScore>(); 
        [DataMember(Name = "CandidateListDistancesToTopScore")]
        public List<float> CandidateListDistancesToTopScore = new List<float>(); //TODO refactor to dictio is dangerous, when resort 

        public MatchingResultGroup()
        {

        }
        public MatchingScore GetTopScore()
        {
            if (CandidateList.Count > 0)
                return CandidateList.First();
            return null;
        }

        public int GetAmountCandidates()
        {
            return CandidateList.Count;
        }
    }
}
