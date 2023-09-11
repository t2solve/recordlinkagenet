using System;

namespace RecordLinkageNet.Core.Score
{
    public interface IScore : IEquatable<IScore>, IComparable<IScore>
    {
        float GetScoreValue();
    }
}
