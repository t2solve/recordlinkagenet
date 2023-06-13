using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! a candidate set which is oredered, e.g. all candidate in B for index A = 1 
    public class MatchGroupOrdered
    {
        private List<MatchCandidate> listElements = new List<MatchCandidate>();
        private GroupCriteriaContainer criteria = null;
        private GroupFactory.Type type = GroupFactory.Type.Unknown;



        public void Add(MatchCandidate c)
        {
            this.listElements.Add(c);
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
