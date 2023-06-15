using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Score
{
    public class ScoreDistanceList : IDistanceSet
    {
        private ICandidateSet parent  = null;
        private Dictionary<Tuple<IScore, IScore>, IScoreDistance> distDictio = new 
            Dictionary<Tuple<IScore, IScore>, IScoreDistance>();


        public ScoreDistanceList(ICandidateSet parent)
        {
            this.parent = parent;
        }

        public bool AddScores(IScore a, IScore b)
        {
            bool success = false; 
            //TODO  change this via intefaces
            if(a is WeightedScore && b is WeightedScore)
            {
                ////we produce a score
                //WeightedScore aWS = a as WeightedScore;
                //WeightedScore bWS = b as WeightedScore;
                //TODO maybe differ here in distance metric ? 
                WeightedScoreEuclidianDistance distanceObj = new WeightedScoreEuclidianDistance();

                if(distanceObj.CalculateDistance(a, b)>=0.0f)
                {
                    //we add it 
                    Tuple<IScore, IScore> newPair = new Tuple<IScore, IScore>(a,b);
                    distDictio.Add(newPair,distanceObj);
                    success = true;
                }
                else
                {
                    Trace.WriteLine("error 2938989 during calc distance"); 
                }
            }
            else
            {
                Trace.WriteLine("error 23523523 type of score : " + a.GetType().Name + 
                    " not known, will be ignored");
            }
            return success;
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
            else Trace.WriteLine("warning 298398 key not found in distance set, please AddScores before use");

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
