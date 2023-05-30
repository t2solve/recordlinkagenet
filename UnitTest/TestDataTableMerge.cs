using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Data.Transpose;
using RecordLinkageNet.Core.Transpose;

namespace UnitTest
{
    [TestClass]

    public class TestDataTableMerge
    {

        private DataTableFeather GenTestData(int amount = 10)
        {
            var testData = TestDataGenerator.CreateTestPersons(amount);
            return TableConverter.CreateTableFeatherFromDataObjectList(testData);
        }

        [TestMethod]
        public void TestMergeIntegration()
        {

            //we construct two tables
            DataTableFeather tabA = GenTestData();
            DataTableFeather tabB = GenTestData();

            DataTableMerger merger = new DataTableMerger();
            List<IndexPair> indexList = new List<IndexPair>();
            //we add diagonal
            for (uint i = 0; i < tabA.GetAmountRows(); i++)
                indexList.Add(new IndexPair(i, i));

            DataTableFeather tabMerged =  merger.MergeColumnsDataByIndexList(indexList, tabA, tabB,true);
            //DataTableWriter.WriteAsCSV("Z:dataMerged.csv", tabMerged);
            //DataTableWriter.WriteAsCSV("Z:dataA.csv", tabA);
            //DataTableWriter.WriteAsCSV("Z:dataB.csv", tabB);

            int amountDataMemberInTestClass = 5; 
            //we test for specific cells 
            Assert.AreEqual(tabA.GetCellAt(0, 0).Value, tabMerged.GetCellAt(0, 0).Value, "error during merge");
            Assert.AreEqual(tabB.GetCellAt(0, 0).Value, tabMerged.GetCellAt(0 + amountDataMemberInTestClass, 0).Value, "error during merge");
            //test all a 
            for (int j = 0; j < tabA.GetAmountColumns(); j++)
                for (int i = 0; i < tabA.GetAmountRows(); i++)
                    Assert.AreEqual(tabA.GetCellAt(j, i).Value, tabMerged.GetCellAt(j, i).Value, "error during merge");

            //test all b
            for (int j = 0; j < tabB.GetAmountColumns(); j++)
                for (int i = 0; i < tabB.GetAmountRows(); i++)
                    Assert.AreEqual(tabB.GetCellAt(j, i).Value, tabMerged.GetCellAt(j + amountDataMemberInTestClass, i).Value, "error during merge");
        }

        [TestMethod]
        public void TestCountDoubleRowsInIndexList()
        {

            DataTableFeather tabA = GenTestData();

            DataTableMerger merger = new DataTableMerger();
            List<IndexPair> indexList = new List<IndexPair>();

            int amountRowA = tabA.GetAmountRows();
            //we add diagonal
            for (uint i = 0; i < amountRowA; i++)
                indexList.Add(new IndexPair(i, i));

            Assert.AreEqual(10, merger.CountAmountOfRowsFromIndexList(indexList),
                "error 23482347878 wrong index list row calc" );

            //we add diagonal
            for (uint i = 0; i < tabA.GetAmountRows(); i++)
                indexList.Add(new IndexPair(i, i));

            Assert.AreEqual(20, merger.CountAmountOfRowsFromIndexList(indexList),
            "error 345636734 wrong index list row calc");

        }

        [TestMethod]
        public void TestCheckDoubledColumnNames()
        {
            //we construct two tables
            DataTableFeather tabA = GenTestData();
            DataTableFeather tabB = GenTestData();

            DataTableMerger merger = new DataTableMerger();
            Assert.IsTrue(merger.CheckTableColumnNamesAreEqual(tabA, tabB),
                "error 29389328 not detect doubled names columns");
        }
    }
}
