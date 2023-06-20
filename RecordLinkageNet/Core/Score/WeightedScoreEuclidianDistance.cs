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
        private float distance = -1.0f;

        public float Distance { get => distance; set => distance = value; }
        public WeightedScore ScoreA { get => scoreA; set => scoreA = value; }
        public WeightedScore ScoreB { get => scoreB; set => scoreB = value; }

        public float CalculateDistance(IScore a, IScore b)
        {
            Distance = -1.0f;
            ScoreA = a as WeightedScore;
            ScoreB = b as WeightedScore;
            Distance = WeightedScoreDistanceProducer.CalcEuclidianDistance(ScoreA, ScoreB); 
            return Distance; 
        }

        public int CompareTo(IScoreDistance other)
        {
            WeightedScoreEuclidianDistance otherCast = other as WeightedScoreEuclidianDistance;
            if (otherCast == null)
                throw new ArgumentNullException(nameof(otherCast));

            //only focus on distance 
            return (int)(otherCast.Distance - this.Distance);
        }

        public bool Equals(IScoreDistance other)
        {
            if (other == null)
                return false;

            WeightedScoreEuclidianDistance otherCast = other as WeightedScoreEuclidianDistance;
            if (otherCast == null)
                return false; 

            //only focus on distance 
            return (otherCast.Distance == this.Distance);
        }

        public float GetValue()
        {
            return distance; 
        }
    }
}
