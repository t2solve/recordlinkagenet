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
        private List<MatchCandidate> data = null;

        public MatchCandidateList()
        {
            data = new List<MatchCandidate>(); 
        }

        public MatchCandidateList Add(MatchCandidate c)
        {
            data.Add(c);
            return this;
        }

        public MatchCandidateList AddRange(MatchCandidateList list)
        {
            data.AddRange(list);
            return this; 
        }

        public IEnumerator<MatchCandidate> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
