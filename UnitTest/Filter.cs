using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data.Transpose;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UnitTest
{
    public class Filter
    {

        [TestMethod]
        public async void TestMinDistanceFilter()
        {
            List<TestDataPerson> testDataPeopleA = new List<TestDataPerson>
            {
                //we genrate a hand test list
                new TestDataPerson("Thomas", "Mueller", "Lindenstrasse", "Testhausen", "012345")
            };
            var tabA = TableConverter.CreateTableFeatherFromDataObjectList(testDataPeopleA);

            List<TestDataPerson> testDataPeopleB = new List<TestDataPerson>
            {
                new TestDataPerson("Thomas", "Mueller", "Lindetrasse", "Testhausen", "12345"),
                new TestDataPerson("Thomas", "Mueller", "Lindenstrasse", "Testcity", "012345"),
                new TestDataPerson("Thomas", "Müller", "Lindenstrasse", "Testcity", "012345"),
                new TestDataPerson("Tomas", "Müller", "Lindenstroad", "Testhausen", "012342"),
                new TestDataPerson("Tomas", "Müller", "Lindenstroad", "Dorf", "012342")
            };

            var tabB = TableConverter.CreateTableFeatherFromDataObjectList(testDataPeopleB);


            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            conList.String("NameFirst", "NameFirst", testMethod);
            conList.String("Street", "Street", testMethod);
            conList.String("PostalCode", "PostalCode", testMethod);
            conList.String("NameLast", "NameLast", testMethod);

            //add weight
            Dictionary<string, float> scoreTable = new Dictionary<string, float>
            {
                { "NameLast", 2.0f },
                { "NameFirst", 1.5f },
                { "Street", 0.9f },
                { "PostalCode", 0.7f }
            };

            //add weight
            foreach (Condition c in conList)
            {
                c.ScoreWeight = scoreTable[c.NameColNewLabel];
            }

            Configuration config = Configuration.Instance;
            config.AddIndex(new IndexFeather().Create(tabA, tabB));
            config.AddConditionList(conList);
            config.SetStrategy(Configuration.CalculationStrategy.WeightedConditionSum);


            //we init a worker
            WorkScheduler workScheduler = new WorkScheduler();
            var pipeLineCancellation = new CancellationTokenSource();
            var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

            await resultTask;

            int amountResults = resultTask.Result.Count();

            Assert.IsTrue(amountResults >= testDataPeopleB.Count);

            var orderedList = GroupFactory.GroupResultAsMatchingBlocks(resultTask.Result, GroupFactory.Type.IndexAIsKey);
            int fooCounter = orderedList.First().Count();
            Assert.IsTrue(fooCounter == amountResults);

            MatchGroupOrderedList filteredCandidate2 = FilterGroup(orderedList, 0.0f);
            Assert.IsTrue(filteredCandidate2.First().Count() == amountResults);


            MatchGroupOrderedList filteredCandidate3 = FilterGroup(orderedList, 0.01f);
            int fooCounter3 = filteredCandidate3.First().Count();
            Assert.IsTrue(fooCounter3 == 4);


            MatchGroupOrderedList filteredCandidate4 = FilterGroup(orderedList, 0.2f);
            int fooCounter4 = filteredCandidate4.First().Count();
            Assert.IsTrue(fooCounter4 == 0);

        }

        [TestMethod]
        public MatchGroupOrderedList FilterGroup(MatchGroupOrderedList gIn, float minValue)
        {
            MatchGroupOrderedList gOut = new MatchGroupOrderedList();

            ICandidateListFilter filter = new FilterCandidatesByMinDistanceToTopScore(minValue);
            foreach (var l1 in gIn)
            {
                var foo = filter.Apply(l1);
                gOut.Add(foo as MatchGroupOrdered);

            }
            //int fooCounter2 = gOut.First().Count();
            //Assert.IsTrue(fooCounter2 == amountResults);

            return gOut;
        }

    }
}
