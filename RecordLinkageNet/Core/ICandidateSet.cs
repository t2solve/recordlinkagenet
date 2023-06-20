using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core
{

    public interface ICandidateSet : IEnumerable<MatchCandidate>
    {
        public void Add(MatchCandidate candidate);

        public void SortByScoreTopDown();

        public MatchCandidate GetTopScoreCandidate();
    }
}
