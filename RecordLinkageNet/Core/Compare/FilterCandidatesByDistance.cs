using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public class FilterCandidatesByDistance : ICandidateListFilter
    {
        public FilterCandidatesByDistance(float minDistanceTresholdInPercentage)
        {
            if (minDistanceTresholdInPercentage < 0 || minDistanceTresholdInPercentage > 1.0f)
            {
                Trace.WriteLine("error 235235234 parameter minDistanceTresholdInPercentage out of range [0.0f,1.0f]");

                throw new ArgumentException("error 235235234 parameter out of range");
            }
            Configuration.Instance.SetFilterParameterThresholdRelativMinDistance(minDistanceTresholdInPercentage);
        }
        public ICandidateSet Apply(ICandidateSet group)
        {
            ICandidateSet retObj = null;
            var matchGroupOrderes = group as MatchGroupOrdered;
            if (matchGroupOrderes != null)
            {
                matchGroupOrderes.CalculateAllDistances();

                int amountConditions = Configuration.Instance.ConditionList.Count();
                //we calc the absolute from the relativ 
                float absoluteMinValue = amountConditions * byte.MaxValue * 
                    Configuration.Instance.FilterParameterThresholdRelativMinDistance ;


            }
            else
            {
                Trace.WriteLine("error 239898 group is  wrong type: " + group.GetType().Name);
            }

            return retObj;
        }
    }
}
