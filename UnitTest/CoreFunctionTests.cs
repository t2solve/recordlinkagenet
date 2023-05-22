using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using RecordLinkageNet.Core;
using System;
using System.IO;
using RecordLinkageNet.Core.Compare;
using RecordLinkage.Core;
using System.Collections.Generic;
using System.Linq;
using RecordLinkageNet.Core.Data;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class CoreFunctionTests
    {
        [TestMethod]
        public async Task BasicSystemTest1()
        {
            Stopwatch sw = new Stopwatch();
            int amountTestSet = 1000; 


            int amountTestValueUnion = (int)( (float) amountTestSet  * (0.1f)); // 10 percent
            var testDataA = TestDataGenerator.CreateTestPersons(amountTestSet);
            var testDataUnion = TestDataGenerator.CreateTestPersons(amountTestValueUnion); //generate union amount
           
            //we do a copy of a list https://stackoverflow.com/a/65584515/14105642
            var testDataUnion2 = testDataUnion.Select(b => b with { }).ToList();
            //we fake manipulate all 
            TestDataGenerator.ManipulateListAddAa(testDataUnion2);
            //we add the data
            testDataA.AddRange(testDataUnion2);
            testDataA.AddRange(testDataUnion);

            //TODO change to generic containter add
            DataTableFeather tabA = new DataTableFeather();
            // like tab.AddListAndCreate
            tabA.AddDataClassAsColumns(new TestDataPerson(), testDataA.Count);
            foreach (TestDataPerson p in testDataA)//we add all cells 
            {
                tabA.AddRow(p);
            }

            DataTableFeather tabB = new DataTableFeather();
            tabB.AddDataClassAsColumns(new TestDataPerson(), testDataUnion.Count);
            foreach (TestDataPerson p in testDataUnion) //we add all cells 
            {
                tabB.AddRow(p);
            }

            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            conList.String("NameFirst", "NameFirst", testMethod);
            conList.String("Street", "Street", testMethod);
            conList.String("PostalCode", "PostalCode", testMethod);
            conList.String("NameLast", "NameLast", testMethod);

            //add scores
            Dictionary<string, float> scoreTable = new Dictionary<string, float>();
            scoreTable.Add("NameLast", 2.0f);
            scoreTable.Add("NameFirst", 1.5f);
            scoreTable.Add("Street", 0.9f);
            scoreTable.Add("PostalCode", 0.7f);

            foreach (Condition c in conList)
            {
                c.ScoreWeight = scoreTable[c.NameColNewLabel];
            }
            //we sort it for a small speed up
            conList.SortByScoreWeight();

            Configuration config = Configuration.Instance;

            config.AddIndex(new IndexFeather().Create(tabB, tabA));
            config.AddConditionList(conList);

            config.NumberTransposeModus = NumberTransposeHelper.TransposeModus.LOG10;
            //we do change some pre set things
            config.ScoreProducer.SetMinimumAcceptanceThresholdInPerentage(0.8f);

            //we init a worker
            WorkScheduler workScheduler = new WorkScheduler();
            //workScheduler.EstimateWork();
            var pipeLineCancellation = new CancellationTokenSource();
            var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

            await resultTask;

            //ResultSet result = resultTask.Result;

            //int amount = result.MatchingScoreCompareResulList.Count;
            //Assert.IsTrue(amount >= amountTestValueUnion, "wrong amount of results");

            //Trace.WriteLine("amount of pot: matches in result set :" + amount);
            //var groupsComplete = result.GroupResultAsMatchingBlocks();

            ////we do a filter selection 
            //Trace.WriteLine("amount of groups: " + groupsComplete.Data.Count);

            //var groupsFiltered = result.FilterByConditon(config, 0.8f, 0.2f);
            //Trace.WriteLine("amount of groups: " + groupsFiltered.Data.Count);

            //TimeSpan stopwatchElapsed = sw.Elapsed;
            //Console.WriteLine("\tfinsihed used:\t" + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms");

        }

        //[TestMethod]
        //public void ReadAndWriteResultSetTestFunction()
        //{
        //    TestDataPerson[] inMemoryCollection = GenTestData();

        //    //we load the ml context for data parsing
        //    MLContext mlContext = new MLContext();
        //    IDataView dataA = mlContext.Data.LoadFromEnumerable<TestDataPerson>(inMemoryCollection);

        //    RecordLinkageNet.Core.Compare.Index indexer = new RecordLinkageNet.Core.Index();
        //    indexer.full();
        //    CandidateList can = indexer.index(dataA, dataA);
        //    ConditionList compare = new Compare(can);

        //    compare.Exact("PostalCode", "PostalCode");
        //    compare.String("NameFirst", "NameFirst", Condition.StringMethod.JaroWinklerSimilarity, 0.9f);
        //    compare.String("NameLast", "NameLast", Condition.StringMethod.JaroWinklerSimilarity, -1.0f, "foo");
        //    compare.Compute();
        //    ResultSet resIn = compare.PackedResult;
        //    string folderWeStoreIn = Path.Combine(Environment.CurrentDirectory, "testdir");

        //    Console.WriteLine(folderWeStoreIn);
        //    bool succes = ResultSetIOHelper.WriteResultSetToFolder(folderWeStoreIn, resIn);
        //    Assert.IsTrue(succes, "error during write");

        //    ResultSet resOut = ResultSetIOHelper.ReadResultSetToFolder(folderWeStoreIn);
        //    Assert.IsNotNull(resOut);

        //    //we compare the sets 
        //    Assert.AreEqual(resIn.colNames[0], resOut.colNames[0]);

        //    Assert.AreEqual(resIn.data[0, 1], resOut.data[0, 1], "wrong data");

        //}

        //private TestDataPerson[] GenTestData()
        //{
        //    //we do basic functional
        //    TestDataPerson[] inMemoryCollection = new TestDataPerson[]
        //    {
        //            new TestDataPerson
        //            {
        //                NameFirst = "thomas",
        //                NameLast = "mueller",
        //                City = "moxee",
        //                Street = "91st ave",
        //                PostalCode =  "98944"
        //            },
        //            new TestDataPerson
        //            {

        //                NameFirst = "adalina",
        //                NameLast = "nibbs",
        //                City = "waterville",
        //                Street = "jefferson",
        //                PostalCode = "98944"
        //            }
        //    };

        //    return inMemoryCollection;
        //}
    }
}
