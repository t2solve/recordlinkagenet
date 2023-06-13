using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public interface ICandidateSet : IEnumerable<MatchCandidate>
    {
        public void Add(MatchCandidate candidate);

        public void SortByScoreTopDown();
    }
}
