using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public interface IScore : IEquatable<IScore>, IComparable<IScore>
    {
        float Calculate(MatchCandidate x);
    }
}
