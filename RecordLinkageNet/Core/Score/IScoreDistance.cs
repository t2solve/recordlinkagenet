using System;

namespace RecordLinkageNet.Core.Score
{
    public interface IScoreDistance : IEquatable<IScoreDistance>, IComparable<IScoreDistance>
    {
        float CalculateDistance(IScore a, IScore b);

        float GetValue();
    }
}
