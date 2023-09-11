using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;

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
