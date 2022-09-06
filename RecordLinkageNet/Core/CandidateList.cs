using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class CandidateList
    {
        public Candidate canA { get; set; } = null;

        public Candidate canB { get; set; } = null;

        //public 
        public List<Tuple<int,int>> indexList = null;

        public CandidateList()
        {
            indexList = new List<Tuple<int, int>>();
        }

        public bool CheckDataAHasColWithName(string name)
        {
            return this.canA.CheckDataHasColWithName( name);
        }

        public bool CheckDataBHasColWithName(string name)
        {
            return this.canB.CheckDataHasColWithName(name);
        }

        public long GetAmountRowsA()
        {
            return canA.GetAmountRows();
          
        }
        public long GetAmountRowsB()
        {
            return canB.GetAmountRows();
        }

        public void CreateFullIndexList()
        {
            indexList.Clear();
            for(int i = 0; i < canA.GetAmountRows(); i++)
            {
                for (int j = 0; j < canB.GetAmountRows(); j++)
                {
                    indexList.Add(new Tuple<int, int>(i, j));   
                }
            }
        }
    }
}
