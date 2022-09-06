using Microsoft.ML;
using RecordLinkageNet.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! class to create index 
    public class Index
    {

        bool flagFullMode = false;
        //TODO: add block possibibilty
        List<Tuple<string, string>> blockNames = null; 

        public void full()
        {
            flagFullMode = true;
        }

        public CandidateList index(IDataView dataA, IDataView dataB = null)
        {
            CandidateList candidates = new CandidateList();

            candidates.canA = new Candidate(dataA);
            candidates.canB = new Candidate(dataB);

            //create a index list 
            if (flagFullMode)
            {
                candidates.CreateFullIndexList();
            }
            else //TODO double check others
            {
                Console.WriteLine("warning no index specified, will use full");
                candidates.CreateFullIndexList();
            }
            return candidates;
        }

    }
}