using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using RecordLinkageNet.Core;
using System;
using Microsoft.ML;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class CoreFunctionTests
    {
        [TestMethod]
        public void BasicSystemTest1()
        {
            Stopwatch sw = new Stopwatch();

            TestDataPerson[] inMemoryCollection = GenTestData();

            //we load the ml context for data parsing
            MLContext mlContext = new MLContext();
            IDataView dataA = mlContext.Data.LoadFromEnumerable<TestDataPerson>(inMemoryCollection);

            RecordLinkageNet.Core.Index indexer = new RecordLinkageNet.Core.Index();
            indexer.full();
            CandidateList can = indexer.index(dataA, dataA);
            Compare compare = new Compare(can);

            compare.Exact("PostalCode", "PostalCode");
            compare.String("NameFirst", "NameFirst", CompareCondition.StringMethod.JaroWinklerSimilarity, 0.9f);
            compare.String("NameLast", "NameLast", CompareCondition.StringMethod.JaroWinklerSimilarity, -1.0f, "foo");
            bool success = compare.Compute();

            Assert.IsTrue(success, "error during calc resulset");

            ResultSet res = compare.PackedResult;

            Assert.AreEqual(res.colNames[0], "PostalCode", "error names mismatch");
            Assert.AreEqual(res.colNames[1], "NameFirst", "error names mismatch");
            Assert.AreEqual(res.colNames[2], "foo", "error names mismatch");

            //res.data[] idx,condition
            //case a1 == b1 ? 
            Assert.AreEqual(1, res.data[0, 0], "resultset mismatch con1");
            Assert.AreEqual(1, res.data[0, 1], "resultset mismatch con2");
            Assert.AreEqual(1, res.data[0, 2], "resultset mismatch con3");
            //case a1 == b2 ?
            Assert.AreEqual(0, res.data[1, 0], "resultset mismatch con1");
            Assert.AreEqual(0, res.data[1, 1], "resultset mismatch con2");
            Assert.AreEqual(0, res.data[1, 2], "resultset mismatch con3");
            //case a2 == b1 ?
            Assert.AreEqual(0, res.data[2, 0], "resultset mismatch con1");
            Assert.AreEqual(0, res.data[2, 1], "resultset mismatch con2");
            Assert.AreEqual(0, res.data[2, 2], "resultset mismatch con3");
            //case a2 == b2 ?
            Assert.AreEqual(1, res.data[3, 0], "resultset mismatch con1");
            Assert.AreEqual(1, res.data[3, 1], "resultset mismatch con2");
            Assert.AreEqual(1, res.data[3, 2], "resultset mismatch con3");

            TimeSpan stopwatchElapsed = sw.Elapsed;
            Console.WriteLine("\tfinsihed used:\t" + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms");

        }

        [TestMethod]
        public void ReadAndWriteResultSetTestFunction()
        {
            TestDataPerson[] inMemoryCollection = GenTestData();

            //we load the ml context for data parsing
            MLContext mlContext = new MLContext();
            IDataView dataA = mlContext.Data.LoadFromEnumerable<TestDataPerson>(inMemoryCollection);

            RecordLinkageNet.Core.Index indexer = new RecordLinkageNet.Core.Index();
            indexer.full();
            CandidateList can = indexer.index(dataA, dataA);
            Compare compare = new Compare(can);

            compare.Exact("PostalCode", "PostalCode");
            compare.String("NameFirst", "NameFirst", CompareCondition.StringMethod.JaroWinklerSimilarity, 0.9f);
            compare.String("NameLast", "NameLast", CompareCondition.StringMethod.JaroWinklerSimilarity, -1.0f, "foo");
            compare.Compute();
            ResultSet resIn = compare.PackedResult;
            string folderWeStoreIn = Path.Combine(Environment.CurrentDirectory, "testdir");

            Console.WriteLine(folderWeStoreIn);
            bool succes = ResultSetIOHelper.WriteResultSetToFolder(folderWeStoreIn,resIn);
            Assert.IsTrue(succes, "error during write");

            ResultSet resOut = ResultSetIOHelper.ReadResultSetToFolder(folderWeStoreIn);
            Assert.IsNotNull(resOut);

            //we compare the sets 
            Assert.AreEqual(resIn.colNames[0], resOut.colNames[0]);

            Assert.AreEqual(resIn.data[0,1], resOut.data[0,1], "wrong data");

        }

        private TestDataPerson[] GenTestData()
        {
            //we do basic functional
            TestDataPerson[] inMemoryCollection = new TestDataPerson[]
            {
                    new TestDataPerson
                    {
                        NameFirst = "thomas",
                        NameLast = "mueller",
                        City = "moxee",
                        Street = "91st ave",
                        PostalCode = 98944
                    },
            new TestDataPerson
            {

                NameFirst = "adalina",
                NameLast = "nibbs",
                City = "waterville",
                Street = "jefferson",
                PostalCode = 98858
            }
            };

            return inMemoryCollection;
        }
    }
}
