using Microsoft.ML;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class to create an index of two datasets
    /// </summary>
    public class Index //TODO rename to avoid conflict
    {
        public enum ConstructionState
        {
            UNKNOWN,
            ONLY_MAX_STORED,
            ARRAY_FULL_CREATED
        }

        public enum IndexType
        {
            UNKNOWN,
            FULL_INDEX,
            PREFILTERED
        }

        public IndexType type = IndexType.UNKNOWN;
        public ConstructionState state = ConstructionState.UNKNOWN;
        //bool flagFullMode = false;
        //private List<IndexPair> indexList = new List<IndexPair>();
        private IndexPair[] indexArray = null;
        private uint idxAMax = uint.MaxValue;
        private uint idxBMax = uint.MaxValue;
        public DataTable dataTabA = null; 
        public DataTable dataTabB = null;

        public int GetAmountIndexPairs()
        {
            return indexArray.Length; 
        }
        public uint GetMaxADim()
        {
            return idxAMax; 
        }
        public uint GetMaxBDim()
        {
            return idxBMax;
        }

        ////TODO: add block possibibilty
        ////List<Tuple<string, string>> blockNames = null;

        ///// <summary>
        ///// function to configure class to use a fullindex
        ///// </summary>
        //public void full()
        //{
        //    flagFullMode = true;
        //}

        /// <summary>
        /// function to create a index 
        /// </summary>
        /// <param name="dataA">DataTable of DataSet A</param>
        /// <param name="dataB">DataTable of DataSet B</param>
        public IndexPair[] Create(DataTable dataA, DataTable dataB = null, Condition prefilter = null)
        {
            idxAMax = (uint) dataA.GetAmountRows();
            dataTabA = dataA;
            dataTabB = dataB;
            
            if (prefilter == null)
            {
                type = IndexType.FULL_INDEX;
                //flagFullMode = true; 
                //we create a full set, for a 
                if (dataB == null)
                {
                    //we mark b as a, to make future steps easier
                    dataTabB = dataTabA;
                    idxBMax = idxAMax; 

                    long max = (long)idxAMax;
                    //short estitmate 
                    long amoountValues = max * max ;
                    int appIndexPairSize = 2 * 4; //2times 4 Byte(int) 
                    if(MemoryUsageEstimator.CheckCreateArrayPossible(amoountValues, appIndexPairSize))
                    {
                        //we init the array 
                        indexArray = new IndexPair[amoountValues];

                        uint counter = 0;
                        for (uint i = 0; i < max; i++)
                            for (uint j = 0; j < max; j++)
                            {
                                //if (i % 100== 0 && j % 100 == 0)
                                //    Trace.WriteLine("foo"); 
                                //indexList.Add(new IndexPair(i, j));
                                indexArray[counter] = new IndexPair(i, j);
                                counter += 1;
                                if (counter > amoountValues)
                                    Trace.WriteLine("bar");

                            }
                        state = ConstructionState.ARRAY_FULL_CREATED; 
                    }
                    else
                    {
                        state = ConstructionState.ONLY_MAX_STORED;

                        //TODO save start and end 
                        throw new NotImplementedException(); 
                    }
                }
                else
                {
                    idxBMax = (uint)dataB.GetAmountRows();
                    state = ConstructionState.ONLY_MAX_STORED;
                    throw new NotImplementedException();
                    //TODO rewrite
                    ////short estitmate 
                    //uint amoountValues = idxAMax * idxBMax;

                    ////we init the array 
                    //indexArray = new IndexPair[amoountValues];
                    //uint counter = 0;
                    //for (uint i = 0; i < idxAMax; i++)
                    //    for (uint j = i; j < idxBMax; j++)
                    //    {
                    //        indexArray[counter] = new IndexPair(i, j);
                    //        counter += 1;

                    //    }
                }
            }
            else
            {
                type = IndexType.PREFILTERED;

                throw new NotImplementedException();

                //TODO support multi conditions
                /*
                 *    var joined = from Item1 in tabA.GetColumnByName("NameLast")
                         join Item2 in tabB.GetColumnByName("NameLast")
                         on Item1.Value equals Item2.Value  // join on some property
                         select new { IdA = Item1.Id, IdB = Item2.Id };
                */

                //Trace.WriteLine("warning no index specified, will use full");
                //candidates.CreateFullIndexList();
            }
            return indexArray;
        }

    }
}