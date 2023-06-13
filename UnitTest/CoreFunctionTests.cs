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
using RecordLinkageNet.Core.Data.Transpose;

namespace UnitTest
{
    [TestClass]
    public class CoreFunctionTests
    {
        [TestMethod]
        public async Task BasicSystemTest1()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
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
            DataTableFeather tabA = TableConverter.CreateTableFeatherFromDataObjectList(testDataA);
            DataTableFeather tabB = TableConverter.CreateTableFeatherFromDataObjectList(testDataUnion);

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
            //conList.SortByScoreWeight();

            Configuration config = Configuration.Instance;
            config.AddIndex(new IndexFeather().Create(tabB, tabA));
            config.AddConditionList(conList);
            config.AddStrategy(Configuration.CalculationStrategy.WeightedConditionSum);
            config.AddNumberTransposeModus(NumberTransposeHelper.TransposeModus.LOG10);;

            //we init a worker
            WorkScheduler workScheduler = new WorkScheduler();
            //workScheduler.EstimateWork();
            var pipeLineCancellation = new CancellationTokenSource();
            var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

            await resultTask;

            //ResultSet result = resultTask.Result;

            int amount = resultTask.Result.Count();
            Assert.IsTrue(amount >= amountTestValueUnion, "wrong amount of results");

            TimeSpan stopwatchElapsed = sw.Elapsed;
            Trace.WriteLine("\tfinsihed used:\t" + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms");

        }

    }
}
