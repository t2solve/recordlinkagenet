using System.Collections.Generic;

namespace RecordLinkageNet.Core
{

    public interface ICandidateSet : IEnumerable<MatchCandidate>
    {
        public void Add(MatchCandidate candidate);

        public void SortByScoreTopDown();

        public MatchCandidate GetTopScoreCandidate();
    }
}
