using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core.Data.Transpose;
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
        [TestMethod]
        public void TestSqliteWriter()
        {
            int amountPersons = 1000;// (int)1E+6; //mio
            string tableName = "PersonList";
            List<TestDataPerson> testData =  TestDataGenerator.CreateTestPersons(amountPersons);
            var tabFeather1 = TableConverter.CreateTableFeatherFromDataObjectList(testData);

            //we write it down
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "foobarTest.db");
            bool succesWriteFlag =  SqliteWriter.WriteDataFeatherToSqlite(tabFeather1, tableName, fileName,true);
            Assert.IsTrue(succesWriteFlag); 

            var tabFeather2 = SqliteReader.ReadTableFromSqliteFile(fileName, tableName);

            Assert.IsNotNull(tabFeather2);

            Random rnd = new Random();
            int amountChecks = 100;
            for (int i = 0; i < amountChecks; i++)
            {
                int rowIndex = rnd.Next(0, amountPersons);

                var row1 = tabFeather1.GetRow(rowIndex);
                var row2 = tabFeather2.GetRow(rowIndex);

                foreach (var colName in row1.Data.Keys)
                {
                    Assert.IsTrue(string.Compare(row1.Data[colName].Value, row2.Data[colName].Value)==0,"error 239897889");
                       
                }
            }
            ////clean up
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}
        }
    }@
}
