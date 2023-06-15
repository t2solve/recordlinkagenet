using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public interface IDistanceSet : IEnumerable<IScoreDistance>
    {
        void AddSetToCandidateSet(ICandidateSet group);

        bool AddScores(IScore a, IScore b);

        IScoreDistance GetDistance(IScore a, IScore b);

    }
}
