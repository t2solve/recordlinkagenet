using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public interface IScore : IEquatable<IScore>, IComparable<IScore>
    {
        float GetScoreValue();
    }
}
