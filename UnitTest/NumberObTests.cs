using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class NumberObTests
    {
        [TestMethod]
        public void TestLog10ExaktMatchBothDirections()
        {
            Dictionary<byte, float> testExaktBoundary = new Dictionary<byte, float>();

            testExaktBoundary.Add(2, 0.198140055946704f);
            testExaktBoundary.Add(250, 0.996542243139966f);
            testExaktBoundary.Add(254, 0.999393762638686f);

            foreach ( var p in testExaktBoundary )
            { 

                byte valByte = NumberTransposeHelper.TransposeFloatToByteRange01(p.Value, NumberTransposeHelper.TransposeModus.LOG10);
                Assert.IsTrue(valByte == p.Key, "missmatch  FloatToByte  byte:\t"+ p.Key + "\tfloat:\t" + p.Value);

                float valFloat = NumberTransposeHelper.TransposeByteToFloatRange01(p.Key, NumberTransposeHelper.TransposeModus.LOG10);
                Assert.IsTrue(p.Value == valFloat, "missmatch  ByteToFloat  byte:\t" + p.Key + "\tfloat:\t" + p.Value);
            }

        }

        public void TestLog10FloatToByte()
        {
            Dictionary<float, byte> testCategory = new Dictionary<float, byte>();

            testCategory.Add(0.009f, 1);
            testCategory.Add(0.1f, 1);
            testCategory.Add(0.15f, 2);

            testCategory.Add(0.5f, 15);
            testCategory.Add(0.52f, 16);
            testCategory.Add(0.9999f, 254); 

            foreach (var p in testCategory)
            {
                byte valByte = NumberTransposeHelper.TransposeFloatToByteRange01(p.Value, NumberTransposeHelper.TransposeModus.LOG10);
                Assert.IsTrue(valByte == p.Key, "missmatch  FloatToByte  byte:\t" + p.Key + "\tfloat:\t" + p.Value);
            }
        }

        [TestMethod]
        public void TestZero()
        {
            //float to byte
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(0f, NumberTransposeHelper.TransposeModus.LOG10), byte.MinValue, "error during 0 test");
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(0f, NumberTransposeHelper.TransposeModus.LINEAR), byte.MinValue, "error during 0 test");
            //byte to float
            Assert.AreEqual(NumberTransposeHelper.TransposeByteToFloatRange01(0, NumberTransposeHelper.TransposeModus.LOG10), 0f, "error during 0 test");
            Assert.AreEqual(NumberTransposeHelper.TransposeByteToFloatRange01(0, NumberTransposeHelper.TransposeModus.LINEAR), 0f, "error during 0 test");

        }
        [TestMethod]
        public void TestOne()
        {
            //float to byte
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(1f, NumberTransposeHelper.TransposeModus.LOG10), byte.MaxValue, "error during 1 test");
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(1f, NumberTransposeHelper.TransposeModus.LINEAR), byte.MaxValue, "error during 1 test");

            //byte to float
            Assert.AreEqual(NumberTransposeHelper.TransposeByteToFloatRange01(byte.MaxValue, NumberTransposeHelper.TransposeModus.LOG10), 1f, "error during 0 test");
            Assert.AreEqual(NumberTransposeHelper.TransposeByteToFloatRange01(byte.MaxValue, NumberTransposeHelper.TransposeModus.LINEAR), 1f, "error during 0 test");

        }
        [TestMethod]
        public void TestRangeLimit()
        { 
            //upper 
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(1.2f, NumberTransposeHelper.TransposeModus.LOG10), byte.MaxValue, "error during upper range test");
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(239898f, NumberTransposeHelper.TransposeModus.LINEAR), byte.MaxValue, "error during upper range test");
            //lower
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(-1.0f, NumberTransposeHelper.TransposeModus.LOG10), byte.MinValue, "error during lower range test");
            Assert.AreEqual(NumberTransposeHelper.TransposeFloatToByteRange01(-0.01f, NumberTransposeHelper.TransposeModus.LINEAR), byte.MinValue, "error during lower range test");
        }
    }
    
}