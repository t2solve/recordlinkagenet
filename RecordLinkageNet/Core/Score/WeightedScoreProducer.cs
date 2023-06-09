using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class WeightedScoreProducer
    {
        //*** singelton pattern
        //accoring to https://csharpindepth.com/Articles/Singleton
        private static readonly Lazy<WeightedScoreProducer> lazy =
        new Lazy<WeightedScoreProducer>(() => new WeightedScoreProducer());

        public static WeightedScoreProducer Instance { get { return lazy.Value; } }

        private WeightedScoreProducer()
        {
            this.scoreAbsTotalMaxReachable = CalcAbsTotalMaxScoreFromConditionList();
            this.amountScoreParts = Configuration.Instance.ConditionList.Count();
            SetMinScoreThresholdInPercentage(thresholdMinTresholdInPerentage);

        }
        //**** end singelton code 

        private float thresholdMinTresholdInPerentage = 0.7f;//default 70 percent
        private float thresholdMinScoreAbsTotalAccepted = -1.0f;
        private float scoreAbsTotalMaxReachable = -1.0f;
        private int amountScoreParts = 0;

        public bool AddScorePartAndWeightIt(WeightedScore scoreObj,string conditionName, float comparisionResult)
        {
            bool success = false;
            //we get the index 
            byte index = Configuration.Instance.ConditionList.GetConditionIndexByName(conditionName);
            if (index == byte.MaxValue)
            {
                Trace.WriteLine("error 2932398 during get condition by name : " + conditionName);
                throw new ArgumentException("error 2932398 columnName wrong");
                return success;
            }

            byte scoreTransformed = TransposeComparisonResult(comparisionResult);
            
            //calc
            float scorePartWeighted = GetWeightForConditionIndex(index) * scoreTransformed;

            float scoreSummedParts =  scoreObj.AddScorePart(index, scorePartWeighted, scoreTransformed);
            
            
            if(CheckWeCouldStillReachMinimum(scoreObj,scoreSummedParts))
                success = true; 

            return success;
        }

        private float GetWeightForConditionIndex( byte index)
        {
            float weight = Configuration.Instance.ConditionList.GetConditionByIndex(index).ScoreWeight;
            if (weight == -1.0f)
            {
                Trace.WriteLine("error 34983948 not weight set for condition");
                throw new ArgumentNullException("error 34983948");

            }
            return weight; 
        }
        private bool CheckWeCouldStillReachMinimum(WeightedScore scoreObj,float scoreSummedParts)
        {
            if (scoreSummedParts > this.thresholdMinScoreAbsTotalAccepted)
                return true;

            //if (scoreObj.IsScoreComplete()) //not realy needed here, no speedup etc.
            //{
            //    if(scoreObj.GetScoreValue()>this.thresholdMinScoreAbsTotalAccepted)
            //        return true;
            //}
            //we do check 
            List<byte> listOfMissingIndexParts = scoreObj.GetIndexListOfMissingParts();

            //we calc what we could add max
            float addableRest = 0.0f;
            foreach(byte index in listOfMissingIndexParts)
            {
                addableRest += (byte.MaxValue * GetWeightForConditionIndex(index));
            }

            
            if ((scoreSummedParts + addableRest) >= this.thresholdMinScoreAbsTotalAccepted)
                return true; 

            return false; 
        }

        public byte TransposeComparisonResult(float result)
        {
            if (Configuration.Instance.NumberTransposeModus == NumberTransposeHelper.TransposeModus.UNKNOWN)
            {
                Trace.WriteLine("error 2039029309 set transpose modus before");
                return 0;
            }
            return NumberTransposeHelper.TransposeFloatToByteRange01(result, Configuration.Instance.NumberTransposeModus);

        }

        public void SetMinScoreThresholdInPercentage(float percentage)
        {
            if (percentage < 0 || percentage > 1.0f)
            {
                Trace.WriteLine("error 2093029309 parameter minTresholdInPerentage out of range [0,1.0f]");
                throw new ArgumentOutOfRangeException("error 2093029309 parameter");
            }
            else
            { 
                this.thresholdMinTresholdInPerentage = percentage;
                this.thresholdMinScoreAbsTotalAccepted = CalcAbsMinimumScoreWhichShouldBeReached();
            }
            
            
        }

        private float CalcAbsMinimumScoreWhichShouldBeReached()
        {   
            float minAbs = this.thresholdMinTresholdInPerentage * scoreAbsTotalMaxReachable;
            return minAbs;
        }
        //private static float ScaleWeightToByteRangeMax(float x)
        //{
        //    return x * byte.MaxValue;
        //}

        private float CalcAbsTotalMaxScoreFromConditionList()
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
