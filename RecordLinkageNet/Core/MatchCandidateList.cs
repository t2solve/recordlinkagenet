using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class MatchCandidateList : IEnumerable<MatchCandidate>
    {
        private List<MatchCandidate> listElements = null;

        public MatchCandidateList()
        {
            listElements = new List<MatchCandidate>(); 
        }

        public MatchCandidateList Add(MatchCandidate c)
        {
            listElements.Add(c);
            return this;
        }

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
    }
}
