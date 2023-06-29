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
using RecordLinkageNet.Core.Score;

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

        public async Task<MatchCandidateList> Compare(CancellationToken stopToken, IProgress<int> progress = null)
        {
            if(!config.IsValide())
            {
                Trace.WriteLine("warning 293898 abort Compare, configuration is not valide");
                return null; 
            }

            //TODO check config before compute
            Configuration.Instance.EnterDoCompareCalculationModus();

            MatchCandidateList allResults = new MatchCandidateList();
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

            List<MatchCandidateList> resultList = new List<MatchCandidateList>();
            var consumer = new ActionBlock<JobSet>(x => resultList.Add(WorkScheduler.ProcessJobSet(x,progress)), consumerOptions);
            queue.LinkTo(consumer, new DataflowLinkOptions { PropagateCompletion = true, });

            // Wait for everything to complete.
            await Task.WhenAll(consumer.Completion, queue.Completion);

            //we sum up all results 
            foreach (MatchCandidateList rs in resultList)
                allResults.AddRange(rs);

            Trace.WriteLine("debug: amount of matches is: " + allResults.Count());

            Configuration.Instance.ExitDoCompareCalculationModus();

            return allResults;

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
            uint stepSize = 5;
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


        private static MatchCandidate DoMatching2(IndexPair p)//, Configuration configuration)
        {
            Configuration conf = Configuration.Instance;
            MatchCandidate mCan = new MatchCandidate(p);


            //TODO differ all strats
            switch(conf.Strategy)
            {
                case Configuration.CalculationStrategy.WeightedConditionSum:
                    mCan.SetScore(new WeightedScore(mCan));
                    break;
                case Configuration.CalculationStrategy.Unknown:
                    throw new Exception("error 238787 no strategy selected");
                    return null;
            }
            
            
            int conditionCounter = 0;
            //int conditionAmount = configuration.ConditionList.GetAmountConditions(); 
            //we do compare all
            foreach (Condition cond in conf.ConditionList)
            {
                DataColumn colA = conf.Index.dataTabA.GetColumnByName(cond.NameColA);
                DataColumn colB = conf.Index.dataTabB.GetColumnByName(cond.NameColB);

                if (colA == null || colB == null)
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

                if (result == -1.0f)
                {
                    throw new Exception("error 29382938989 during calc distance");
            
                }
                if (conf.Strategy == Configuration.CalculationStrategy.WeightedConditionSum)
                {
                    WeightedScore wScore = mCan.GetScore() as WeightedScore;
                    bool mReachableFlag = WeightedScoreProducer.Instance.AddScorePartAndWeightIt(wScore, cond.NameColNewLabel, result);

                    if (!mReachableFlag)
                        return null;
                }


            }//end for each
            return mCan;
        }

        private static float IsStringSame(string a, string b)
        {
            //TODO move to other class, is a short cut here 
            float result = 0;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0.0f;

            if (string.Compare(a, b) == 0)//same same
                return 1.0f; //
            else
                return 0.0f;
        }

        private static MatchCandidateList ProcessJobSet(JobSet jobSet, IProgress<int> progressInformer=null)
        {
            MatchCandidateList ret = new MatchCandidateList();
            uint jobIdCounter = 0;
            //TODO check here the strategy , full index or prelist

            //we do the job 
            for (uint a = jobSet.aIdxStart; a < jobSet.aIdxEnd; a++)
                for (uint b = jobSet.bIdxStart; b < jobSet.bIdxEnd; b++)
                {
                    var indexPair = new IndexPair(a, b);

                    var matchingResult = DoMatching2(indexPair); //, jobSet.configuration);
                    if(matchingResult!=null)
                        ret.Add(matchingResult);

                    jobIdCounter += 1;
                  
                }

            //we calc a progress
            if (progressInformer != null)
                 progressInformer.Report(jobSet.CalcPercentageDone()); 

            return ret;
        }
 

    }
}
