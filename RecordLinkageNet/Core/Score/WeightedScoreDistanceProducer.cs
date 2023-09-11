using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScoreDistanceProducer
    {
        public static float CalcEuclidianDistance(WeightedScore sa, WeightedScore sb)
        {
            float result = -1;
            //we do check parameter
            if (sa == null || sb == null)
            {
                Trace.WriteLine("error 28932983 null parameter");
                throw new ArgumentException("error 28932983");
            }
            if (!sa.IsScoreComplete() || !sb.IsScoreComplete())
            {
                Trace.WriteLine("error 2345634463123 please build the score before use ist ");
                throw new ArgumentException("error 2345634463123");
            }

            //we do a completeness check 
            foreach (var pA in sa.GetScoreParts())
            {
                if (!sb.GetScoreParts().ContainsKey(pA.Key))
                {
                    Trace.WriteLine("error 4564562234 index key not found in result ");
                    throw new ArgumentException("error 4564562234");
                }
            }
            foreach (var pB in sb.GetScoreParts())
            {
                if (!sa.GetScoreParts().ContainsKey(pB.Key))
                {
                    Trace.WriteLine("error 56875643453 index key not found in result ");
                    throw new ArgumentException("error 56875643453");
                }
            }
            //float result = -1;
            result = EuclidianDistanceMultiDim(sb.GetScoreParts().Values, sa.GetScoreParts().Values);
            return result;
        }

        private static float EuclidianDistanceMultiDim(IEnumerable<byte> one, IEnumerable<byte> two)
        {
            //do it the linq way
            var sum = one.Select((x, i) => (x - two.ElementAt(i)) * (x - two.ElementAt(i))).Sum();
            return (float)Math.Sqrt(sum);
        }
    }
}
