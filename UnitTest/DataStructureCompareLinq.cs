using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Data.Transpose;
using System.Linq;

namespace UnitTest
{
    [TestClass]

    public class DataStructureCompareLinq
    {

        [TestMethod]
        public void TestAutoBugsReadToDataStructure()
        {
            //generate test data
            var testDataA = TestDataGenerator.CreateTestPersons(1000);
            var testDataB = TestDataGenerator.CreateTestPersons(1000);
            var testDataUnion = TestDataGenerator.CreateTestPersons(100); //generate union amount
            testDataA.AddRange(testDataUnion);
            testDataB.AddRange(testDataUnion);
            int amountRows = testDataA.Count;

            //we construct two tables
            // convert 
            DataTableFeather tabA = TableConverter.CreateTableFeatherFromDataObjectList(testDataA);
            DataTableFeather tabB = TableConverter.CreateTableFeatherFromDataObjectList(testDataB);
            //we compare 
            //we use linq 
            //working
            var joined = from Item1 in tabA.GetColumnByName("NameLast")
                         join Item2 in tabB.GetColumnByName("NameLast")
                         on Item1.Value equals Item2.Value  // join on some property
                         select new { IdA = Item1.Id, IdB = Item2.Id };

            //int counter = 0;
            int elementCount = joined.Count();

            Assert.IsTrue(elementCount >= 100 && elementCount <= 1000, "error linq interface for datatable not working");

            //foreach (var foo in joined)
            //{
            //    Trace.WriteLine(foo);
            //    counter += 1;
            //}
            //Trace.WriteLine(joined);
        }
    }

}
