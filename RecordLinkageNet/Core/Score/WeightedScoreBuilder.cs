using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScoreBuilder
    {
        private WeightedScore scoreObject = null;
        private float thresoldMinScoreAbsTotalAccepted = -1.0f;
        private float scoreAbsTotalMaxReachable = -1.0f;
        private int amountScoreParts = 0;

        public WeightedScoreBuilder(MatchCandidate c, float minTresholdInPerentage = 0.6f)
        {
            this.scoreObject = new WeightedScore(c);
            //calc from config 
            //everytime the same !! 

            this.scoreAbsTotalMaxReachable = WeightedScoreCalculator.CalcAbsTotalMaxScoreFromConditions();
            this.thresoldMinScoreAbsTotalAccepted = WeightedScoreCalculator.CalcMinimumAcceptanceThresholdInPerentage(this.scoreAbsTotalMaxReachable,minTresholdInPerentage);
            amountScoreParts = Configuration.Instance.ConditionList.Count();
        }

        public WeightedScoreBuilder AddScorePart(string conditionName, float scoreOfPart)
        {
            //we get the index 
            byte index = Configuration.Instance.ConditionList.GetConditionIndexByName(conditionName); 
            if(index==byte.MaxValue)
            {
                Trace.WriteLine("error 2932398 during get condition by name : " + conditionName);
                throw new ArgumentException("error 2932398 columnName wrong"); 
            }
            scoreObject.AddScorePart(index,scoreOfPart);

            return this; 
        }


        private static class WeightedScoreCalculator
        {
            public static float CalcMinimumAcceptanceThresholdInPerentage(float scoreAbsTotalMaxReachable , float percentage)
            {
                float thresoldMinScoreAbsTotalAccepted= -1.0f; 
                if (percentage < 0 || percentage > 1.0f)
                {
                    Trace.WriteLine("error 2093029309 parameter minTresholdInPerentage out of range [0,1.0f]");
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    thresoldMinScoreAbsTotalAccepted = percentage * scoreAbsTotalMaxReachable;
                }

                return thresoldMinScoreAbsTotalAccepted;
            }
            //private static float ScaleWeightToByteRangeMax(float x)
            //{
            //    return x * byte.MaxValue;
            //}

            public static float CalcAbsTotalMaxScoreFromConditions()
            {
                float scoreMaxPossible = 0.0f;
                byte maxByte = byte.MaxValue;
                foreach (Condition con in Configuration.Instance.ConditionList)
                {
                    if (con.ScoreWeight == -1.0f)
                    {
                        Trace.WriteLine("warning 287387378 score weight for condtion idx: " + con.ConditionIndex + " not set, will ignore this conditon");
                    }
                    else
                    {
                        scoreMaxPossible += con.ScoreWeight * maxByte;
                    }
                }
                //ScoreAbsTotalMax = scoreMaxPossible;
                return scoreMaxPossible;
            }
        }
    }
}
