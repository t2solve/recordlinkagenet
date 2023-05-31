using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TableConversion
    {

        [TestMethod]
        public void TestSqliteReader()
        {
            string fileName = TestDataGenerator.WriteTestSqliteTableIfNotExists();

            var tab = SqliteReader.ReadTableFromSqliteFile(fileName, "PersonList");
            Assert.IsNotNull(tab);

            //we check some random rows 
            int amountRows = tab.GetAmountRows();
            Assert.IsTrue(amountRows > 0, "error 98298398");

                Random rnd = new Random();
            int amountChecks = 100; 
            for(int i = 0; i < amountChecks; i++)
            {
                int rowIndex = rnd.Next(0, amountRows);

                var row = tab.GetRow(rowIndex);
                foreach(var colName in row.Data.Keys)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(row.Data[colName].Value),
                        "error row:" + rowIndex+ " col: " + colName);
                }
            }

       }
    }
}
