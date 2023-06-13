using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! a candidate set which is oredered, e.g. all candidate in B for index A = 1 
    public class MatchGroupOrdered : ICandidateSet
    {
        //TODO maybe melt with MachtCandidateList ?? 

        protected List<MatchCandidate> listElements = new List<MatchCandidate>();
        protected GroupCriteriaContainer criteria = null;
        protected GroupFactory.Type type = GroupFactory.Type.Unknown;

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

        public List<MatchCandidate> Data
        {
            get { return this.listElements; }
            set { this.listElements = value; }
        }

        public GroupFactory.Type Type { get => type; set => type = value; }
        public GroupCriteriaContainer Criteria { get => criteria; set => criteria = value; }
    }
}
