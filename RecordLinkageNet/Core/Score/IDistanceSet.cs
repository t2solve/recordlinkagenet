using System.Collections.Generic;

namespace RecordLinkageNet.Core.Score
{
    public interface IDistanceSet : IEnumerable<IScoreDistance>
    {
        void AddSetToCandidateSet(ICandidateSet group);

        float AddScores(IScore a, IScore b);

        IScoreDistance GetDistance(IScore a, IScore b);

    }
}
