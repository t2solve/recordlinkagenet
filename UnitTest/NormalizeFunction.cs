using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Util;
using System;

namespace UnitTest
{
    public class NormalizeFunction
    {
        [TestMethod]
        public void TestNormalizeFunction()
        {
            float aValue = 5.0f;
            float bMax = 10.0f;

            Assert.IsTrue(NumberNormalizeHelper.NormalizeNumberToRange0to1(aValue, bMax) == 0.5f);

            float bMin = 0.0f;
            Assert.IsTrue(NumberNormalizeHelper.NormalizeNumberToRange0to1(aValue, bMax, bMin) == 0.5f);

            Assert.IsTrue(NumberNormalizeHelper.NormalizeNumberToRange0to1(100f, 100f, 0.0f) == 1f);

            Assert.IsTrue(NumberNormalizeHelper.NormalizeNumberToRange0to1(0.0f, 100f, 0.0f) == 0.0f);


            //expect exception
            try
            {
                NumberNormalizeHelper.NormalizeNumberToRange0to1(aValue, bMin, bMax);
                Assert.Fail();
            }
            catch (Exception)
            {
                //expected
            }
        }
    }
}