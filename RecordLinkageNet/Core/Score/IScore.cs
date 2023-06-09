using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public interface IScore : IEquatable<IScore>, IComparable<IScore>
    {
        public enum AcceptanceLevel
        {
            Unknown,
            MatchAccepted,
            MatchRejected,
        }

        //! foobar

        float Calculate(MatchCandidate x);

    }
}
