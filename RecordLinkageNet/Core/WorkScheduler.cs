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
using System.Threading;

namespace RecordLinkageNet.Core
{
    public class WorkScheduler
    {
        private Configuration config = null; // TODO delete here
        public WorkScheduler()//Configuration configuration)
        {
            //small redirect
            config = Configuration.Instance;
            //config = configuration;
        }

        //! helper class for job handling
        private class JobSet
        {
            public uint aIdxStart = 0;
            public uint aIdxEnd = 0;
            public uint aIdxAmount = 0;
            public uint bIdxStart = 0;
            public uint bIdxEnd = 0;
            public uint bIdxAmount = 0;
            //public Configuration configuration = null;

            public JobSet()
            {
            }

            public int CalcPercentageDone()
            {
                UInt64 amountTODO = aIdxAmount * bIdxAmount; 
                UInt64 amountDone = aIdxEnd * bIdxAmount;

                UInt64 percentage = amountDone * 100 / amountTODO; 
                return (int)percentage;
            }
        }

        public async Task<ResultSet> Compare(CancellationToken stopToken, IProgress<int> progress = null)
        {
           
            ////TODO check config
            //Task< ResultSet>  t = Task.Run(() =>
            //{
            ResultSet allResults = new ResultSet();
            //using tpl to compute in consumer producer pattern 
            //    https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-implement-a-producer-consumer-dataflow-pattern

            //define a queue to exchange data
            var queue = new BufferBlock<JobSet>();

            // Start the producer and consumer.
            WorkScheduler.ProduceCompareJobs(queue);

            //define options for comsumer
            var consumerOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = config.AmountCPUtoUse,
                CancellationToken = stopToken
            };

            List<ResultSet> resultList = new List<ResultSet>();
            var consumer = new ActionBlock<JobSet>(x => resultList.Add(WorkScheduler.ProcessJobSet(x,progress)), consumerOptions);
            queue.LinkTo(consumer, new DataflowLinkOptions { PropagateCompletion = true, });

            // Wait for everything to complete.
            await Task.WhenAll(consumer.Completion, queue.Completion);

            //we sum up all results 
            foreach (ResultSet rs in resultList)
                allResults.MatchingScoreCompareResulList.AddRange(rs.MatchingScoreCompareResulList);

            Trace.WriteLine("debug: amount of matches is: " + allResults.MatchingScoreCompareResulList.Count);

            return allResults;
            //}
            //return t; 
        }

        //public void EstimateWork()
        //{

        //    uint amountConditions = (uint)config.ConditionList.GetAmountConditions(); 
        //    uint amountCandidates =  (uint)config.Index.GetAmountIndexPairs();

        //    //calc jobs and amountConditions
        //    uint numberJobs = amountConditions * amountCandidates;
        //    int appIndexPairSize = 2 * 4; //2times 4 Byte(int) 
        //    MemoryUsageEstimator.CheckCreateArrayPossible(numberJobs, appIndexPairSize);

        //    //TODO react 
        //}

        //private static void ProduceWorkPackage(BufferBlock<int> queue, IEnumerable<int> values)
        //{
        //    foreach (var value in values)
        //    {
        //        queue.Post(value);
        //    }

        //    queue.Complete();
        //}



        private static async void ProduceCompareJobs(BufferBlock<JobSet> queue)
        {
            Configuration config = Configuration.Instance;
            //TODO check what is when b is small but a very big ?? 
            //--> leads to huge jobs

            //found here: 
            //https://blog.stephencleary.com/2012/11/async-producerconsumer-queue-using.html
            //int limitComparison = 100000;
            uint stepSize = 10;
            //TODO calc stepSize

            uint aMaxTest = config.Index.GetMaxADim(); //100

            for (uint a = 0; a < aMaxTest ; a+=stepSize)
            {
                if (a >= aMaxTest) //do a padding
                    a = aMaxTest - 1;

                JobSet set = new JobSet();
                set.aIdxStart = a;
                set.aIdxEnd =  a + stepSize;
                set.aIdxAmount = config.Index.GetMaxADim();
                //complete B
                set.bIdxStart = 0;
                set.bIdxEnd = config.Index.GetMaxBDim();
                set.bIdxAmount = config.Index.GetMaxBDim();
                //set.configuration = config;

                await queue.SendAsync(set);
               
            }
            queue.Complete(); 

        }

