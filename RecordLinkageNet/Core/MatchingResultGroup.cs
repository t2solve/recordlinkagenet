using RecordLinkageNet.Core.Compare;
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
        //[DataMember(Name = "ImportantIDList")]
        //public Dictionary<string, List<string>> ImportantIDList = new Dictionary<string, List<string>>();
        
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

        public bool DeleteCandidate(IndexPair idx)
        {
            bool succes = false;

            //we search the index 
            int i = -1;
            int counter = 0; 
            foreach(MatchingScore ms in CandidateList)
            {
                if (idx.Equals(ms.Pair))
                {
                    i = counter;
                }
                counter += 1;
            }

            if(i!=-1)
            {
                CandidateList.RemoveAt(i);
                CandidateListDistancesToTopScore.RemoveAt(i);
                succes = true;
            }

            //TODO recalc the distances
            return succes;
        }
    }
}
