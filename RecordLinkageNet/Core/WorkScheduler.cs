using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using RecordLinkageNet.Core.Distance;
using RecordLinkage.Core;

namespace RecordLinkageNet.Core
{
    public class WorkScheduler
    {
        public Configuration config = null;
        public WorkScheduler(Configuration configuration)
        {
            config = configuration;
        }

        public void EstimateWork()
        {

            uint amountConditions = (uint)config.ConditionList.GetAmountConditions(); 
            uint amountCandidates =  (uint)config.Index.GetAmountIndexPairs();

            //calc jobs and amountConditions
            uint numberJobs = amountConditions * amountCandidates;
            int appIndexPairSize = 2 * 4; //2times 4 Byte(int) 
            MemoryUsageEstimator.CheckCreateArrayPossible(numberJobs, appIndexPairSize);
        
            //TODO react 
        }

        private static void ProduceWorkPackage(BufferBlock<int> queue, IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                queue.Post(value);
            }

            queue.Complete();
        }

        

        public static async void ProduceCompareJobs(BufferBlock<JobSet> queue,Configuration config)
        {
            //found here: 
            //https://blog.stephencleary.com/2012/11/async-producerconsumer-queue-using.html
            //int limitComparison = 100000;
            uint stepSize = 10;
            //TODO calc stepSize

            uint aMaxTest = config.Index.GetMaxADim(); //100

            for (uint a = 0; a < aMaxTest ; a+=stepSize)
            {
                JobSet set = new JobSet();
                set.aIdxStart = a;
                set.aIdxEnd =  a + stepSize;
                //complete B
                set.bIdxStart = 0;
                set.bIdxEnd = config.Index.GetMaxBDim();
                set.configuration = config;

                await queue.SendAsync(set);
               
            }
            queue.Complete(); 

        }

        public static ResultSet ProcessJob(JobSet jobSet)
        {
            ResultSet ret = new ResultSet();

            //TODO init
            ret.indexMap = new Dictionary<IndexPair, int>();

            int jobIdCounter = 0;
            int amountConditions = jobSet.configuration.ConditionList.GetAmountConditions();
            DataTable datA = jobSet.configuration.Index.dataTabA;
            DataTable datB = jobSet.configuration.Index.dataTabB;

            //TODO check here the strategy , full index or prelist

            //we do the job 
            for (uint a = jobSet.aIdxStart; a < jobSet.aIdxEnd; a++)
                for (uint b = jobSet.bIdxStart; b < jobSet.bIdxEnd; b++)
                {

                    ret.indexMap.Add(new IndexPair(a, b), jobIdCounter);

                    jobIdCounter += 1;
                    //we do compare all
                    foreach (Condition cond in jobSet.configuration.ConditionList)
                    {
                        DataColumn colA = datA.GetColumnByName(cond.NameColA);
                        DataColumn colB = datB.GetColumnByName(cond.NameColB);

                        //TODO fix fixed cast
                        DataCellString cellA = (DataCellString)colA.At((int)a);
                        DataCellString cellB = (DataCellString)colB.At((int)b);

                        float result = -1.0f;
                        //we compare
                        switch (cond.MyStringMethod)
                        {
                            case Condition.StringMethod.Exact:
                                result = CompareTaskFactory.CompareExact(
                                    0,
                                    cellA.Value.AsMemory(),
                                    cellB.Value.AsMemory()).Item2;
                                break;
                                //TODO implement more
                        }
                        byte resultAsByte = NumberTransposeHelper.TransposeFloatToByteRange01(result);

                        //TODO add
                    }
                }

            return ret;
        }
        public static async Task<ResultSet> Consume(BufferBlock<JobSet> queue)
        {
            ResultSet ret = new ResultSet();

            //TODO init
            ret.indexMap = new Dictionary<IndexPair, int>();

            // init result set
            int callCounter = 0; 
            while (await queue.OutputAvailableAsync())
            {
                callCounter += 1; 
                //var jobSet = await queue.ReceiveAsync();
                //multi consumer 
                queue.TryReceive(out JobSet jobSet);
                ret = ProcessJob(jobSet);
               //TODO fix
            }
            return ret;
        }

        public class JobSet
        {
            public uint aIdxStart = 0;
            public uint aIdxEnd = 0;
            public uint bIdxStart = 0;
            public uint bIdxEnd = 0;
            public Configuration configuration = null; 

            public JobSet()
            {
            }
        }
    }
}
