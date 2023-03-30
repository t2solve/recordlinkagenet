using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "ResultGroup", Namespace = "DataContracts")]
    public class ResultGroup //TODOD: find a better name, 
    {
        [DataMember(Name = "Data")]
        public List<MatchingResultGroup> Data = new List<MatchingResultGroup>();
        public ResultGroup()
        {
        }
        public ResultGroup(List<MatchingResultGroup> group)
        {
            Data = group;
        }

       
    }
}
