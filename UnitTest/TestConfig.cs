using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkage.Core;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static RecordLinkageNet.Core.WorkScheduler;

namespace UnitTest
{
    [TestClass]
    public class TestConfig
    {
        private void ManipulateSet(List<TestDataPerson> list)
        {
            foreach(TestDataPerson person in list)
            {
                person.NameFirst = 'a' + person.NameFirst.Remove(0, 1);
                person.NameLast = 'a' + person.NameLast.Remove(0, 1);
                person.City  = 'a' + person.City.Remove(0, 1);

            }
        }
        [TestMethod]
        public async Task TestConfigEstiamte()
        {
            int amountTestValueUnion = 100; 
            var testDataA = TestDataGenerator.CreateTestPersons(1000);
            var testDataUnion = TestDataGenerator.CreateTestPersons(amountTestValueUnion); //generate union amount
            //we fake manipulate all 
            //we do a copy of a list https://stackoverflow.com/a/65584515/14105642
            var testDataUnion2 = testDataUnion.Select(b => b with { }).ToList();
            
            ManipulateSet(testDataUnion2);
            testDataA.AddRange(testDataUnion2);
            testDataA.AddRange(testDataUnion);

            //TODO change to generic containter add
            DataTable tabA = new DataTable();
            // like tab.AddListAndCreate
            tabA.AddDataClassAsColumns(new TestDataPerson(), testDataA.Count);
            foreach (TestDataPerson p in testDataA)//we add all cells 
            {
                tabA.AddRow(p);
            }

            DataTable tabB = new DataTable();
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

            foreach(Condition c in conList)
            {
                c.ScoreWeight = scoreTable[c.NameColNewLabel];
            }
            //we sort it for a small speed up
            conList.SortByScoreWeight();
            //float stepSize = 0.01f;
            //float maxCompareValueOutPut = 255;
            //float scale = 1.0f;
            ////laut by 
            //float con1plzWeight = 0.7f * scale; //1  //dataA[0,].Average();//TODO use linq
            //float con2vornameWeight = 1.5f * scale;//5
            //float con3nameWeight = 2.0f * scale;//4
            //float con4gbdateWeight = 4.0f * scale;//4.0f* scale;//10
            //float con5ibanWeight = scoreIBANuser * scale;//4.0f* 255;
            //float con6placeWeight = 0.5f * scale;
            //float con7streetWeight = 0.9f * scale;
            //float con8streetNumberWeight = 0.4f * scale;

            Configuration config = new Configuration();

            config.AddIndex(new IndexFeather().Create(tabB, tabA));
            config.AddConditionList(conList);
            config.NumberTransposeModus = NumberTransposeHelper.TransposeModus.LOG10;
            //we do change some pre set things
            config.ScoreProducer.SetMinimumAcceptanceThresholdInPerentage(0.8f);

            //we init a worker
            WorkScheduler workScheduler = new WorkScheduler(config);
            //workScheduler.EstimateWork();
            var pipeLineCancellation = new CancellationTokenSource();
            var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

            await resultTask;

            ResultSet result = resultTask.Result; 

                  int amount = result.MatchingScoreCompareResulList.Count;
            Assert.IsTrue(amount >= amountTestValueUnion, "wrong amount of results");

            Trace.WriteLine("amount of pot: matches in result set :" + amount);
            var groupsComplete =  result.GroupResultAsMatchingBlocks();

            //we do a filter selection 
            Trace.WriteLine("amount of groups: " + groupsComplete.Count);

            var groupsFiltered = result.FilterByConditon(config, 0.8f, 0.2f);
            Trace.WriteLine("amount of groups: " + groupsFiltered.Count);


        }
    }
}
