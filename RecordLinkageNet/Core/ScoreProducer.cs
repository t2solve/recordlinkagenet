using RecordLinkage.Core;
using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //TODO make this to an inteface 
    public class ScoreProducer
    {
        public float ScoreAbsTotalMinAccepted = -1.0f;
        public float ScoreAbsTotalMax = -1.0f;
        //public NumberTransposeHelper.TransposeModus TransposeModus = NumberTransposeHelper.TransposeModus.UNKNOWN;//TODO reunite with config 
        //public float ScoreTotalReached = -1.0f;
        private ConditionList conditionList = null;
        private Configuration config = null;
        public ScoreProducer(Configuration config) //ConditionList conList, Configuration config)// NumberTransposeHelper.TransposeModus transposeModus)
        {
            conditionList = config.ConditionList;
            //TransposeModus = transposeModus;
            this.config = config;
            this.CalcAbsTotalMaxScore();

            //default percentag is 60 percent
            this.SetMinimumAcceptanceThresholdInPerentage(0.6f);
        }

        public void SetMinimumAcceptanceThresholdInPerentage(float percentage)
        {
            if (percentage < 0 || percentage > 1.0f)
            {
                Trace.WriteLine("error 2093029309 percetage out of range ");
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                ScoreAbsTotalMinAccepted = percentage * ScoreAbsTotalMax;
            }
        }

        private float ScaleWeightToByteRange(float x)
        {
            return x * byte.MaxValue;
        }

        public float CalcAbsTotalMaxScore()
        {
            float scoreMaxPossible = 0.0f;
            byte maxByte = byte.MaxValue;
            foreach (Condition con in conditionList)
            {
                if (con.ScoreWeight == -1.0f)
                {
                    Trace.WriteLine("warning 287387378 score weight for condtion not set, will ignore this conditon");
                }
                else
                {
                    scoreMaxPossible += (ScaleWeightToByteRange(con.ScoreWeight) * maxByte);
                }
            }
            ScoreAbsTotalMax = scoreMaxPossible;
            return scoreMaxPossible;
        }

        public float CalcAbsTotalReachedScore(MatchingScore m)
        {
            float scoreTotalReached = 0.0f;
            foreach (Condition con in conditionList)
            {
                if (con.ScoreWeight == -1.0f)
                {
                    Trace.WriteLine("warning 345345345 score weight for condtion not set, will ignore this conditon");
                }
                else
                {
                    //we get the score 
                    byte compareResult = 0;
                    if (m.MatchScoreColumnByName.TryGetValue(con.NameColNewLabel, out compareResult))
                    {
                        scoreTotalReached += (ScaleWeightToByteRange(con.ScoreWeight) * compareResult);
                    }
                    //else //not added yes
                    //    Trace.WriteLine("warning 763455235235 result column name not found in result");
                }
            }
            //we remember
            m.ScoreTotal = scoreTotalReached;
            return scoreTotalReached;
        }

        public bool CheckScoreReachedForMinimumReachable(MatchingScore m)
        {
            bool success = false;
            byte maxByte = byte.MaxValue;
            //update //TODO no calc every time ??
            float scoreReached = CalcAbsTotalReachedScore(m);

            //we check if we will be able to reach our minimum 
            float possibleRestScore = 0.0f;
            Dictionary<string, float> conditionColNames = new Dictionary<string, float>();
            foreach (Condition con in conditionList)
            {
                if (con.ScoreWeight != -1.0f)//only if set
                    conditionColNames.Add(con.NameColNewLabel, con.ScoreWeight);
            }
            //we calc 
            foreach (var data in conditionColNames)
            {
                if (!m.MatchScoreColumnByName.ContainsKey(data.Key))
                {
                    possibleRestScore += (ScaleWeightToByteRange(data.Value) * maxByte);
                }
            }

            if ((scoreReached + possibleRestScore) > ScoreAbsTotalMinAccepted)
                success = true;


            return success;
        }

        public float CalcScoreRelativePercentageFrom(MatchingScore m)
        {
            float relativeValue = -1.0f;
            if (ScoreAbsTotalMax != -1.0f && m.ScoreTotal != -1.0f)
            {
                relativeValue = (m.ScoreTotal * 100.0f) / ScoreAbsTotalMax;
            }
            else
            {
                Trace.WriteLine("warning 2982983 please configure score");
                throw new ArgumentOutOfRangeException();
            }
            return relativeValue;
        }

        public float CalcScoreAbsoluteValueFromPercentage(float percentage)
        {
            float absoluteValue = -1.0f;
            if (percentage < 0 || percentage > 1.0f)
            {
                Trace.WriteLine("error 2093029309 percetage out of range ");
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                if (ScoreAbsTotalMax == -1.0f)
                    ScoreAbsTotalMax = CalcAbsTotalMaxScore();

                absoluteValue = percentage * ScoreAbsTotalMax;
            }
            return absoluteValue;
        }
        public byte TransposeComparisonResult(float result)
        {
            if (config.NumberTransposeModus == NumberTransposeHelper.TransposeModus.UNKNOWN)
            {
                Trace.WriteLine("error 2039029309 set transpose modus before");
                return 0;
            }
            return NumberTransposeHelper.TransposeFloatToByteRange01(result, config.NumberTransposeModus);

        }

    }
}
