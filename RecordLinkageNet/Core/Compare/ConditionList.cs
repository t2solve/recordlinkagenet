using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class which does the comparision, internal iterating over DataSet A and B 
    /// </summary>
    public class ConditionList : IEnumerable<Condition>
    {
        //jobId, result
        //private Queue<Task<Tuple<long, float>>> myTaskList = new Queue<Task<Tuple<long, float>>>(); //really ?? 
        private List<Condition> conditionList = new List<Condition>();//condition list 
        //private CandidateList canList = null;
        //                // jobID, index
        //private Dictionary<long,IndexPair> myIndexJobMap = new Dictionary<long, IndexPair>();//lookup table

        //      //resultInd, conditionNr
        //private float[,] result = null; //TODO check double needed ? or smaller ones short, bool?? 
        //public ResultSet PackedResult = null;
        //private long amountOfTotalJobsTODO = 0;
        //private long amountOfTotalJobsDone = 0;

        //private readonly int maxElementsInQueue = 1000000;  //TODO maybe make it adjustable

        public enum ConditionCompareMode //Check is this clever here ? 
        {
            FullSetBased, // every condition one by one
            DecisionTreeBased,
            Unknown
        }
        public ConditionCompareMode ProcessMode = ConditionCompareMode.FullSetBased;

        public int GetAmountConditions()
        {
            return conditionList.Count;
        }

        //public Compare(CandidateList c)
        //{   
        //    canList = c;

        //    //TODO set amount CPU now is always
        //    int amountCpu = Environment.ProcessorCount;
        //    //Trace.WriteLine("amount cpu: "+ amountCpu + " used");

        //    int d1 = canList.canA.GetHashCode();
        //    int d2 = canList.canB.GetHashCode();

        //    ////TODO check if identity is same 
        //    //if(d1 == d2)
        //    //{
        //        //shortcut
        //    //}

        //    //small way around here ? 
        //    //Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)1;
        //    ////TODO double check 
        //    //bool succes = ThreadPool.SetMaxThreads(amountCpu, amountCpu);
        //    //if(!succes)
        //    //{
        //    //    throw new Exception("error during set max threads");
        //    //}

        //    //Console.WriteLine("foo: " + Process.GetCurrentProcess().ProcessorAffinity);

        //    //basicly no control is overed here
        //    //https://stackoverflow.com/a/42277947/14105642
        //}

        //private bool IsTaskQueueFull()
        //{
        //    //TODO maybe set a ram limit ??
        //    //Trace.WriteLine("ram usage: " + GetRamUsedInMegaByte() + " mb"); 
        //    if (myTaskList.Count> maxElementsInQueue) //TODO check wether count is expensive ??
        //    {
        //        return true; 
        //    }
        //    return false; 
        //}

        public Condition Exact(string dataAColName, string dataBColName, string newColLabel = null)
        {
            //TODO check  class recordlinkage.compare.Exact(left_on, right_on, agree_value=1, disagree_value=0, missing_value=0, label=None)
            //TODO check asserts 
            //Debug.Assert(canList.CheckDataAHasColWithName(dataAColName), "error 29882938 col name not found");
            //Debug.Assert(canList.CheckDataBHasColWithName(dataBColName), "error 23235325 col name not found");

            //we save the have 
            Condition j = new Condition();
            j.Mode = Condition.CompareType.Exact;
            j.NameColA = dataAColName;
            j.NameColB = dataBColName;
            j.NameColNewLabel = newColLabel;

            j.SetNewColName();
            conditionList.Add(j);

            return j;
        }

        public Condition String(string dataAColName, string dataBColName, Condition.StringMethod method,  string newColLabel = null)
        {

            //Debug.Assert(canList.CheckDataAHasColWithName(dataAColName), "error 2342346534 col name not found");
            //Debug.Assert(canList.CheckDataBHasColWithName(dataBColName), "error 2342355456 col name not found");

            //TODO also test types => string = string etc.

            //we save the have 
            Condition j = new Condition();
            j.Mode = Condition.CompareType.String;
            j.NameColA = dataAColName;
            j.NameColB = dataBColName;
            j.MyStringMethod = method;
            //j.threshold = threshold;
            j.NameColNewLabel = newColLabel;

            j.SetNewColName();
            conditionList.Add(j);

            return j;
        }

        public void SortByScoreWeight()
        {
            conditionList.Sort(); //do default sort
        }

        public IEnumerator<Condition> GetEnumerator()
        {
            return conditionList.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //public bool Compute()
        //{
        //    //TODO: vector type
        //    //TODO: bool type ? 
        //    //TODO double type

        //    bool success = true; 
        //    long amountConditions = myConditionList.Count();
        //    if(amountConditions==0)
        //    {
        //        Trace.WriteLine("warning 3783748374, no condition added use Compare.String, Compare.Exact functions , will abort here");
        //        return false;      
        //    }
        //    //TODO check wether use Assert
        //    //Debug.Assert((amountConditions != 0),); 

        //    long amountElements = CalcAmountComparisions();

        //    long amountRowsInB = canList.GetAmountRowsB();

        //    //create result     resultIndex    , condNr
        //    this.result = new float[amountElements, amountConditions];
        //    //we init the array with -1
        //    //result = Enumerable.Repeat(Enumerable.Repeat(-1.0f, (int)amountConditions).ToArray(), (int)amountElements).ToArray();
        //    //TODO check old one existed ?

        //    //generate the jobs 
        //    long numberJobs = amountConditions * amountElements;
        //    amountOfTotalJobsTODO = numberJobs;
        //    //Trace.WriteLine("amount jobs TODO in total: " + amountOfTotalJobsTODO);
        //    //TODO warning here if too many
        //    Task<Tuple<long, float>> task = null;

        //    long jobCounter = 0;
        //    //List< ValueGetter < ReadOnlyMemory<char> >> cloneBList = new List< ValueGetter < ReadOnlyMemory<char> >>();
        //    short conditionCounter = 0; 

        //    //TODO might check if we do have same columns multiple times ? 
        //    foreach (CompareCondition job in myConditionList)
        //    {
        //        //get the columns by name 
        //        DataViewSchema.Column colA = (DataViewSchema.Column)canList.canA.GetColumnByName(job.NameColA);
        //        DataViewSchema.Column colB = (DataViewSchema.Column)canList.canB.GetColumnByName(job.NameColB);

        //        //we pack it into a list 
        //        List<DataViewSchema.Column> colAList = new List<DataViewSchema.Column>();
        //        List<DataViewSchema.Column> colBList = new List<DataViewSchema.Column>();
        //        //create iterable 
        //        colAList.Add(colA);
        //        colBList.Add(colB);
        //        //get the row cursors
        //        DataViewRowCursor cursorA = canList.canA.data.GetRowCursor(colAList);

        //        //we check the types of both columns
        //        //check docu here
        //        //https://github.com/dotnet/machinelearning/blob/main/docs/code/IDataViewTypeSystem.md
        //        DataViewType typeA = colA.Type;
        //        DataViewType typeB = colB.Type;
        //        if (typeA != typeB)
        //        {
        //            Trace.WriteLine("error 29839283  two columns of one condition have different types could not compare");
        //            return false ;
        //        }

        //        //TODO implement others
        //        //CASE col is string 
        //        if (typeA == TextDataViewType.Instance)//string
        //        {

        //            //if exact was used. we set the compare string compare method to exact
        //            if(job.Mode==CompareCondition.CompareType.Exact) //TODO double check this style
        //            {
        //                job.MyStringMethod = CompareCondition.StringMethod.Exact;
        //            }

        //            long indexA = 0;
        //            while (cursorA.MoveNext())
        //            {
        //                Memory<char> textValueACopy = GetStringValue(colA, cursorA);
        //                DataViewRowCursor cursorB = canList.canB.data.GetRowCursor(colBList);
        //                long indexB = 0;
        //                while (cursorB.MoveNext())
        //                {
        //                    IndexPair indx = new IndexPair(indexA, indexB, conditionCounter);
        //                    Memory<char> textValueBCopy = GetStringValue(colB, cursorB);
        //                    long jobID = jobCounter; //we do a copy

        //                    task = CompareTaskFactory.CreateStringCompare(jobID, job, textValueACopy, textValueBCopy);

        //                    if (task != null)
        //                    {
        //                        myIndexJobMap.Add(jobCounter, indx);
        //                        myTaskList.Enqueue(task);
        //                        //we check 
        //                        if (IsTaskQueueFull())
        //                            ComputeQueueElements();
        //                    }
        //                    else
        //                    {
        //                        Trace.WriteLine("error 23982938 during create  compare task");
        //                        success = false;
        //                    }
        //                    jobCounter += 1;
        //                    indexB += 1;
        //                }//end while b
        //                indexA += 1;
        //            }//end while a
        //        }//end case string
        //        else if (typeA == NumberDataViewType.Single) //float  CASE
        //        {
        //            //we do a direct comparison
        //            //no jobs or so ?? TODO double check overhead of task create
        //            long indexA = 0;
        //            float valueA = default;
        //            ValueGetter<float> valueADelegate = cursorA.GetGetter<float>(colA);
        //            while (cursorA.MoveNext())
        //            {
        //                //get a
        //                valueADelegate.Invoke(ref valueA); 

        //                DataViewRowCursor cursorB = canList.canB.data.GetRowCursor(colBList);
        //                float valueB = default;
        //                ValueGetter<float> valueBDelegate = cursorB.GetGetter<float>(colB);
        //                long indexB = 0;
        //                while (cursorB.MoveNext())
        //                {
        //                    //IndexPair idx = new IndexPair(indexA, indexB, conditionCounter);
        //                    valueBDelegate.Invoke(ref valueB);
        //                    long jobID = jobCounter; //we do a copy
        //                    float compResult = 0.0f;

        //                    //TODO compare and write result
        //                    if (valueA==valueB)
        //                    {
        //                        //success
        //                        compResult = 1.0f;
        //                    }
        //                    //wrong
        //                    //int resultIndex =(int) (indexA * indexB);
        //                    //TODO use function to unify calc, same in GetResultIndex(canList.canB.GetAmountRows()
        //                    int resultIndex = (int)((indexA * (amountRowsInB)) +  indexB); //TODO 
        //                    //int foo = conditionCounter;
        //                    //try //not needed here
        //                    //{
        //                    this.result[resultIndex, (int)conditionCounter] = compResult;//copyValue;
        //                    //}
        //                    //catch (Exception e)
        //                    //{
        //                    //    Trace.WriteLine("error 2982938 " + e.ToString());
        //                    //}
        //                    jobCounter += 1;
        //                    indexB += 1;

        //                }//end while b
        //                indexA += 1;
        //            }//end while a
        //        }
        //        else
        //        {
        //            Trace.WriteLine("warning 3908398989 column type not suppurted yet  during create  compare task");
        //            success = false;
        //        }
        //        conditionCounter += 1; //we count
        //    }//end contion 

        //    //Trace.WriteLine("----- start final queue at end");
        //    ComputeQueueElements();
        //    if (success)
        //        PackResultSet();

        //    return success;
        //}

        //private void PackResultSet()
        //{
        //    //we get the colnames
        //    List<string> colNameList = new List<string>();
        //    foreach (CompareCondition j in myConditionList)
        //        colNameList.Add(j.NameColNewLabel);

        //    PackedResult = new ResultSet(canList.indexList, colNameList, this.result);

        //}

        //private Memory<char> GetStringValue(DataViewSchema.Column col, DataViewRowCursor cursor)
        //{
        //    //https://docs.microsoft.com/de-de/dotnet/machine-learning/how-to-guides/inspect-intermediate-data-ml-net
        //    Memory<char> textValueCopy = null; 
        //    ReadOnlyMemory<char> textValue = default;
        //    //get value getter
        //    ValueGetter<ReadOnlyMemory<char>> textGetterDelegateB = cursor.GetGetter<ReadOnlyMemory<char>>(col);
        //    //get values from respective columns
        //    textGetterDelegateB.Invoke(ref textValue);
        //    //we do a copy TODO better handling ? 
        //    textValueCopy = new char[textValue.Length];
        //    if (!textValue.TryCopyTo(textValueCopy))
        //    {
        //        Trace.WriteLine("error 880998 during copy value");
        //    }
        //    return textValueCopy;
        //}



        //private void ComputeQueueElements()
        //{
        //    //we start all 
        //    int amountJobsThisIteration = myTaskList.Count();
        //    Task[] myTaskListStarted = myTaskList.ToArray();

        //    //Trace.WriteLine($"\tstart { amountJobsThisIteration}  jobs this iter");

        //    //Stopwatch stopwatch = new Stopwatch();
        //    //stopwatch.Start();

        //    //we dequeue all and
        //    //we start all in parrallel
        //    while (myTaskList.Count>0)
        //    {
        //       Task<Tuple<long,float>>  t = myTaskList.Dequeue();
        //       t.Start();
        //    }

        //    Task.WaitAll(myTaskListStarted); // wait for all 

        //    foreach (Task<Tuple<long, float>> finishedTask in myTaskListStarted)
        //    {
        //        //foo
        //        if (finishedTask.IsCompleted)
        //        {
        //                //jobID, result
        //            Tuple<long, float> taskResult = finishedTask.Result;

        //            //we get indexA, indexB, condNr
        //            IndexPair idx = myIndexJobMap.GetValueOrDefault(taskResult.Item1);

        //            //we write to result 
        //            result[idx.GetResultIndex(canList.canB.GetAmountRows()),idx.conditionIndex] = taskResult.Item2; //copyValue;
        //            //Console.WriteLine(taskResult);

        //        }//else warning
        //    }

        //    amountOfTotalJobsDone += amountJobsThisIteration;

        //    //stopwatch.Stop();
        //    //TimeSpan stopwatchElapsed = stopwatch.Elapsed;
        //    //Trace.WriteLine("\tfinsihed used:\t" + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms");

        //    //we calc percent ready 
        //    long percentDone = amountOfTotalJobsDone * 100 / amountOfTotalJobsTODO;
        //    //Trace.WriteLine($"\t {percentDone} percent done");

        ////}
        //private long CalcAmountComparisions()
        //{ 
        //    long result = 0;
        //    long n = canList.GetAmountRowsA();
        //    long m = canList.GetAmountRowsB();

        //    Debug.Assert(n != 0, "error 29832983, need a NOT empty data view A");

        //    if (m == 0)
        //        return n;

        //    result = m * n;

        //    return result; 
        //}

    }
}
