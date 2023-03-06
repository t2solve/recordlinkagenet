using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using Microsoft.ML;
using Microsoft.ML.Data;

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
    }
    //small test data class
    public record TestDataPerson
    {
        [LoadColumn(0)]
        public string NameFirst { get; set; } = "";
        [LoadColumn(1)]
        public string NameLast { get; set; } = "";
        [LoadColumn(2)]
        public string Street { get; set; } = "";
        [LoadColumn(3)]
        public string City { get; set; } = "";
        [LoadColumn(4)]
        public string PostalCode { get; set; } = "";

    }
}
