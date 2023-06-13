using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkage.Core;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Data.Transpose;
using RecordLinkageNet.Core.Distance;
using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class ScoreWeight
    {
        private TestDataPerson DoADeepClone(TestDataPerson obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (TestDataPerson) formatter.Deserialize(ms);
            }
        }


        [TestMethod]
        public void TestScoreCalc()
        {
            int amountRows = 100;
            List<TestDataPerson> list1 = TestDataGenerator.CreateTestPersons(amountRows);
            var tabA = TableConverter.CreateTableFeatherFromDataObjectList(list1);
            List<TestDataPerson> list2 = new List<TestDataPerson>();

            foreach (var p in list1)
                list2.Add(DoADeepClone(p));

            //we manipulate
            list2 = TestDataGenerator.ManipulateListAddAa(list2);
            var tabB = TableConverter.CreateTableFeatherFromDataObjectList(list2);
            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            conList.String("NameFirst", "NameFirst", testMethod);
            conList.String("Street", "Street", testMethod);
            conList.String("PostalCode", "PostalCode", testMethod);
            conList.String("NameLast", "NameLast", testMethod);

            //add weight
            Dictionary<string, float> scoreTable = new Dictionary<string, float>();
            scoreTable.Add("NameLast", 1.0f);
            scoreTable.Add("NameFirst", 1.0f);
            scoreTable.Add("Street", 1.0f);
            scoreTable.Add("PostalCode", 1.0f);

            //add weight
            foreach (Condition c in conList)
            {
                c.ScoreWeight = scoreTable[c.NameColNewLabel];
            }

            Configuration config = Configuration.Instance;
            config.AddIndex(new IndexFeather().Create(tabA, tabB));
            config.AddConditionList(conList);
            int amountConditions = conList.Count();
            WeightedScoreProducer.Instance.SetMinScoreThresholdInPercentage(0.8f);

            MatchCandidateList candidateList = new MatchCandidateList();
            MatchCandidateList candidateListFull = new MatchCandidateList();
            Dictionary<IndexPair, MatchCandidate> candidateListFullRefs = new Dictionary<IndexPair, MatchCandidate>();

            float[,,] resultArray = new float[amountRows, amountRows, amountConditions];
            //init
            for (int i = 0; i < amountRows; i++)
                for (int j = i; j < amountRows; j++)
                    for (int x = 0; x < amountConditions; x++)
                        resultArray[i, j, x] = -1.0f;

            //we do  basic calc
            for (int i = 0; i < amountRows; i++)
            {
                for (int j = i; j < amountRows; j++)
                {
                    int conditionCounter = 0;
                    MatchCandidate candidate = new MatchCandidate(new IndexPair((uint)i, (uint)j));
                    WeightedScore weightedScore = new WeightedScore(candidate);

                    foreach (Condition con in conList)
                    {
                        DataColumn colA = tabA.GetColumnByName(con.NameColA);
                        DataColumn colB = tabB.GetColumnByName(con.NameColB);

                        //DataCell cellA = tabA.GetCellAt(i, j);
                        float resultCompare = (float)JaroWinkler.JaroDistance(colA.At(i).Value, colB.At(j).Value);
                        //store basic result
                        resultArray[i, j, conditionCounter] = resultCompare;

                        bool stillReach = WeightedScoreProducer.Instance.AddScorePartAndWeightIt(weightedScore, con.NameColNewLabel, resultCompare);

                        conditionCounter += 1;
                    }
                    if (WeightedScoreProducer.Instance.CheckScoreOverMinThreshold(weightedScore))
                    {
                        candidateList.Add(candidate);
                    }

                    candidateListFull.Add(candidate);
                    candidateListFullRefs.Add(candidate.GetIndexPair(), candidate);
                }
            }
            //we except a diagonal one 1,1 is same etc.
            List<IndexPair> exceptedIndexList = new List<IndexPair>();
            for (uint i = 0; i < amountRows; i++)
                exceptedIndexList.Add(new IndexPair(i, i));

            foreach (MatchCandidate mc in candidateList)
            {
                Assert.IsTrue(exceptedIndexList.Contains(mc.GetIndexPair()));

                //Trace.WriteLine(mc.GetIndexPair());
                //Trace.WriteLine(mc.GetScore().GetScoreValue());
                WeightedScore score = (WeightedScore)mc.GetScore();
                //Trace.WriteLine(WeightedScoreProducer.Instance.CalcRelativeScoreValueInPercentage(score));
                //Trace.WriteLine("#");
            }

            //we recalc the value
            var modus = Configuration.Instance.NumberTransposeModus;

            //we compare the calculation
            for (int i = 0; i < amountRows; i++)
            {
                for (int j = i; j < amountRows; j++)
                {
                    IndexPair p = new IndexPair((uint)i, (uint)j);
                    float result1 = 0.0f;
                    for (int x = 0; x < amountConditions; x++)
                    {
                        result1 += NumberTransposeHelper.TransposeFloatToByteRange01(resultArray[i, j, x], modus);
                    }

                    float result2 = candidateListFullRefs[p].GetScore().GetScoreValue();
                    Assert.AreEqual(result1, result2, double.Epsilon);

                }

            }


            //we add a filter test
            ICandidateListFilter filter = new FilterRelativMinScore(0.8f);
            MatchCandidateList listFiltered = filter.Apply(candidateListFull) as MatchCandidateList;

            Assert.IsTrue(listFiltered.Count() == candidateList.Count());

            //TODO check elements  !!!
            Dictionary<IndexPair,MatchCandidate>  mapList =  new Dictionary<IndexPair,MatchCandidate>();
            foreach (MatchCandidate mc in candidateList)
                mapList.Add(mc.GetIndexPair(), mc);

            foreach(MatchCandidate mcA in listFiltered)
            {
                Assert.IsTrue(mapList.ContainsKey(mcA.GetIndexPair()));
                Assert.IsTrue(mapList[mcA.GetIndexPair()] is MatchCandidate);
                    
                //we test if they are the same 
                MatchCandidate mcB = mapList[mcA.GetIndexPair()];
                Assert.IsTrue(mcA.Equals(mcB));    
            }
        }
        [TestMethod]
        public void TestScoringAbortPossibility()
        {
            DataTableFeather tabA = TestDataGenerator.GenTestData();

            //build a simle configuration
            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            conList.String("NameFirst", "NameFirst", testMethod);
            conList.String("Street", "Street", testMethod);
            conList.String("PostalCode", "PostalCode", testMethod);
            conList.String("NameLast", "NameLast", testMethod);

            //add weight
            Dictionary<string, float> scoreTable = new Dictionary<string, float>();
            scoreTable.Add("NameLast", 2.0f);
            scoreTable.Add("NameFirst", 1.5f);
            scoreTable.Add("Street", 0.9f);
            scoreTable.Add("PostalCode", 0.7f);


            //add weight
            foreach (Condition c in conList)
            {
                c.ScoreWeight = scoreTable[c.NameColNewLabel];
            }

            Configuration config = Configuration.Instance;
            config.AddIndex(new IndexFeather().Create(tabA, tabA));
            config.AddConditionList(conList);

            MatchCandidate mc = new MatchCandidate(new RecordLinkageNet.Core.Compare.IndexPair(0, 0));


            //general test
            WeightedScoreProducer.Instance.SetMinScoreThresholdInPercentage(0.6f);

            WeightedScore score1 = new WeightedScore(mc);
            float resultCompare = (float)JaroWinkler.JaroDistance(tabA.GetCellAt(0, 0).Value, tabA.GetCellAt(0, 0).Value);
            Assert.IsTrue(WeightedScoreProducer.Instance.AddScorePartAndWeightIt(score1, "NameFirst", resultCompare));


            //we do compare emulation 
            WeightedScore score2 = new WeightedScore(mc);
            Dictionary<string, float> testScoreCreate2 = new Dictionary<string, float>();
            testScoreCreate2.Add("NameLast", 1f);
            testScoreCreate2.Add("NameFirst", 1f);
            testScoreCreate2.Add("PostalCode", 1f);
            testScoreCreate2.Add("Street", 1f);

            foreach (var p in testScoreCreate2)
            {
                Assert.IsTrue(WeightedScoreProducer.Instance.AddScorePartAndWeightIt(score2, p.Key, p.Value));
            }
            //check
            Assert.IsTrue(score2.IsScoreComplete());

            //again
            WeightedScoreProducer.Instance.SetMinScoreThresholdInPercentage(0.5f);

            WeightedScore score3 = new WeightedScore(mc);
            Dictionary<string, float> testScoreCreate3 = new Dictionary<string, float>();
            testScoreCreate3.Add("NameLast", 0f);
            testScoreCreate3.Add("NameFirst", 0f);
            testScoreCreate3.Add("PostalCode", 0f);
            testScoreCreate3.Add("Street", 0f);

            for(int i=0;i<1;i++)
            {
                var p = testScoreCreate3.ElementAt(i);
                Assert.IsTrue(WeightedScoreProducer.Instance.AddScorePartAndWeightIt(score3, p.Key, p.Value));
            }
            for (int i = 1; i < 4; i++)
            {
                var p = testScoreCreate3.ElementAt(i);
                
                Assert.IsFalse(WeightedScoreProducer.Instance.AddScorePartAndWeightIt(score3, p.Key, p.Value));
            }


        }
    }
}
