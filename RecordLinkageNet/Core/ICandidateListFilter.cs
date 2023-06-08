using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public interface IResultFilter
    {
        MatchCandidateList Apply(MatchCandidateList g);
    }
}
