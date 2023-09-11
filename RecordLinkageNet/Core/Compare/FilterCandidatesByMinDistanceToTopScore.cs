using System;
using System.Diagnostics;
using System.Linq;

namespace RecordLinkageNet.Core.Compare
{
    public class FilterCandidatesByMinDistanceToTopScore : ICandidateListFilter
    {
        public FilterCandidatesByMinDistanceToTopScore(float minDistanceTresholdInPercentage)
        {
            if (minDistanceTresholdInPercentage < 0 || minDistanceTresholdInPercentage > 1.0f)
            {
                Trace.WriteLine("error 235235234 parameter minDistanceTresholdInPercentage out of range [0.0f,1.0f]");

                throw new ArgumentException("error 235235234 parameter out of range");
            }
            Configuration.Instance.SetFilterParameterThresholdRelativMinAllowedDistanceToTopScoree(minDistanceTresholdInPercentage);
        }
        public ICandidateSet Apply(ICandidateSet group)
        {
            ICandidateSet retObj = null;
            var matchGroupOrderes = group as MatchGroupOrdered;
            if (matchGroupOrderes != null)
            {
                //matchGroupOrderes.CalculateAllDistances();

                int amountConditions = Configuration.Instance.ConditionList.Count();
                //we calc the absolute from the relativ 
                float absoluteMinValueThreshold = amountConditions * byte.MaxValue *
                    Configuration.Instance.FilterParameterThresholdRelativMinAllowedDistanceToTopScore;

                if (amountConditions > 0)// && absoluteMinValue > 0)
                {
                    retObj = new MatchGroupOrdered();
                    var topScoredCand = matchGroupOrderes.GetTopScoreCandidate();
                    var topScore = topScoredCand.GetScore();
                    //we add the top score itself
                    //matchGroupOrderes.ScoreDistances.AddScores(topScore, topScore);

                    if (topScoredCand != null)
                    {
                        foreach (var cand in matchGroupOrderes)
                        {
                            var canScore = cand.GetScore();

                            //we add it 
                            var absDistance = matchGroupOrderes.ScoreDistances.AddScores(topScore, canScore);

                            if (absDistance >= absoluteMinValueThreshold)
                                retObj.Add(cand);
                        }
                    }
                    else Trace.WriteLine("error 2387873875 no top score found");
                }

            }
            else
            {
                Trace.WriteLine("error 239898 group is  wrong type: " + group.GetType().Name);
            }

            return retObj;
        }
    }
}
