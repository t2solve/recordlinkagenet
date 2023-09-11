using RecordLinkageNet.Core.Score;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Core
{
    //! a candidate set which is oredered, e.g. all candidate in B for index A = 1 
    [DataContract(Name = "MatchGroupOrdered", Namespace = "RecordLinkageNet")]
    public class MatchGroupOrdered : ICandidateSet
    {
        //TODO use a base class to share double code MatchCandidateList  
        //TODO maybe melt with MachtCandidateList ?? 
        [DataMember(Name = "ListElements")]
        protected List<MatchCandidate> listElements = new List<MatchCandidate>();
        [DataMember(Name = "Criteria")]
        protected GroupCriteriaContainer criteria = null;
        [DataMember(Name = "Type")]
        protected GroupFactory.Type type = GroupFactory.Type.Unknown;
        protected IDistanceSet scoreDistances = null;

        public MatchGroupOrdered()
        {
            ScoreDistances = new ScoreDistanceList(this);
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

        //public void CalculateAllDistances()
        //{
        //    //TODO maybe to in another class ?? 
        //    //we get the top score
        //    MatchCandidate topCand = GetTopScoreCandidate();
        //    if (topCand != null)
        //    {
        //        ScoreDistances.AddSetToCandidateSet(this);
        //        //we do calc distance for all 
        //        foreach (MatchCandidate c in listElements)
        //        {
        //            ScoreDistances.AddScores(topCand.GetScore(),c.GetScore());
        //        }
        //    }
        //    else Trace.WriteLine("error 23534544656");
        //}

        public GroupFactory.Type Type { get => type; set => type = value; }
        public GroupCriteriaContainer Criteria { get => criteria; set => criteria = value; }
        public IDistanceSet ScoreDistances { get => scoreDistances; set => scoreDistances = value; }
    }
}
