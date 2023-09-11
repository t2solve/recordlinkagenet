using Bogus;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Data.Transpose;
using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UnitTest
{
    public class TestDataGenerator
    {
        public static List<TestDataPerson> ManipulateListAddAa(List<TestDataPerson> list)
        {
            foreach (TestDataPerson person in list)
            {
                person.NameFirst = 'a' + person.NameFirst.Remove(0, 1);
                person.NameLast = 'a' + person.NameLast.Remove(0, 1);
                person.City = 'a' + person.City.Remove(0, 1);

            }
            return list;
        }


        public static DataTableFeather GenTestData(int amount = 10)
        {
            var testData = TestDataGenerator.CreateTestPersons(amount);
            return TableConverter.CreateTableFeatherFromDataObjectList(testData);
        }

        public static List<TestDataPerson> CreateTestPersons(int x = 10)
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

        public static string WriteTestSqliteTableIfNotExists(int amountPersons = -1, string fileName = "", string tableName = "")
        {
            if (amountPersons == -1)
                amountPersons = (int)1E+6; //mio
            if (string.IsNullOrEmpty(fileName))
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "test.db");
            if (string.IsNullOrEmpty(tableName))
                tableName = "PersonList";

            //check if exsits
            if (File.Exists(fileName))
                return fileName;

            List<TestDataPerson> personList = CreateTestPersons(amountPersons);

            if (!SqliteWriter.WriteObjectListToSqliteStringBulkFile(personList, tableName, fileName))
            {
                Trace.WriteLine("error 2o389283 ");
                fileName = "";
            }
            return fileName;
        }
    }
    //small test data class
    [Serializable]
    public record TestDataPerson
    {
        public TestDataPerson()
        {
        }
        public TestDataPerson(string nameFirst, string nameLast, string street, string city, string postalCode)
        {
            NameFirst = nameFirst;
            NameLast = nameLast;
            Street = street;
            City = city;
            PostalCode = postalCode;
        }

        public string NameFirst { get; set; } = "";
        public string NameLast { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";

    }
}
