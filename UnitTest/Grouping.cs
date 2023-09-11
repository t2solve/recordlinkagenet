using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class Grouping
    {
        private MatchCandidateList FakeIndexPairB(MatchCandidateList list, uint offset)
        {
            MatchCandidateList newList = new MatchCandidateList();
            foreach (MatchCandidate c in list)
            {
                IndexPair newP = new IndexPair(c.GetIndexPair().aIdx, c.GetIndexPair().bIdx + offset);
                MatchCandidate newC = new MatchCandidate(newP);
                newC.SetScore(c.GetScore());

                newList.Add(newC);
            }
            return newList;
        }

        private MatchCandidateList FakeIndexPairA(MatchCandidateList list, uint offset)
        {
            MatchCandidateList newList = new MatchCandidateList();
            foreach (MatchCandidate c in list)
            {
                IndexPair newP = new IndexPair(c.GetIndexPair().aIdx + offset, c.GetIndexPair().bIdx);
                MatchCandidate newC = new MatchCandidate(newP);
                newC.SetScore(c.GetScore());

                newList.Add(newC);
            }
            return newList;
        }

        [TestMethod]
        public async Task TestGroupToIndexAorB()
        {

            int amountData = 10;
            DataTableFeather tabA = TestDataGenerator.GenTestData(amountData);

            //build a simle configuration
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

            Configuration.Instance.Reset();
            Configuration config = Configuration.Instance;

            config.AddIndex(new IndexFeather().Create(tabA, tabA));
            config.AddConditionList(conList);
            config.SetStrategy(Configuration.CalculationStrategy.WeightedConditionSum);
            //config.Set = NumberTransposeHelper.TransposeModus.LOG10;
            //we do change some pre set things

            //we init a worker
            WorkScheduler workScheduler = new WorkScheduler();
            //workScheduler.EstimateWork();
            var pipeLineCancellation = new CancellationTokenSource();
            var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

            await resultTask;

            int amountResults = resultTask.Result.Count();

            //we assume a diagonal shoudl exists at least
            Assert.IsTrue(amountResults >= amountData);

            //filter
            FilterRelativMinScore filter = new FilterRelativMinScore(1f);

            MatchCandidateList filteredList = filter.Apply(resultTask.Result) as MatchCandidateList;
            //we do check we have the diagonal 1,1 2,2 etc
            for (uint i = 0; i < amountData; i++)
            {
                IndexPair testP = new IndexPair(i, i);
                Assert.IsTrue(filteredList.ContainsIndexPair(testP));
            }

            //we alter our index 
            //uint offset = 10;
            //MatchCandidateList fakedIndexListB = FakeIndexPairB(filteredList, offset);
            //MatchGroupOrderedList groupA = GroupFactory.GroupResultAsMatchingBlocks(fakedIndexListB, GroupFactory.Type.IndexAIsKey);
            //for (uint i = 0; i < amountData; i++)
            //{
            //    IndexPair testP = new IndexPair(i, i + offset);
            //    Assert.IsTrue(fakedIndexListB.ContainsIndexPair(testP));
            //    var foo = groupA.Data[(int)i];
            //    //we scan per hand
            //    bool success = false;
            //    foreach (MatchCandidate c in foo)
            //    {
            //        if (c.GetIndexPair().Equals(testP))
            //            success = true;
            //    }
            //    Assert.IsTrue(success,"error 23989238 candidate not found");
            //}

            //MatchCandidateList fakedIndexListA = FakeIndexPairA(filteredList, offset);
            //MatchGroupOrderedList groupB = GroupFactory.GroupResultAsMatchingBlocks(fakedIndexListA, GroupFactory.Type.IndexAIsKey);
            //for (uint i = 0; i < amountData; i++)
            //{
            //    IndexPair testP = new IndexPair(i + offset, i);
            //    Assert.IsTrue(fakedIndexListA.ContainsIndexPair(testP));
            //    var bar = groupB.Data[(int)i];
            //    //we scan per hand
            //    bool success = false;
            //    foreach (MatchCandidate c in bar)
            //    {
            //        if (c.GetIndexPair().Equals(testP))
            //            success = true;
            //    }
            //    Assert.IsTrue(success, "error 9283989  index access not successful");
            //}

        }
    }
}
