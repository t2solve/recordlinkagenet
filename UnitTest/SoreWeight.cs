using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Distance;
using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class ScoreWeight
    {
        [TestMethod]
        public void TestScoring()
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
