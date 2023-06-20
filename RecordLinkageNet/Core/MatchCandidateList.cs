using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "MatchCandidateList", Namespace = "RecordLinkageNet")]

    public class MatchCandidateList : ICandidateSet
    {
        //TODO use a base class to share double code MatchGroupOrdered  
        [DataMember(Name = "ListElements") ]
        private List<MatchCandidate> listElements = new List<MatchCandidate>(); 

        public MatchCandidateList AddRange(MatchCandidateList list)
        {
            listElements.AddRange(list);
            return this; 
        }

        public bool ContainsIndexPair(Compare.IndexPair p )
        {
            foreach(MatchCandidate c in listElements)
            {
                if (c.GetIndexPair().Equals(p))
                    return true;
            }
            return false;
        }

        public IEnumerator<MatchCandidate> GetEnumerator()
        {
            return listElements.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void SortByScoreTopDown()
        {
            listElements.Sort(delegate (MatchCandidate c1, MatchCandidate c2) 
            { return c1.GetScore().CompareTo(c2.GetScore()); });

        }

        public void Add(MatchCandidate candidate)
        {
            listElements.Add(candidate);
        }

        public MatchCandidate GetTopScoreCandidate()
        {
            this.SortByScoreTopDown();
            if (listElements.Count > 0)
                return listElements.First();
            return null;
        }
    }
}
