using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! a helper class for to serialzie a complete set 
    public class MatchGroupOrderedList
    {
        private List<MatchGroupOrdered> listElements = null;

        public MatchGroupOrderedList()
        {
            this.listElements = new List<MatchGroupOrdered>();
        }   

        public void Add(MatchGroupOrdered o)
        {
            this.listElements.Add(o);
        }

        public List<MatchGroupOrdered> Data
        {
            get { return this.listElements; }
            set { this.listElements = value; }
        }
    }
}
