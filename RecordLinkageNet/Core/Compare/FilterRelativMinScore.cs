using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public class FilterRelativMinScore : ICandidateListFilter
    {
        private float minThresholdInPercentage = 60.0f;

        public FilterRelativMinScore(float minThresholdInPercentage)
        {
            this.minThresholdInPercentage = minThresholdInPercentage;
        }

        public MatchCandidateList Apply(MatchCandidateList g)
        {
            MatchCandidateList newList = new MatchCandidateList();
            foreach (MatchCandidate mc in g)
            {
                IScore score = mc.GetScore(); 
                //TODO differt scoring type 
                if (score is WeightedScore)
                {
                    float scorePercentage = WeightedScoreProducer.Instance.CalcRelativeScoreValueInPercentage((WeightedScore) score);

                    if (scorePercentage >= minThresholdInPercentage)
                        newList.Add(mc);
                }
            }

            return newList;
        }
    }
}
