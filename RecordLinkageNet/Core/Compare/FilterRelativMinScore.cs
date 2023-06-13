using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public class FilterRelativMinScore : ICandidateListFilter
    {
        private float minThresholdInPercentage = 0.6f;

        public FilterRelativMinScore(float minThresholdInPercentage)
        {
            if(minThresholdInPercentage<0||minThresholdInPercentage>1.0f)
            {
                Trace.WriteLine("error 29382983 parameter minTresholdInPerentage out of range [0.0f,1.0f]");

                throw new ArgumentException("error 29382983 parameter out of range"); 

            }
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