        private static MatchingScore DoMatching(IndexPair p)//, Configuration configuration)
        {
            Configuration configuration = Configuration.Instance;
            MatchingScore matchingScore = new MatchingScore( p);

            int conditionCounter = 0;
            //int conditionAmount = configuration.ConditionList.GetAmountConditions(); 
            //we do compare all
            foreach (Condition cond in configuration.ConditionList)
            {
                DataColumn colA = configuration.Index.dataTabA.GetColumnByName(cond.NameColA);
                DataColumn colB = configuration.Index.dataTabB.GetColumnByName(cond.NameColB);

                if (colA== null || colB == null)
                {
                    Trace.WriteLine("error 98392839 column in datatable not found will be irngored ");
                    continue;
                }


                //TODO fix fixed cast ??? how todo
                DataCellString cellA = (DataCellString)colA.At((int)p.aIdx);
                DataCellString cellB = (DataCellString)colB.At((int)p.bIdx);

                float result = -1.0f;
                //we do a short cut 
                if (cellA == null || cellB == null)
                {
                    result = 0.0f;
                    //Trace.WriteLine("warning 29398238 data cell is null at indexA:"+ p.aIdx + " and indexB:"+ p.bIdx);
                }
                else
                {
                    //we compare
                    switch (cond.MyStringMethod)
                    {
                        case Condition.StringMethod.Exact:
                            result = IsStringSame(cellA.Value, cellB.Value);
                            break;
                        case Condition.StringMethod.JaroWinklerSimilarity:
                            result = (float)JaroWinkler.JaroDistance(cellA.Value, cellB.Value);
                            break;
                        case Condition.StringMethod.HammingDistance:
                            result = (float)Hamming.HammingDistance(cellA.Value, cellB.Value);
                            break;
                        case Condition.StringMethod.DamerauLevenshteinDistance:
                            result = (float)DamerauLevenshtein.DamerauLevenshteinDistance(cellA.Value, cellB.Value);
                            break;
                        case Condition.StringMethod.ShannonEntropyDistance:
                            result = (float)ShannonEntropy.ShannonEntropyDistance(cellA.Value, cellB.Value);
                            break;
                        case Condition.StringMethod.MyCustomizedDistance:
                            result = (float)CustomizedDistance.MyCustomizedDistance(cellA.Value, cellB.Value);
                            break;
                        default:
                            Trace.WriteLine("error critical  20392093 try to use not implemented compare method");
                            throw new NotImplementedException();
                            break;
                    }
                }//end comparable

                conditionCounter += 1;

                if (result != -1.0f)
                    matchingScore.AddScore(cond.NameColNewLabel, result);

                //test if min Score is still reachable at all 

                //version 1 //TODO optimize reprogramm fluent score calc,  
                // is ~ 30 percent faster than version 2 , tested with 4 conditions
                if (!configuration.ScoreProducer.CheckScoreReachedForMinimumReachable(matchingScore))
                {
                    //Trace.WriteLine("debug: 9283928 skip try here con#: ", conditionCounter + "\tof\t" + conditionAmount);
                    //we abort here 
                    return null;
                }

            }

            ////version 2 is slower than version 1
            ////update
            //configuration.ScoreProducer.CalcTotalReachedScore(matchingScore);
            //if (matchingScore.ScoreTotal < configuration.ScoreProducer.ScoreTotalMinAccepted)
            //{
            //    matchingScore = null;
            //}

            return matchingScore;
        }

        private static float IsStringSame(string a, string b)
        {
            float result = 0;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0.0f;

            if (string.Compare(a, b) == 0)//same same
                return 1.0f; //
            else
                return 0.0f;
        }

        private static ResultSet ProcessJobSet(JobSet jobSet, IProgress<int> progressInformer=null)
        {
            ResultSet ret = new ResultSet();

            //TODO init
            //ret.indexMap = new Dictionary<IndexPair, int>();

            ret.MatchingScoreCompareResulList = new List<MatchingScore>();

            uint jobIdCounter = 0;
            //int amountConditions = jobSet.configuration.ConditionList.GetAmountConditions();
            //DataTable datA = jobSet.configuration.Index.dataTabA;
            //DataTable datB = jobSet.configuration.Index.dataTabB;

            //TODO check here the strategy , full index or prelist

            //we do the job 
            for (uint a = jobSet.aIdxStart; a < jobSet.aIdxEnd; a++)
                for (uint b = jobSet.bIdxStart; b < jobSet.bIdxEnd; b++)
                {
                    var indexPair = new IndexPair(a, b);

                    var matchingResult = DoMatching(indexPair); //, jobSet.configuration);
                    if(matchingResult!=null)
                        ret.MatchingScoreCompareResulList.Add(matchingResult);

                    jobIdCounter += 1;
                  
                }

            //we calc a progress
            if (progressInformer != null)
                 progressInformer.Report(jobSet.CalcPercentageDone()); 

            return ret;
        }
        //public static async Task<ResultSet> Consume(BufferBlock<JobSet> queue)
        //{
        //    ResultSet ret = new ResultSet();

        //    //TODO init
        //    ret.indexMap = new Dictionary<IndexPair, int>();

        //    // init result set
        //    int callCounter = 0; 
        //    while (await queue.OutputAvailableAsync())
        //    {
        //        callCounter += 1; 
        //        //var jobSet = await queue.ReceiveAsync();
        //        //multi consumer 
        //        queue.TryReceive(out JobSet jobSet);
        //        ret = ProcessJob(jobSet);
        //       //TODO fix
        //    }
        //    return ret;
        //}

    }
}
