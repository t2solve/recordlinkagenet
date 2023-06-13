using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public interface IScoreDistance : IEquatable<IScoreDistance>, IComparable<IScoreDistance>
    {
        float CalculateDistance(IScore a, IScore b);
    }
}
