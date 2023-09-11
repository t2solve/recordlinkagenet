using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace RecordLinkageNet.Core.Score
{
    public class ScoreDistanceList : IDistanceSet
    {
        private ICandidateSet parent = null;
        private Dictionary<Tuple<IScore, IScore>, IScoreDistance> distDictio = new
            Dictionary<Tuple<IScore, IScore>, IScoreDistance>();


        public ScoreDistanceList(ICandidateSet parent)
        {
            this.parent = parent;
        }

        public float AddScores(IScore a, IScore b)
        {
            //TODO change this via intefaces

            float absDistance = -1.0f;
            if (a is WeightedScore && b is WeightedScore)
            {
                ////we produce a score
                //WeightedScore aWS = a as WeightedScore;
                //WeightedScore bWS = b as WeightedScore;
                //TODO maybe differ here in distance metric ? 
                WeightedScoreEuclidianDistance distanceObj = new WeightedScoreEuclidianDistance();

                absDistance = distanceObj.CalculateDistance(a, b);
                if (absDistance >= 0.0f)
                {
                    //we add it 
                    Tuple<IScore, IScore> newPair = new Tuple<IScore, IScore>(a, b);

                    if (distDictio.ContainsKey(newPair))//check double add 

                    {
                        //replace //TODO check is same ?? 
                        distDictio[newPair] = distanceObj;
                    }
                    else
                    {
                        distDictio.Add(newPair, distanceObj);

                    }


                }
                else
                {
                    Trace.WriteLine("error 2938989 during calc distance");
                    absDistance = -1.0f;
                }
            }
            else
            {
                Trace.WriteLine("error 23523523 type of score : " + a.GetType().Name +
                    " not known, will be ignored");
            }
            return absDistance;
        }

        public void AddSetToCandidateSet(ICandidateSet group)
        {
            this.parent = group;
        }

        public IScoreDistance GetDistance(IScore a, IScore b)
        {
            IScoreDistance retObj = null;

            var tup = new Tuple<IScore, IScore>(a, b);
            if (distDictio.ContainsKey(tup))
            {
                retObj = distDictio[tup];
            }
            else
            {
                //we add the distance 
                float d = this.AddScores(a, b);
                if (d == -1.0f)
                    Trace.WriteLine("warning 23453412 GetDistance error during calc distance, will return null");
                else
                    retObj = distDictio[tup];
            }
            return retObj;
        }

        public IEnumerator<IScoreDistance> GetEnumerator()
        {
            return distDictio.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
