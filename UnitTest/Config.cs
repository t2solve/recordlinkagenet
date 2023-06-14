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
    public class Config
    {

        [TestMethod]
        public void TestConfigValidate()
        {
            Assert.IsFalse(Configuration.Instance.IsValide());
            //add conditions
            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            Configuration.Instance.AddConditionList(conList);
            Assert.IsFalse(Configuration.Instance.IsValide());
            conList.String("NameFirst", "NameFirst", testMethod);
            Assert.IsFalse(Configuration.Instance.IsValide());

            //add data
            var tabA = TestDataGenerator.GenTestData();
            Configuration.Instance.AddIndex(new IndexFeather().Create(tabA, tabA));
            Assert.IsTrue(Configuration.Instance.IsValide());


            Configuration.Instance.Reset();
            Assert.IsFalse(Configuration.Instance.IsValide());

        }
    }
}
