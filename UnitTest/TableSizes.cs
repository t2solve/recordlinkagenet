using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Data.Transpose;
using RecordLinkageNet.Util;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class TableSizes
    {
        private bool CheckEstimate(long valuePresent, long valueMax)
        {
            if (valuePresent < valueMax)
                return true;

            return false;
        }
        private bool CheckEstimate(long valuePresent, double valueMax)
        {
            if (valuePresent < (long)valueMax)
                return true;

            return false;
        }

        //disabled because its system dependent test for ram size
        //[TestMethod] 
        //public void TestTableMemorySizes()
        //{
        //    long sizeStart = MemoryUsageEstimator.GetAmountMemoryWeUseFromGCInMiB();
        //    Assert.IsTrue(CheckEstimate(sizeStart, 10), "error 2372873 ");
        //    Trace.WriteLine("# start mem test:  \t " + sizeStart + "\tMiB");

        //    //we construct a test set 
        //    int amountData = (int)1E+6; //mio
        //    var testDataA = TestDataGenerator.CreateTestPersons(amountData);
        //    long sizePureObjectListSize = MemoryUsageEstimator.GetAmountMemoryWeUseFromGCInMiB();
        //    Assert.IsTrue(CheckEstimate(sizePureObjectListSize, 300), "error 253454 ");
        //    Trace.WriteLine("# object list mem :  " + sizePureObjectListSize + "\tMiB");


        //    //we generate a table feater
        //    DataTableFeather tabA = TableConverter.CreateTableFeatherFromDataObjectList(testDataA);
        //    double allowedFactor = 1.2f;
        //    long sizeNew = MemoryUsageEstimator.GetAmountMemoryWeUseFromGCInMiB();
        //    long sizeFeatherObject = sizeNew - sizePureObjectListSize;
        //    //around ~ 350 
        //    Assert.IsTrue(CheckEstimate(sizeFeatherObject, sizePureObjectListSize * allowedFactor), "error 236234 ");
        //    Trace.WriteLine("# table feather mem :  " + sizeFeatherObject + "\tMiB");


        //    //create test sqlite db
        //    string fileName = TestDataGenerator.WriteTestSqliteTableIfNotExists();

        //    //read sqlite table to system.data.datatab
        //    if (!string.IsNullOrEmpty(fileName))
        //    {
        //        using var sqliteConnection = new SqliteConnection("Data Source=" + fileName);
        //        {
        //            sqliteConnection.Open();
        //            var cmd = sqliteConnection.CreateCommand();
        //            cmd.CommandText = "SELECT * FROM PersonList";
        //            var rdr = cmd.ExecuteReader();
        //            //read to memory 
        //            DataTable table = new DataTable();
        //            table.Load(rdr);

        //            //check ram size
        //            allowedFactor = 1.7f;
        //            sizeNew = MemoryUsageEstimator.GetAmountMemoryWeUseFromGCInMiB();
        //            long sizeDataTable = sizeNew - sizeFeatherObject;
        //            //around ~ 580 is ~ factor 1.6 
        //            Trace.WriteLine("# system.data.table mem :  " + sizeDataTable + "\tMiB");

        //            Assert.IsTrue(CheckEstimate(sizeNew, sizeDataTable * allowedFactor), "error 5745745 ");
        //            //touch again to force gc to not do anything
        //            int foo = table.MinimumCapacity;
        //        }
        //    }

        //    //we touch the element for the gc
        //    testDataA.First();
        //}


    }
}
