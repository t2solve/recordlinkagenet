using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public interface ICandidateListFilter
    {
        MatchCandidateList Apply(MatchCandidateList g);
    }
}
