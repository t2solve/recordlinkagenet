using AutoBogus;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;
using RecordLinkageNet.Core.Data;
using System.Reflection;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]

    public class TestDataStructureCompareLinq
    {

        private List<TestDataPerson> GenTestPersons(int x = 10)
        {
            var userInfoFaker = new Faker<TestDataPerson>(locale: "de")
              //This will ensure  that all properties have rules defined -- Default is false
              //.StrictMode(true)
              //This sets a rule so that each id value is generated with a new GUID value
              //.RuleFor(o => o.Id, f => Guid.NewGuid())

              .RuleFor(o => o.NameFirst, f => f.Name.FirstName())
              .RuleFor(o => o.NameLast, f => f.Name.LastName())
              .RuleFor(o => o.City, f => f.Address.City())
              .RuleFor(o => o.Street, f => f.Address.StreetName())
              .RuleFor(o => o.PostalCode, f => f.Address.ZipCode());

            List<TestDataPerson> list = userInfoFaker.Generate(x);
            return list; 
        }
        [TestMethod]
        public void TestAutoBugsReadToDataStructure()
        {
            //generate test data
            var testDataA = GenTestPersons(1000);
            var testDataB = GenTestPersons(1000);
            var testDataUnion = GenTestPersons(100); //generate union amount
            testDataA.AddRange(testDataUnion);
            testDataB.AddRange(testDataUnion);
            int amountRows = testDataA.Count; 

            //we construct two tables
            // conert 
            DataTable tabA = new DataTable();
            tabA.AddDataClassAsColumns(new TestDataPerson(), amountRows);
            foreach(TestDataPerson p in testDataA)            //we add all cells 
            {
                tabA.AddRow(p);
            }

            DataTable tabB = new DataTable();
            tabB.AddDataClassAsColumns(new TestDataPerson(), amountRows);
            foreach (TestDataPerson p in testDataB) //we add all cells 
            {
                tabB.AddRow(p);
            }

            //we compare 

        }
    }

}
