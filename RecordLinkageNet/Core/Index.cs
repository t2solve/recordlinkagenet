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
    /// <summary>
    /// Class to create an index of two datasets
    /// </summary>
    public class Index
    {

        bool flagFullMode = false;
        //TODO: add block possibibilty
        List<Tuple<string, string>> blockNames = null;

        /// <summary>
        /// function to configure class to use a fullindex
        /// </summary>
        public void full()
        {
            flagFullMode = true;
        }

        /// <summary>
        /// function to create a index 
        /// </summary>
        /// <param name="dataA">DataView of DataSet A</param>
        /// <param name="dataB">DataView of DataSet B</param>
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