using RecordLinkageNet.Core.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! a candidate set which is oredered, e.g. all candidate in B for index A = 1 
    public class MatchGroupOrdered : ICandidateSet
    {
        //TODO use a base class to share double code MatchCandidateList  
        //TODO maybe melt with MachtCandidateList ?? 

        protected List<MatchCandidate> listElements = new List<MatchCandidate>();
        protected GroupCriteriaContainer criteria = null;
        protected GroupFactory.Type type = GroupFactory.Type.Unknown;
        protected IDistanceSet scoreDistances = null;

        public MatchGroupOrdered()
        {
            scoreDistances = new ScoreDistanceList(this);
        }

        public void Add(MatchCandidate c)
        {
            this.listElements.Add(c);
        }

        public void SortByScoreTopDown()
        {
            listElements.Sort(delegate (MatchCandidate c1, MatchCandidate c2) 
            { return c1.GetScore().CompareTo(c2.GetScore()); });
        }

        public IEnumerator<MatchCandidate> GetEnumerator()
        {
            return listElements.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public MatchCandidate GetTopScoreCandidate()
        {
            this.SortByScoreTopDown();
            if (listElements.Count > 0)
                return listElements.First();
            return null; 
        }

        public List<MatchCandidate> Data
        {
            get { return this.listElements; }
            set { this.listElements = value; }
        }

        public void CalculateAllDistances()
        {
            //TODO maybe to in another class ?? 
            //we get the top score
            MatchCandidate topCand = GetTopScoreCandidate();
            if (topCand != null)
            {
                scoreDistances.AddSetToCandidateSet(this);
                //we do calc distance for all 
                foreach (MatchCandidate c in listElements)
                {
                    scoreDistances.AddScores(topCand.GetScore(),c.GetScore());
                }
            }
            else Trace.WriteLine("error 23534544656");
        }

        public GroupFactory.Type Type { get => type; set => type = value; }
        public GroupCriteriaContainer Criteria { get => criteria; set => criteria = value; }
    }
}
