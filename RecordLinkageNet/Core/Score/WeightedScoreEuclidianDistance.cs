using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScoreEuclidianDistance : IScoreDistance
    {
        private WeightedScore scoreA;
        private WeightedScore scoreB;
        private float distance = 1.0f; 
        public float CalculateDistance(IScore a, IScore b)
        {
            distance = -1.0f;
            scoreA = a as WeightedScore;
            scoreB = b as WeightedScore;
            distance = WeightedScoreDistanceProducer.CalcEuclidianDistance(scoreA, scoreB); 
            return distance; 
        }

       
    }
}
