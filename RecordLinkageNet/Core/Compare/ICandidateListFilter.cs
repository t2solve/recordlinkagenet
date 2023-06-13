using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public interface ICandidateListFilter
    {
        ICandidateSet Apply(ICandidateSet g);
    }
}
